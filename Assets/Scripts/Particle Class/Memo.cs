using System.Collections.Generic;
using UnityEngine;

public class Memo
{
    public float maxRadius = 60.0f;
    public float maxheight = 100.0f;
    public float gravity = -0.5f;

    public GameObject obj;
    public Vector3 pos = Vector3.zero;
    public Vector3 vel = Vector3.zero;
    public Vector3 acc = Vector3.zero;
    public float m = 1.0f;
    public float mood;
    public int id;

    public Vector3 interForce = Vector3.zero;
    public Memo targetMem = null;

    public Neuron targetNeuron = null;
    public int movePhase = 0;
    public int lastMovePhase = 0;


    static Neuron[] pointerNeurons;   


    public LinkedList<Vector3> trail = new LinkedList<Vector3>();
    public LinkedList<float> trailWidth = new LinkedList<float>();

    public bool isEnd = false;
    public bool isStart = false;

    static LinkedList<int> unusedIndices = new LinkedList<int>();
    static int lastIndex = 1;

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
         Mood();
    }


    //public void Reset1()
    //{
    //    float angle = Random.value * Mathf.PI * 2;
    //    float r = RandomGaussian(20.0f, 10.0f) + maxRadius * (1.0f - Mathf.Pow(Random.value, 7.0f));
    //    float x = Mathf.Cos(angle) * r;
    //    float z = Mathf.Sin(angle) * r;
    //    float y = maxheight + maxheight * 0.5f * Mathf.Pow(Random.value, 7.0f);

    //    pos = new Vector3(x, y, z);
    //    vel = new Vector3(0.0f, gravity, 0.0f);
    //    acc = Vector3.zero;
    //    obj.transform.position = pos;
    //    Mood();
    //}

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
        int selected = Random.Range(0, pointerNeurons.Length);
        targetNeuron = pointerNeurons[selected];
    }

    public void FindFisrtNode()
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
            }
        }
        else
        {
            Move();
            DrawTrail();
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

    public void DrawTrail()
    {

        trail.AddLast(pos); 

        if (isEnd == true || isStart == true)
        {
            trailWidth.AddLast(0.0f);
            Debug.Log(trailWidth.Last.Value.ToString());
        }
        else
        {
            trailWidth.AddLast(0.5f);
        }

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
