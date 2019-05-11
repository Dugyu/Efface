using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    ParticleSystem particleSystem;

    public Vector3 bounds = new Vector3(1024.0f, 1.0f, 1024.0f);
    public Vector3Int resolution = new Vector3Int(100, 1, 100);

    void OnEnable()
    {
        particleSystem = GetComponent<ParticleSystem>();

        Vector3 scale;
        Vector3 boundsHalf = bounds / 2.0f;

        scale.x = bounds.x / resolution.x;
        scale.y = bounds.y / resolution.y;
        scale.z = bounds.z / resolution.z;

        ParticleSystem.EmitParams ep = new ParticleSystem.EmitParams();

        for (int i = 0; i < resolution.x; i++)
        {
            for (int j = 0; j < resolution.y; j++)
            {
                for (int k = 0; k < resolution.z; k++)
                {
                    Vector3 position;

                    position.x = (i * scale.x) - boundsHalf.x;
                    position.y = (j * scale.y) - boundsHalf.y;
                    position.z = (k * scale.z) - boundsHalf.z;

                    ep.position = position;
                    particleSystem.Emit(ep, 1);
                }
            }
        }
    }
}
