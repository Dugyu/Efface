using UnityEngine;
using Tobii.Gaming;
public class FirstPersonEye : MonoBehaviour
{
    float speed = 5.0f;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;

    }

    // Update is called once per frame
    void Update()
    {
        HeadPose headPose = TobiiAPI.GetHeadPose();


        Vector3 direction = Vector3.zero;
        if (headPose.IsRecent())
        {
            print("HeadPose Position (X,Y,Z): " + headPose.Position.x + ", " + headPose.Position.y + ", " + headPose.Position.z);

            if(headPose.Position.y < 50.0f)
            {
                direction = Vector3.down;
            }
            if(headPose.Position.y > 150.0f)
            {
                direction = Vector3.up;
            }
            if (headPose.Position.x < -50.0f)
            {
                direction = Vector3.left;
            }
            if (headPose.Position.x > 50.0f)
            {
                direction = Vector3.right;
            }
            if (headPose.Position.z >600.0f)
            {
                direction = Vector3.back;
            }
            if (headPose.Position.z < 450.0f)
            {
                direction = Vector3.forward;
            }

            if (direction != Vector3.zero)
            {
                transform.Translate(direction * speed * Time.deltaTime, Space.Self);
            }
        }

        // Exit Application
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
    }
}
