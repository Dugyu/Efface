using System.Collections;
using UnityEngine;

public class MouseRay : MonoBehaviour
{
    public Camera cam;
    private RaycastHit hit;
    private Ray ray;
    void Start()
    {

    }

    void Update()
    {
        ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            Transform objectHit = hit.transform;
            Debug.Log(hit.point.x);
            
        }
    }
}
