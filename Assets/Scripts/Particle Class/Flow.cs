using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flow : MonoBehaviour
{
    public GameObject memoGraphic;
    public GameObject neuronGraphic;
    private TrailRenderer trail;
    public static int memoCount = 500;
    public static int neuronCount = 100;
    private Memo[] memos = new Memo[memoCount];
    private Neuron[] neurons = new Neuron[neuronCount];

    // Start is called before the first frame update
    void Start()
    {
        trail = memoGraphic.GetComponent<TrailRenderer>();
        for (int i = 0; i < memoCount; i++)
        {
            Memo memo = new Memo(i, memoGraphic);
            memos[i] = memo;
        }
        for (int i = 0; i < neuronCount; i++)
        {
            Neuron neuron = new Neuron(i, neuronGraphic);
            neurons[i] = neuron;
        }

    }

    // Update is called once per frame
    void Update()
    {
        foreach(Memo memo in memos)
        {
            memo.Attract(neurons);
            memo.Update();
        }
    }
}
