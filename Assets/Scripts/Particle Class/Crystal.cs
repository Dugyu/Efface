using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal 
{
    public GameObject obj;
    public Mesh mesh;


    public LinkedList<Vector3> pts;
    public LinkedList<Vector2> ptuvs;
    public LinkedList<int> pttris;
    static int maxVerts = 600;


    public Crystal(GameObject _empty)
    {
        obj = Object.Instantiate(_empty);
        mesh = new Mesh();
        obj.GetComponent<MeshFilter>().mesh = mesh;
    }


    public void UpdateMesh(Vector3[] newVerts)
    {
        pts.AddLast(newVerts[0]);
        pts.AddLast(newVerts[3]);
        pts.AddLast(newVerts[4]);
        pts.AddLast(newVerts[1]);
        pts.AddLast(newVerts[2]);
        pts.AddLast(newVerts[5]);

        // uv and triangles can keep the same
        if (pts.Count> maxVerts)
        {
            for (int i = 0; i < 6; ++i) { pts.RemoveFirst();}

            mesh = obj.GetComponent<MeshFilter>().mesh;
            Vector3[] verts = mesh.vertices;

            int j = 0;
            foreach (Vector3 pt in pts)
            {
                verts[j++] = pt;
            }

            mesh.vertices = verts;
            mesh.RecalculateBounds();
        }

        // uv and triangle also need to add more items
        else
        {
            ptuvs.AddLast( new Vector2(0, 0));
            ptuvs.AddLast(new Vector2(0.5f, 1.0f));
            ptuvs.AddLast(new Vector2(1.0f, 0.0f));
            ptuvs.AddLast(new Vector2(0, 0));
            ptuvs.AddLast(new Vector2(0.5f, 1.0f));
            ptuvs.AddLast(new Vector2(1.0f, 0.0f));

            mesh = obj.GetComponent<MeshFilter>().mesh;

            Vector3[] verts = new Vector3[pts.Count];
            Vector2[] uvs = new Vector2[pts.Count];
            int[] tris = new int[pts.Count];

            int j = 0;
            using (var e1 = pts.GetEnumerator())
            using (var e2 = ptuvs.GetEnumerator())
            using (var e3 = pttris.GetEnumerator())
            {
                while (e1.MoveNext() && e2.MoveNext() && e3.MoveNext())
                {
                    var pt = e1.Current;
                    var uv = e2.Current;
                    var tri = e3.Current;

                    verts[j] = pt;
                    uvs[j] = uv;
                    tris[j] = tri;

                    j++;
                }
            }
            mesh.vertices = verts;
            mesh.uv = uvs;
            mesh.triangles = tris;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
        }

    }


}
