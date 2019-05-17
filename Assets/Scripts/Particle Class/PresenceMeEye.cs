using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.Gaming;

public class PresenceMeEye : MonoBehaviour
{
    //Get Player Position
    public Transform brush;

    public Vector3 lastInput;
    private float baseline;

    private GazePoint _gazePoint;
    

    // Start is called before the first frame update
    void Start()
    {

        _gazePoint = TobiiAPI.GetGazePoint();

        lastInput = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f);

        baseline = Mathf.Sqrt(Screen.width * Screen.width + Screen.height * Screen.height);
    }

    // Update is called once per frame
    void Update()
    {
        float waveValue = 1.0f;

        if (_gazePoint.IsValid)
        {
            
            Vector3 difference = new Vector3(_gazePoint.Screen.x,_gazePoint.Screen.y) - lastInput;
            waveValue = 1.0f - difference.magnitude/baseline;
            waveValue = Mathf.Pow(waveValue, 10);
        }

        //Debug.Log(brush.transform.localScale.magnitude);


        lastInput = new Vector3(_gazePoint.Screen.x, _gazePoint.Screen.y);


        brush.localScale = new Vector3(20.0f * Mathf.Sin(Time.realtimeSinceStartup), 10.0f * Mathf.Sin(Time.realtimeSinceStartup), 16.0f * Mathf.Sin(Time.realtimeSinceStartup));
        brush.localScale *= waveValue;
        brush.gameObject.GetComponent<MeshRenderer>().material.color = new Color(Mathf.Sin(Time.realtimeSinceStartup) * 0.72f, 0.267f, 0.267f);
    }
}
