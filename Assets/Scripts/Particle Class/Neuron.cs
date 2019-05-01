using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neuron 
{
    public float maxRadius = 200.0f;
    public float maxheight = 20.0f;
    public GameObject obj;
    public Vector3 pos = Vector3.zero;
    public float mood;
    public float innerRing; 
    public float outerRing;
    public int id;
    public int level;
    public int levelRange = 3;

    public Neuron(int _id, GameObject _neuron)
    {
        id = _id;
        obj = Object.Instantiate(_neuron);
    }

    public float RandomGaussian(float mean, float stdDev)
    {
        Random.InitState(42);
        float u1 = 1.0f - Random.value;       //uniform(0,1] random doubles
        float u2 = 1.0f - Random.value;
        float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2);//random normal(0,1)
        float randNormal = mean + stdDev * randStdNormal;  //random normal(mean,stdDev^2)
        return randNormal;
    }

    public void AssignLevel()
    {
        level = (int)Mathf.Ceil(Random.value * levelRange);
    }


    public void Place()
    {
        float angle = Random.value * Mathf.PI * 2;
        float r = RandomGaussian(20.0f, 5.0f) + maxRadius * (1.0f - Mathf.Pow(Random.value, 7.0f));
        float x = Mathf.Cos(angle) * r;
        float z = Mathf.Sin(angle) * r;
        float y = maxheight + maxheight * 0.5f * Mathf.Pow(Random.value, 7.0f);

        pos = new Vector3(x, y, z);
    }


    public void Mood()
    {
        mood = Mathf.Sin(Mathf.PerlinNoise(pos.x / 10.0f, pos.y / 10.0f) * Mathf.PI * 2);
        innerRing = Mathf.Pow(5.0f * (1.0f - Mathf.Pow(Random.value, 7.0f)), 2.0f);
        outerRing = Mathf.Pow(10.0f * (1.0f - Mathf.Pow(Random.value, 7.0f)), 2.0f);
    }
}
