using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PresenceOfMe : MonoBehaviour
{
    //Get Player Position
    public Transform brush;

    public Vector3 lastInput;
    private float baseline;


    // Start is called before the first frame update
    void Start()
    {
        lastInput = Input.mousePosition;

        baseline = Mathf.Sqrt(Screen.width * Screen.width + Screen.height * Screen.height);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 difference = Input.mousePosition - lastInput;

        float waveValue = 1.0f - difference.magnitude / baseline;


        waveValue = Mathf.Pow(waveValue, 10);

        if (waveValue < 0.01f)
        {
            waveValue = 0.01f;
        }


        lastInput = Input.mousePosition;

        brush.localScale = new Vector3(20.0f * Mathf.Sin(Time.realtimeSinceStartup), 10.0f * Mathf.Sin(Time.realtimeSinceStartup), 16.0f * Mathf.Sin(Time.realtimeSinceStartup));
        brush.localScale *= waveValue;
        brush.gameObject.GetComponent<MeshRenderer>().material.color = new Color(Mathf.Sin(Time.realtimeSinceStartup) * 0.72f, 0.267f, 0.267f);
    }
}
