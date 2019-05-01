using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Memo
{
    public float maxRadius = 200.0f;
    public float maxheight = 200.0f;

    public GameObject obj;
    public Vector3 pos = Vector3.zero;
    public Vector3 vel = Vector3.zero;
    public Vector3 acc = Vector3.zero;
    public float m = 1.0f;
    public int age;
    public float mood;
    public int id;

    public Memo(int _id, GameObject _memo)
    {
        id = _id;
        obj = Object.Instantiate(_memo);
        Reset();
        
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
    public void Reset()
    {
        float angle = Random.value * Mathf.PI * 2;
        float r = RandomGaussian(5.0f, 2.0f) + maxRadius * (1.0f - Mathf.Pow(Random.value, 7.0f));
        float x = Mathf.Cos(angle) * r;
        float z = Mathf.Sin(angle) * r;
        float y = maxheight + maxheight * 0.5f * Mathf.Pow(Random.value, 7.0f);

        pos = new Vector3(x, y, z);
        age = (int)Mathf.Ceil(Random.Range(5000f, 10000f));
        acc = new Vector3(0.0f, -1.0f, 0.0f);
        Mood();
    }

    public void Mood()
    {
        mood = Mathf.Sin(Mathf.PerlinNoise(pos.x / 10.0f, pos.y / 10.0f) * Mathf.PI * 2);
    }

   
    public float CompareMood(Neuron neuron)
    {
        return 1.0f - Mathf.Abs(neuron.mood - mood);
    }

    public void Attract(Neuron[] neurons)
    {
        acc = Vector3.zero;

        foreach (Neuron neuron in neurons)
        {
            Vector3 dir = neuron.pos - pos;
            float sqrDist = dir.sqrMagnitude;
            dir.Normalize();
            float moodSimilarity = CompareMood(neuron);

            if (sqrDist < neuron.outerRing && sqrDist > neuron.innerRing)
            {
                acc = acc + moodSimilarity * dir;
            }

              
        }

    }


    public void Update()
    {
        if (age-- < 0 || pos.y < 0)
        {
            Reset();
        }
        else
        {
            Move();
        }
    }


    
    public void Move()
    {
        ApplyGravity();
        vel += acc;
        pos += vel;
        obj.transform.position = pos;
    }


    public void ApplyGravity()
    {
        acc = new Vector3(acc.x, acc.y - 1.0f, acc.z);
    }

}
