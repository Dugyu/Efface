using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticTexture : MonoBehaviour
{
    private static int texSize = 2048;
    Color[] texColor = new Color[texSize * texSize];
    private Texture2D texture;

    public Color colorA;
    public Color colorB;

    public GameObject Crystal;

    void Start()
    {
        // initialize
        texture = new Texture2D(texSize, texSize);
        GetComponent<Renderer>().material.mainTexture = texture;
        Crystal.GetComponent<Renderer>().material.mainTexture = texture;
        float noiseScale = 0.003f;
        // initial color
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                float t = Mathf.PerlinNoise(x * noiseScale, y * noiseScale);
                float r = Mathf.Lerp(colorA.r, colorB.r, t);
                float g = Mathf.Lerp(colorA.g, colorB.g, t);
                float b = Mathf.Lerp(colorA.b, colorB.b, t);
                Color colorC = new Color(r, g, b);
                int index = (texture.height - y - 1) * texture.width + x;
                texColor[index] = colorC; 
            }
        }

        texture.SetPixels(texColor);
        texture.Apply();
    }

}
