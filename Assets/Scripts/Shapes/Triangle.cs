using UnityEngine;

public class Triangle : MonoBehaviour
{
    private Mesh mesh;
    private Vector3[] verts;
    private Vector2[] uvs;
    private int[] tris;
    // Start is called before the first frame update
    void Start()
    {
        verts = new Vector3[3];
        tris = new int[3];
        uvs = new Vector2[3];
        mesh = new Mesh();
        GetComponent <MeshFilter>().mesh = mesh;

        verts[0] = new Vector3(0, 0, 0);
        verts[1] = new Vector3(1, 0, 0);
        verts[2] = new Vector3(0, 1, 0);

        uvs[0] = new Vector2(0, 0);
        uvs[1] = new Vector2(0.5f, 1.0f);
        uvs[2] = new Vector2(1.0f, 0.0f);
        tris[0] = 0;
        tris[1] = 1;
        tris[2] = 2;


        mesh.vertices = verts;
        mesh.uv = uvs;
        mesh.triangles = tris;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

    
}
