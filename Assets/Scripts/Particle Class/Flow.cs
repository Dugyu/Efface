using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flow : MonoBehaviour
{
    public float strength;
    public static int memoCount = 50;
    public static int neuronCount = 50;
    public static int layerCount = 5;
    public static int neuronsPerLayer = neuronCount / layerCount;

    private List<Memo> memos = new List<Memo>();
    private Neuron[] neurons = new Neuron[neuronCount];
    private Neuron[] neuronFirstLayer = new Neuron[neuronsPerLayer];

    public GameObject memoGraphic;
    public GameObject neuronGraphic;
    private TrailRenderer trail;
    public GameObject crystalGraphic;

    void Start()
    {
        trail = memoGraphic.GetComponent<TrailRenderer>();

        
        // initialize neurons
        for (int j = 0; j < layerCount; j++)
        {
            for (int i = 0; i < neuronsPerLayer; i++)
            {
                Neuron neuron = new Neuron(i, j, neuronGraphic);
                neurons[(j*neuronsPerLayer) + i] = neuron;
            }   
        }

        neuronGraphic.GetComponent<MeshRenderer>().enabled = false;

        // copy first layer neuron to array
        System.Array.Copy(neurons, 0 , neuronFirstLayer, 0, neuronsPerLayer);
        // find firt nodes for memos
        Memo.SetPointerNeurons(neuronFirstLayer);
        for (int i = 0; i < memoCount; i++)
            {
                Memo memo = new Memo(memoGraphic);
                //memo.obj.GetComponent<MeshRenderer>().enabled = false;
                memo.InitializeTriBuffer();
                memo.crystal = new Crystal(crystalGraphic);
                memos.Add(memo);
            }

        // construct neuron child and parent relation graph
        for (int j = 0; j < layerCount - 1; j++)
        {
            
            Neuron[] neuronsThisLayer = new Neuron[neuronsPerLayer];
            System.Array.Copy(neurons, (j + 1) * neuronsPerLayer, neuronsThisLayer, 0, neuronsPerLayer);
           
            for (int i = 0; i < neuronsPerLayer; i++)
            {
                Neuron neuron = neurons[(j * neuronsPerLayer) + i];
                neuron.ChooseChildNeuron(neuronsThisLayer);
            }

        }

    }

    //Memo[] branch(Memo m, int branchCount, float branchVel)
    //{
    //    Memo[] mb = new Memo[branchCount];
    //    Vector3 vz = m.vel;
    //    vz.Normalize();

    //    Vector3 vx = new Vector3(1.0f, 0.0f, 0.0f);
    //    Vector3 vy = Vector3.Cross(vz, vx);
    //    vy.Normalize();
    //    vx = Vector3.Cross(vy, vz);

    //    float angleStep = Mathf.PI * 2.0f / (float)branchCount;

    //    for(int i=0; i<branchCount; ++i)
    //    {
    //        Memo mn = new Memo( m.obj);
    //        float angle = i * angleStep;
    //        Vector3 vperp = vx * Mathf.Cos(angle) + vy * Mathf.Sin(angle);
    //        mn.vel = m.vel + vperp * branchVel;
    //        mn.pos = m.pos + vperp * 0.0001f;

    //        mb[i] = mn;
    //    }
    //    return mb;
    //}


    void Update()
    {

        foreach (Memo memo in memos)
        {
            memo.Attract(strength);
            //Debug.Log(memo.targetNeuron.id);
            memo.Update();
        }

        foreach (Neuron neuron in neurons)
        {
            Color existing = neuron.obj.GetComponent<MeshRenderer>().material.color;
            if (existing != Color.black)
            {
                Color target = existing * 0.99f + Color.black * 0.01f;
                Debug.Log(neuron.id);
                neuron.obj.GetComponent<MeshRenderer>().material.color = target;
                
            }

            neuron.UpdateMood();
        }
        //List<Memo> branches = new List<Memo>();
        //foreach (Memo memo in memos)
        //{
        //    bool shouldBranch = false;
        //    if (shouldBranch)
        //    {
        //        Memo[] mm = branch(memo, 3, 0.1f);
        //        branches.AddRange(mm);
        //    }
        //}

        //memos.AddRange(branches);

        //foreach (Memo memo in memos)
        //{
        //    memo.interForce = Vector3.zero;
        //    memo.targetMem = null;
        //}

        //for (int j=0; j<memos.Count; ++j)
        //{
        //    for (int i=j+1; i<memos.Count; ++i)
        //    {
        //        Vector3 dm = memos[i].pos - memos[j].pos;
        //        float d = dm.magnitude;

        //        if (d <= 0.0001f) continue;
        //        Vector3 f = dm * (Mathf.Exp(-0.001f*d*d)/d)*0.05f;
        //        memos[i].interForce -= f;
        //        memos[j].interForce += f;
        //    }
        //}

    }
}
