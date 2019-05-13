using System.Collections.Generic;
using UnityEngine;

public class Avatar
{
    // shared property

    static float range = 800.0f;
    static float boundary = 1024.0f;
    // self
    public GameObject obj;
    public Vector3 pos = Vector3.zero;
    public Vector3 vel = Vector3.zero;
    public Vector3 acc = Vector3.zero;
    public int id;
    public int type; // 0 = Quiet, 1 = Moving, 2 =  Noisy

    // wave
    public LinkedList<float> fHz = new LinkedList<float>();
    public LinkedList<float> ampl = new LinkedList<float>();
    public int maxWaveCount;


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


    // initialize
    public Avatar(GameObject _avatar)
    {

        id = getIndex();
        obj = Object.Instantiate(_avatar);
        Reset();
    }

    public void Release()
    {
        releaseIndex(id);
        Object.Destroy(obj);
        obj = null;
    }

    public void Reset()
    {

        // Place

        float angle = Random.value * Mathf.PI * 2;

        float r = RandomGaussian(100.0f, 50.0f) + range * 0.5f * (1.0f - Mathf.Pow(Random.value, 3.0f));
        float x = Mathf.Cos(angle) * r;
        float z = Mathf.Sin(angle) * r;

        pos = new Vector3(x, 100.0f, z);

        //pos = new Vector3(Random.value * range - 512.0f, 100.0f, Random.value * range - 512.0f);


        // Choose Type
        float randomType = Random.value;

        if ( randomType < 0.40f)
        {
            type = 0;
        }
        else if (randomType > 0.750f)
        {
            type = 1;
        }
        else
        {
            type = 2;
        }


        // Set maxWaveCount
        maxWaveCount = Random.Range(3, 8);
        AddSinWaves(maxWaveCount);
    }





    public void Update()
    {
        if (Mathf.Abs(pos.x) > boundary * 0.5f || Mathf.Abs(pos.z) > boundary * 0.5f)
        {
            Reset();
        }
        switch (type)
        {
            case 0:
                Quiet();
                break;
            case 1:
                Moving();
                break;
            default:
                Noisy();
                break;
        }
        Move();
        Spin();
    }


    public void Spin()
    {

        float waveValue = 0.0f;

        int j = 0;
        using (var e1 = fHz.GetEnumerator())
        using (var e2 = ampl.GetEnumerator())
        {
            while (e1.MoveNext() && e2.MoveNext())
            {
                var f = e1.Current;
                var a = e2.Current;
                waveValue += Mathf.Sin(Time.realtimeSinceStartup * f) * a;
                j++;
            }
        }


        obj.transform.localScale = new Vector3(20.0f * Mathf.Sin(Time.realtimeSinceStartup), 10.0f * Mathf.Sin(Time.realtimeSinceStartup), 16.0f * Mathf.Sin(Time.realtimeSinceStartup));
        obj.transform.localScale *= waveValue;

        obj.GetComponent<MeshRenderer>().material.color = new Color(Mathf.Sin(Time.realtimeSinceStartup) *  0.72f, 0.267f, 0.267f);
    }

    public void AddSinWaves(int n)
    {
        for (int i = 0; i < n; ++i)
        {
            while (fHz.Count > maxWaveCount - 1)
            {
                fHz.RemoveFirst();
                ampl.RemoveFirst();
            }
            ampl.AddLast(Random.Range(0.1f, 1.0f));
            fHz.AddLast(Random.Range(0.1f, 1.0f));

        }
    }


    // type 0
    public void Quiet()
    {
        vel = Vector3.zero;
        acc = Vector3.zero;

    }

    // type 1
    public void Moving()
    {
        if (acc == Vector3.zero)
        {
            acc = new Vector3(Random.Range(0.0f,1.0f) -0.5f, 0.0f, Random.Range(0.0f,1.0f) -0.5f);
        };

    }
    // type 2
    public void Noisy()
    {
        acc = new Vector3(Random.Range(0.0f, 1.0f) -0.5f, 0.0f, Random.Range(0.0f, 1.0f) -0.5f);

    }


    public void Move()
    {

        vel *= 0.8f;
        vel += acc;
        pos += vel * 0.05f;
        obj.transform.position = pos;
    }

    
    public float RandomGaussian(float mean, float stdDev)
    {
        float u1 = 1.0f - Random.value;       //uniform(0,1] random doubles
        float u2 = 1.0f - Random.value;
        float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2);//random normal(0,1)
        float randNormal = mean + stdDev * randStdNormal;  //random normal(mean,stdDev^2)
        return randNormal;
    }
}
