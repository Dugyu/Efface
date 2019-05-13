using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neuron 
{
    static float maxRadius = 200.0f;
    static float maxheight = 20.0f;
    public GameObject obj;
    public Vector3 pos = Vector3.zero;
    public float mood;
    public float innerRing; 
    public float outerRing;
    public int id;
    public int level;


    public List<Neuron> childNeurons = new List<Neuron>();
    public List<Neuron> parentNeurons = new List<Neuron>();
    static int maxChildCount = 2;
    static int minChildCount = 2;

    public Neuron(int _id, int _level, GameObject _neuron)
    {
        id = _id;
        level = _level;
        obj = Object.Instantiate(_neuron);
        Place();
    }

    public float RandomGaussian(float mean, float stdDev)
    {

        float u1 = 1.0f - Random.value;       //uniform(0,1] random doubles
        float u2 = 1.0f - Random.value;
        float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2);//random normal(0,1)
        float randNormal = mean + stdDev * randStdNormal;  //random normal(mean,stdDev^2)
        return randNormal;
    }

    public void ChooseChildNeuron(Neuron[] neurons)
    {
        List<int> candidates = new List<int>();

        float range = maxRadius * maxRadius * 4;

        foreach (Neuron neuron in neurons)
        {

            Vector3 dir = neuron.pos - pos;

            float sqrDist = dir.sqrMagnitude;

            if ( sqrDist < range)
            {
            candidates.Add(neuron.id);
            }

        }

        if (candidates.Count == 0) return;

        int countChoices = maxChildCount - minChildCount + 1;


        int choice = (int)Mathf.Floor(Random.value * countChoices) + minChildCount;

        int[] selected = new int[choice];
        for (int i = 0; i< choice; i++)
        {
           
            selected[i] = Random.Range(0, candidates.Count);
            Neuron childNeuron = neurons[candidates[selected[i]]];
            childNeurons.Add(childNeuron);
            childNeuron.parentNeurons.Add(this);
            candidates.RemoveAt(selected[i]);
            if (candidates.Count == 0) return;
        }

    }



    public void Place()
    {
        float angle = Random.value * Mathf.PI * 2;
        float r = RandomGaussian(20.0f, 10.0f) + maxRadius * (1.0f - Mathf.Pow(Random.value, 7.0f)); 
        float x = Mathf.Cos(angle) * r;
        float z = Mathf.Sin(angle) * r;
        float y = 2.0f*maxheight + maxheight * 0.5f * Mathf.Pow(Random.value, 7.0f) - maxheight * level * 0.5f;
        pos = new Vector3(x, y, z);
        obj.transform.position = pos;
        ResetMood();
    }




    public void ResetMood()
    {
        mood = 1.0f;
        //mood = Mathf.Sin(Mathf.PerlinNoise(pos.x / 10.0f, pos.y / 10.0f) * Mathf.PI * 2);
        //innerRing = Mathf.Pow(5.0f * (1.0f - Mathf.Pow(Random.value, 7.0f)), 2.0f);
        //outerRing = Mathf.Pow(50.0f * (1.0f - Mathf.Pow(Random.value, 7.0f)), 2.0f);   
    }

    public void UpdateMood()
    {
        mood = 1.0f + obj.GetComponent<MeshRenderer>().material.color.r * 10.0f;
    }

}
