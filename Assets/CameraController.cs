using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CameraController : MonoBehaviour
{

    private new Camera camera;
    private Vector2 lastPos;

    public float MoveSpeed = 5.0f;
    public float RotationSpeed = 90.0f;


    private Vector3 translationForce = Vector3.zero;
    private float translationForceFactor = 0.1f;

    /// <summary>
    /// Initializes all parameters for the camera controller 
    /// (Called by Unity internally)
    /// </summary>
    void Start()
    {
        camera = this.gameObject.GetComponent<Camera>();
        lastPos = new Vector2(Input.mousePosition.x / (float)Screen.width, Input.mousePosition.y / (float)Screen.height);
    }


    /// <summary>
    /// Updates the camera controller
    /// (Called by Unity internally)
    /// </summary>
    void Update()
    {
        Vector2 currentPos = new Vector2(Input.mousePosition.x / (float)Screen.width, Input.mousePosition.y / (float)Screen.height);
        Vector2 deltaPos = currentPos - lastPos;
        lastPos = currentPos;
        translationForce += ControlWASD();
        translationForce += ControlPan(deltaPos);


        translationForce = Vector3.ClampMagnitude(translationForce, 1);

        camera.transform.position += (translationForce * MoveSpeed);

        ControlRotation(deltaPos);


        translationForce /= (1 + 10 * Time.deltaTime);
        if (translationForce.magnitude <= 0.0001) translationForce = Vector3.zero;

    }


    /// <summary>
    /// Returns the acceleration vector from WASD input
    /// </summary>
    private Vector3 ControlWASD()
    {
        // Decrease speed
        var relativeForce = translationForceFactor * Time.deltaTime;

        Vector3 force = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) force += transform.forward * relativeForce;
        if (Input.GetKey(KeyCode.A)) force -= transform.right * relativeForce;
        if (Input.GetKey(KeyCode.S)) force -= transform.forward * relativeForce;
        if (Input.GetKey(KeyCode.D)) force += transform.right * relativeForce;

        return force;
    }

    /// <summary>
    /// Returns the acceleration vector from Mouse input
    /// </summary>
    private Vector3 ControlPan(Vector2 deltaPos)
    {

        var relativeForce = deltaPos * translationForceFactor;

        Vector3 force = Vector3.zero;

        if (Input.GetMouseButton(2))
        {
            force += camera.transform.right * relativeForce.x;
            force += -camera.transform.up * relativeForce.y;
        }
        return force;
    }

    /// <summary>
    /// Rotates the camera
    /// </summary>
    private void ControlRotation(Vector2 deltaPos)
    {
        if (Input.GetMouseButton(1))
        {

            this.transform.Rotate(Vector3.right, -deltaPos.y * RotationSpeed);
            this.transform.Rotate(Vector3.up, deltaPos.x * RotationSpeed, Space.World);
        }
    }
}

