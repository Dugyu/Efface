using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonController : MonoBehaviour {
    Dictionary<KeyCode, Vector3> directions;

    void Start () {
        // Turn off Mouse Cursor
        Cursor.lockState = CursorLockMode.Locked;
    
        directions = new Dictionary<KeyCode, Vector3>()
        {
            { KeyCode.W, Vector3.forward},
            { KeyCode.A, Vector3.left},
            { KeyCode.D, Vector3.right},
            { KeyCode.S, Vector3.back},
            { KeyCode.R, Vector3.up },
            { KeyCode.F, Vector3.down}

        };
        
	}
	
	void Update () {
        float speed = 5;
        foreach (KeyCode direction in directions.Keys) {
            if (Input.GetKey(direction)) {
               this.transform.Translate(directions[direction] * speed * Time.deltaTime, Space.Self);
            }
        }

        // Turn on Mouse cursor
        if (Input.GetKeyDown("escape"))
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
