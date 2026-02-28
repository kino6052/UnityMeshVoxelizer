using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target; // the object to look at
    public float distance = 5.0f; // distance from the target
    public float speed = 5.0f; // speed of rotation

    private float angle = 0.0f; // current angle

    void Update()
    {
        // check for left and right arrow keys
        float input = Input.GetAxis("Horizontal");
        if (input != 0)
        {
            // update the angle based on input and speed
            angle += input * speed * Time.deltaTime;
        }

        // calculate the position based on the target and angle
        Vector3 position = target.position + Quaternion.Euler(0, angle, 0) * new Vector3(0, 0, -distance);

        // set the camera position and rotation
        transform.position = position;
        transform.LookAt(target);
    }
}
