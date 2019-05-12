using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swamp : MonoBehaviour
{

    //serialize for terrain
    private Mesh mesh;
    public int mDivision;
    public float mSize;
    public float mHeight;
    private static int vertCount;
    private int divPlusOne;
    private Vector3[] verts;
    private Vector2[] uvs;
    private int[] tris;

    private float halfSize;
    private float divisionSize;

    // collider
    public MeshCollider meshc;

   



    void Start()
    {
        //initialize terrain
        divPlusOne = mDivision + 1;
        vertCount = divPlusOne * divPlusOne;

        halfSize = mSize * 0.5f;
        divisionSize = mSize / mDivision;

        verts = new Vector3[vertCount];
        uvs = new Vector2[vertCount];
        tris = new int[mDivision * mDivision * 6];

        CreateTerrain();

        meshc = gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
        meshc.sharedMesh = mesh;

    }

    void CreateTerrain()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        int triOffset = 0;

        for (int i = 0; i <= mDivision; ++i)
        {
            for (int j = 0; j <= mDivision; ++j)
            {

                float xCoord = (float)j / mDivision;
                float yCoord = (float)i / mDivision;
                float height = 0.5f* Mathf.PerlinNoise(xCoord * 4, yCoord * 4) + 0.25f * Mathf.PerlinNoise(xCoord * 8 + 0.5f * Time.timeSinceLevelLoad, yCoord * 8 + 0.5f * Time.timeSinceLevelLoad) ;
                height = Mathf.Pow(height, 10);
                height *= mHeight * 2;


                verts[i * (divPlusOne) + j] = new Vector3(-halfSize + j * divisionSize, height, halfSize - i * divisionSize);
                uvs[i * (divPlusOne) + j] = new Vector2((float)j / mDivision, (float)i / mDivision);

                if (i < mDivision && j < mDivision)
                {
                    int topLeft = i * divPlusOne + j;
                    int botLeft = (i + 1) * divPlusOne + j;

                    tris[triOffset] = topLeft;
                    tris[triOffset + 1] = topLeft + 1;
                    tris[triOffset + 2] = botLeft;

                    tris[triOffset + 3] = topLeft + 1;
                    tris[triOffset + 4] = botLeft + 1;
                    tris[triOffset + 5] = botLeft;

                    triOffset += 6;
                }
            }

        }

        mesh.vertices = verts;
        mesh.uv = uvs;
        mesh.triangles = tris;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }


    void UpdateTerrain()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        verts = mesh.vertices;

        // update vertices position
        for (int i = 0; i <= mDivision; ++i)
        {
            for (int j = 0; j <= mDivision; ++j)
            {

                float xCoord = (float)j / mDivision;
                float yCoord = (float)i / mDivision;

                float height = 0.5f * Mathf.PerlinNoise(xCoord * 4, yCoord * 4) + 0.25f * Mathf.PerlinNoise(xCoord * 8 + 0.5f * Time.timeSinceLevelLoad, yCoord * 8 + 0.5f * Time.timeSinceLevelLoad);
                height = Mathf.Pow(height, 10);
                height *= mHeight * 2;
                verts[i * (divPlusOne) + j] = new Vector3(-halfSize + j * divisionSize, height, halfSize - i * divisionSize);
            }
        }
        // assign back those verts
        mesh.vertices = verts;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

    void Update()
    {
        UpdateTerrain();
    }
}
