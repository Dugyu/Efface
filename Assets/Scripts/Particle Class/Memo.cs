using System.Collections.Generic;
using UnityEngine;

public class Memo
{
    // shared
    //static float maxRadius = 60.0f;
    static float maxheight = 100.0f;
    static float gravity = -0.5f;


    // self
    public GameObject obj;
    public Vector3 pos = Vector3.zero;
    public Vector3 vel = Vector3.zero;
    public Vector3 acc = Vector3.zero;
    public float m = 1.0f;
    public float mood;
    public int id;


    // interforce
    public Vector3 interForce = Vector3.zero;
    public Memo targetMem = null;


    //find and attract to neuron 
    static Neuron[] pointerNeurons;
    public Neuron targetNeuron = null;
    public int movePhase = 0;
    public int lastMovePhase = 0;


    // draw triangle fragments
    public Crystal crystal;
    public Vector3 lastRecordedPos;
    public Vector3[] triVertsBuffer = new Vector3[6];


    // draw trails
    public LinkedList<Vector3> trail = new LinkedList<Vector3>();
    public LinkedList<float> trailWidth = new LinkedList<float>();


    // limited resources
    static LinkedList<int> unusedIndices = new LinkedList<int>();
    static int lastIndex = 1;


    // limited resources
    static int getIndex()
    {
        int idx = 0;
        if (unusedIndices.Count != 0)
        {
            idx = unusedIndices.Last.Value;
            unusedIndices.RemoveLast();
        }
        else
        {
            idx = lastIndex;
            lastIndex++;
        }

        return idx;
    }

    static void releaseIndex(int idx)
    {
        unusedIndices.AddLast(idx);
    }

    public static void SetPointerNeurons(Neuron[] neurons)
    {
        pointerNeurons = neurons;
    }



    // initialize
    public Memo(GameObject _memo)
    {
        
        id = getIndex();
        obj = Object.Instantiate(_memo);
        Reset();
    }

    public void Release()
    {
        releaseIndex(id);
        Object.Destroy(obj);
        obj = null;
    }

