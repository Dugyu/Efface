using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flow : MonoBehaviour
{
    public GameObject memoGraphic;
    public GameObject neuronGraphic;
    private TrailRenderer trail;
    public static int memoCount = 100;
    public static int neuronCount = 100;
    private List<Memo> memos = new List<Memo>();
    private Neuron[] neurons = new Neuron[neuronCount];

    // Start is called before the first frame update
    void Start()
    {
        trail = memoGraphic.GetComponent<TrailRenderer>();
        for (int i = 0; i < memoCount; i++)
        {
            Memo memo = new Memo(memoGraphic);
            memos.Add(memo);
        }
        for (int i = 0; i < neuronCount; i++)
        {
            Neuron neuron = new Neuron(i, neuronGraphic);
            neurons[i] = neuron;
        }

    }

    Memo[] branch(Memo m, int branchCount, float branchVel)
    {
        Memo[] mb = new Memo[branchCount];
        Vector3 vz = m.vel;
        vz.Normalize();

        Vector3 vx = new Vector3(1.0f, 0.0f, 0.0f);
        Vector3 vy = Vector3.Cross(vz, vx);
        vy.Normalize();
        vx = Vector3.Cross(vy, vz);

        float angleStep = Mathf.PI * 2.0f / (float)branchCount;

        for(int i=0; i<branchCount; ++i)
        {
            Memo mn = new Memo( m.obj);
            float angle = i * angleStep;
            Vector3 vperp = vx * Mathf.Cos(angle) + vy * Mathf.Sin(angle);
            mn.vel = m.vel + vperp * branchVel;
            mn.pos = m.pos + vperp * 0.0001f;

            mb[i] = mn;
        }
        return mb;
    }
    // Update is called once per frame
    void Update()
    {
        List<Memo> branches = new List<Memo>();
        foreach (Memo memo in memos)
        {
            bool shouldBranch = false;
            if (shouldBranch)
            {
                Memo[] mm = branch(memo, 3, 0.1f);
                branches.AddRange(mm);
            }
        }

        memos.AddRange(branches);

        foreach (Memo memo in memos)
        {
            memo.interForce = Vector3.zero;
            memo.targetMem = null;
        }

        for (int j=0; j<memos.Count; ++j)
        {
            for (int i=j+1; i<memos.Count; ++i)
            {
                Vector3 dm = memos[i].pos - memos[j].pos;
                float d = dm.magnitude;

                if (d <= 0.0001f) continue;
                Vector3 f = dm * (Mathf.Exp(-0.001f*d*d)/d)*0.05f;
                memos[i].interForce -= f;
                memos[j].interForce += f;
            }
        }

        foreach (Memo memo in memos)
        {
            memo.Attract(neurons);
            memo.Update();
        }
    }
}
