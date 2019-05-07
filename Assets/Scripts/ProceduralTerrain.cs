using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ProceduralTerrain : MonoBehaviour
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
    //private MeshCollider meshc;
    // Start is called before the first frame update

    //serial for texture
    private Texture2D texture;
    private static int texSize = 1024;
    private Color[] texColor = new Color[texSize * texSize];
    private Renderer m_Renderer;
    private int[] rowTable;
    private int[] colTable;
    private float pixelSize;


    public Color colorA;

    //Get Player Position
    public Transform player;

 

    void Start()
    {
        //initialize terrain
        divPlusOne = mDivision + 1;
        vertCount = divPlusOne * divPlusOne;

        halfSize = mSize * 0.5f;
        divisionSize =  mSize / mDivision;

        verts = new Vector3[vertCount];
        uvs = new Vector2[vertCount];
        tris = new int[mDivision * mDivision * 6];

        CreateTerrain();
        //meshc = gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
        //meshc.sharedMesh = mesh;


 

        //initial texture
        texture = new Texture2D(texSize, texSize);
        Color[] texColor = new Color[texSize * texSize];
        pixelSize = mSize / texSize;

        //fetch renderer
        m_Renderer = GetComponent<Renderer>();
        m_Renderer.material.SetTexture("_SecondTex",texture);

        SetBlack();
    }

    void FillLookTable()
    {
        rowTable = new int[texSize * texSize];
        colTable = new int[texSize * texSize];

        for (int y = 0; y< texSize; y++)
        {
            for (int x = 0; x< texSize; x++)
            {
                int id = (texSize - y - 1) * texSize + x;
                colTable[id] = x;
                rowTable[id] = y;
            }
        }
    }

    void SetBlack()
    {
        for (int i=0; i< texColor.Length; i++)
        {
            texColor[i] = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        }
    }

    //void SetOriginalColor()
    //{
    //    //float noiseScale = 0.003f;
    //    //// initial color
    //    //for (int y = 0; y < texture.height; y++)
    //    //{
    //    //    for (int x = 0; x < texture.width; x++)
    //    //    {
    //    //        float t = Mathf.PerlinNoise(x * noiseScale, y * noiseScale);
    //    //        float r = Mathf.Lerp(colorA.r, colorB.r, t);
    //    //        float g = Mathf.Lerp(colorA.g, colorB.g, t);
    //    //        float b = Mathf.Lerp(colorA.b, colorB.b, t);
    //    //        Color colorC = new Color(r, g, b);
    //    //        int index = (texture.height - y - 1) * texture.width + x;
    //    //        texColor[index] = colorC;
    //    //    }
    //    //}

    //    //texture.SetPixels(texColor);
    //    //texture.Apply();
    //}


    void AddColorTrail()
    {
        colorA.r += Mathf.Sin(Time.deltaTime) * Random.value;

        int xPlayer = (int)Mathf.Floor((player.position.x + halfSize) / pixelSize);
        int yPlayer = (int)Mathf.Floor((player.position.z + halfSize) / pixelSize);

        for (int y = 0; y < texture.height; y++)
        {
            if (Mathf.Abs(y - yPlayer) > 20)
            {
                continue;
            }

            for (int x = 0; x < texture.width; x++)
            {
                if (Mathf.Abs(x - xPlayer) > 20)
                {
                    continue;
                }

                Vector3 worldPos = new Vector3(-halfSize + x * pixelSize, 0.0f, -halfSize + y * pixelSize);

                float dist = (worldPos.x - player.position.x) * (worldPos.x - player.position.x) + (worldPos.z - player.position.z) * (worldPos.z - player.position.z);
                float changeValue = Mathf.Pow(2, -0.1f * dist);
                int index = y * texture.width + x;

                Color original = texColor[index];
                float r = Mathf.Lerp(original.r, colorA.r, changeValue);
                float g = Mathf.Lerp(original.g,colorA.g,changeValue);
                float b = Mathf.Lerp(original.b, colorA.b, changeValue);
                Color changeColor = new Color(r, g, b,1.0f);
                texColor[index] = changeColor;
            }
        }

        texture.SetPixels(texColor);
        texture.Apply();
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
                float height = Mathf.PerlinNoise(xCoord * 4, yCoord * 4) + 0.5f * Mathf.PerlinNoise(xCoord * 8 + 0.5f * Time.timeSinceLevelLoad, yCoord * 8 + 0.5f * Time.timeSinceLevelLoad) + 0.25f * Mathf.PerlinNoise(xCoord * 16, yCoord * 16); // + 0.25f * Mathf.PerlinNoise(xCoord * 32, yCoord * 32) ;
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
                float height = Mathf.PerlinNoise(xCoord * 4, yCoord * 4) + 0.5f * Mathf.PerlinNoise(xCoord * 8 + 0.5f * Time.timeSinceLevelLoad, yCoord * 8 + 0.5f * Time.timeSinceLevelLoad) + 0.25f * Mathf.PerlinNoise(xCoord * 16, yCoord * 16); // + 0.25f * Mathf.PerlinNoise(xCoord * 32, yCoord * 32) ;
                height = Mathf.Pow(height, 10);
                height *= mHeight * 2;
                verts[i * (divPlusOne) + j] = new Vector3(-halfSize + j * divisionSize, height, halfSize - i * divisionSize);
            }
        }
        // assign back those verts
        mesh.vertices = verts;
        mesh.RecalculateBounds();

    }


    // Update is called once per frame
    void Update()
    {
        UpdateTerrain();
        AddColorTrail();
    }
}