    public float RandomGaussian(float mean, float stdDev)
    {
        float u1 = 1.0f - Random.value;       //uniform(0,1] random doubles
        float u2 = 1.0f - Random.value;
        float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2);//random normal(0,1)
        float randNormal = mean + stdDev * randStdNormal;  //random normal(mean,stdDev^2)
        return randNormal;
    }

    public void Reset()
    {
         movePhase = 0;
         lastMovePhase = 0;
         ChooseFisrtNode();

         pos = targetNeuron.pos;
         pos.y = maxheight + maxheight * 0.5f * Mathf.Pow(Random.value, 7.0f);

         vel = new Vector3(0.0f, gravity, 0.0f);
         acc = Vector3.zero;
         obj.transform.position = pos;
         //Mood();

         lastRecordedPos = pos;
          
    }

    public void Mood()
    {
        mood = Mathf.Sin(Mathf.PerlinNoise(pos.x / 10.0f, pos.y / 10.0f) * Mathf.PI * 2);
    }

   
    public float CompareMood(Neuron neuron)
    {
        return 1.0f - Mathf.Abs(neuron.mood - mood);
    }


    public void ChooseFisrtNode()
    {
        float[] weight = new float[pointerNeurons.Length];
        float totalSum = 0.0f;
        for (int i = 0; i < weight.Length; ++i)
        {
            float thisSum = pointerNeurons[i].mood;
            //for (int j = 0; j < pointerNeurons[i].childNeurons.Count; ++j)
            //{
            //    thisSum += pointerNeurons[i].childNeurons[j].mood;
            //}
            
            weight[i] = thisSum;
            totalSum += thisSum;
        }
        float pointerSum = 0.0f;

        float value = Random.Range(0, totalSum);
        int selected = 0;
        for (int i = 0; i < weight.Length; ++i)
        {
            pointerSum += weight[i];
            if (pointerSum > value){
                selected = i;
                break;
            }
        }

        //int selected = Random.Range(0, pointerNeurons.Length);
        targetNeuron = pointerNeurons[selected];
    }

    public void FindFirstNode()
    {
        float mindist = Mathf.Infinity;
        int index = 0;

        foreach (Neuron neuron in pointerNeurons)
        {
            Vector3 dir = neuron.pos - pos;
            dir.y = 0.0f;
            float sqrDist = dir.sqrMagnitude;
            ;
            if (sqrDist < mindist)
            {
                mindist = sqrDist;
                index = neuron.id;
            }
        }
        targetNeuron = pointerNeurons[index];
    }

    public void Attract(float strength)
    {
        if (Mathf.Abs(targetNeuron.pos.y - pos.y) < 5.0f)
        {
            movePhase += 1;
        }

        if (movePhase != lastMovePhase && targetNeuron.childNeurons.Count > 0)
        {
            //gravity = 0.0f;
            int select = Random.Range(0, targetNeuron.childNeurons.Count);
            targetNeuron = targetNeuron.childNeurons[select];
            acc =  Vector3.zero;
            lastMovePhase = movePhase;
        }

        if (movePhase == lastMovePhase)
        {
            Vector3 dir = targetNeuron.pos - pos;
            float sqrDist = dir.sqrMagnitude;
            dir.Normalize();
            acc = 2 * strength * dir * sqrDist *0.00001f;//(Mathf.Exp(-0.01f * sqrDist));
        }
        else
        {
            vel = new Vector3(0.0f, gravity, 0.0f);
        }
    }


    public void Update()
    {
        if (pos.y < 0)
        {
            DrawTrail();
            if (trail.First.Value.y < 0)
            {
                trail = new LinkedList<Vector3>();
                trailWidth = new LinkedList<float>();
                Reset();
                InitializeTriBuffer();
            }
        }
        else
        {
            Move();
            //DrawTrail();
            DrawTriangle();
        }
    }


    
    public void Move()
    {     
        ApplyGravity();
        vel *= 0.8f;
        vel += acc+interForce;
        pos += vel*0.08f;
        obj.transform.position = pos;
    }


    public void InitializeTriBuffer()
    {
        Vector3 vz = vel;
        vz.Normalize();

        Vector3 vx = Vector3.right;
        Vector3 vy = Vector3.Cross(vz, vx);
        vy.Normalize();

        for (int i = 3; i < 6; ++i)
        {

            float angle = Random.value * Mathf.PI * 2;
            Vector3 vperp = vx * Mathf.Cos(angle) + vy * Mathf.Sin(angle);
            Vector3 pt = pos + vperp * 2.0f * (1.0f - Mathf.Pow(Random.value, 7)) + vz * 5.0f * (1.0f - Mathf.Pow(Random.value, 7));
            triVertsBuffer[i] = pt;
        }


    }



    public void DrawTriangle()

    {
        
        Vector3 dir = pos - lastRecordedPos;
        if (dir.sqrMagnitude > 4.0f)
        {
            Vector3 vz = vel;
            vz.Normalize();

            Vector3 vx = Vector3.right;
            Vector3 vy = Vector3.Cross(vz, vx);
            vy.Normalize();

            for (int i = 0; i < 6; ++i)
            {
 
                float angle = Random.value * Mathf.PI * 2;
                Vector3 vperp = vx * Mathf.Cos(angle) + vy * Mathf.Sin(angle);
                Vector3 pt = pos + vperp * 1.0f *(1.0f - Mathf.Pow(Random.value, 7)) + vz * 0.5f * (1.0f - Mathf.Pow(Random.value, 7));
                triVertsBuffer[i] = pt;
                if (i == 2)
                {
                    crystal.UpdateMesh(triVertsBuffer);
                }
            }
            lastRecordedPos = pos;
        }
        
    }


    
    public void DrawTrail()
    {

        trail.AddLast(pos);
        trailWidth.AddLast(0.5f);

        if (trail.Count > 500)
        {
            trail.RemoveFirst();
            trailWidth.RemoveFirst();
        }

        LineRenderer r = obj.GetComponent<LineRenderer>();
        if (trail.Count < 2)
        {
            return;
        }

        else
        {
            Vector3[] pp = new Vector3[trail.Count];
            int i = 0;
            foreach (Vector3 v in trail)
            {
                pp[i++] = v;
            }

            r.positionCount = pp.Length;
            r.SetPositions(pp);


            Keyframe[] kf = new Keyframe[pp.Length];
            int j = 0;
            foreach (float w in trailWidth)
            {
                kf[j++] = new Keyframe(j / (float)kf.Length, w);
            }

            r.widthCurve = new AnimationCurve(kf);
        }
       
    }

    public void ApplyGravity()
    {
        acc = new Vector3(acc.x, acc.y + gravity, acc.z);
    }

}
