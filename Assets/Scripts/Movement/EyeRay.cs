using Tobii.Gaming;
using UnityEngine;

public class EyeRay : MonoBehaviour
{

    public Camera cam;
    public GameObject player;
    private RaycastHit hit;
    private Ray ray;

    public float targettingSpeed = 0.01f; //0 to 1
    Vector3 instTarget;
    Vector3 smoothTarget;

    public GazePoint _gazePoint;

    void Update()
    {

        _gazePoint = TobiiAPI.GetGazePoint();

        float targetingSpeed = targettingSpeed;

        if (_gazePoint.IsValid) {

            ray = cam.ScreenPointToRay( new Vector3(_gazePoint.Screen.x, _gazePoint.Screen.y));

            if (Physics.Raycast(ray, out hit))
            {
                Transform objectHit = hit.transform;
                Vector3 hitPoint = hit.point;

                if (objectHit.transform.tag == "Neuron")
                {
                    Color existing = objectHit.GetComponent<MeshRenderer>().material.color;
                    Color target = Color.white;
                    Color lerpColor = existing * 0.9f + target * 0.1f;
                    objectHit.GetComponent<MeshRenderer>().material.color = lerpColor;

                    player.transform.Translate(player.transform.forward * 5.0f * Time.deltaTime, Space.Self);

                }


                instTarget = hitPoint;
                smoothTarget = smoothTarget * (1.0f - targetingSpeed) + instTarget * targetingSpeed;
                if (player.transform.forward == Vector3.up)
                {
                    player.transform.LookAt(smoothTarget, Vector3.back);
                }
                else
                {
                    player.transform.LookAt(smoothTarget, Vector3.up);
                }
            }

            else
            {
                // Imagine the hit point falls on an invisible globe 
                Vector3 hitPoint = ray.GetPoint(512.0f);
                instTarget = hitPoint;
                smoothTarget = smoothTarget * (1.0f - targettingSpeed) + instTarget * targettingSpeed;

                if (player.transform.forward == Vector3.up)
                {
                    player.transform.LookAt(smoothTarget, Vector3.back);
                }
                else
                {
                    player.transform.LookAt(smoothTarget, Vector3.up);
                }
            }
        }


       

    }
}
