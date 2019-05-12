using System.Collections;
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
        pos = new Vector3(Random.value * range - 512.0f, 100.0f, Random.value * range - 512.0f);


        //Choose Type
        float randomType = Random.value;

        if ( randomType < 0.333f)
        {
            type = 0;
        }
        else if (randomType > 0.667f)
        {
            type = 1;
        }
        else
        {
            type = 2;
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
            acc = new Vector3(Random.value, 0.0f, Random.value);
        };

    }
    // type 2
    public void Noisy()
    {
        acc = new Vector3(Random.value, 0.0f, Random.value);

    }


    public void Move()
    {

        vel *= 0.8f;
        vel += acc;
        pos += vel * 0.08f;
        obj.transform.position = pos;
    }


    public void Update()
    {
        if ( Mathf.Abs(pos.x) > boundary * 0.5f || Mathf.Abs(pos.z) > boundary * 0.5f)
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
    }
}
