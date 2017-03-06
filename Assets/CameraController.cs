using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CameraController : MonoBehaviour
{

        private new Camera camera;

        Vector2 lastMousePos;
       

        public float MoveSpeed      = 5.0f;
        public float RotationSpeed  = 90f;

        private Vector3 rotation;
        private Vector3 position;



        private Vector3 translationForce   = Vector3.zero;


        void Start()
        {
            camera          = this.gameObject.GetComponent<Camera>();
            this.rotation = camera.transform.rotation.eulerAngles;
            this.position = camera.transform.position;
        }
       
        

        void Update()
        {
           
            
            
                
            translationForce += ControlWASD();
            translationForce += ControlPan();

                
            translationForce = Vector3.ClampMagnitude(translationForce, 1);
                
            camera.transform.position += (translationForce * MoveSpeed);



            ControlRotation();
                //rotation = rotation.ClampCircular(glm.Radians(85.0), 0, 0);
                //rotation = normalizeAngles(rotation);
                //var x = camera.transform.rotation.eulerAngles;
                //camera.transform.rotation = Quaternion.Euler(rotation);
                
                
            translationForce    /= 1.1f;                      
            if (translationForce.magnitude <= 0.0001) translationForce = Vector3.zero;            
                           
        }



        private float translationForceFactor = 0.1f;

        

        /// <summary>
        /// Returns the acceleration vector from WASD input
        /// </summary>
        private Vector3 ControlWASD()
        {
        // Decrease speed
            var relativeForce = translationForceFactor * Time.deltaTime ;

            Vector3 force = Vector3.zero;

            if (Input.GetKey(KeyCode.W)) force += transform.forward   * relativeForce;           
            if (Input.GetKey(KeyCode.A)) force -= transform.right     * relativeForce;           
            if (Input.GetKey(KeyCode.S)) force -= transform.forward   * relativeForce;            
            if (Input.GetKey(KeyCode.D)) force += transform.right     * relativeForce;

            return force;  
        }


        private Vector3 ControlPan()
        {

            var relativeForce = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * translationForceFactor * 0.1f;
            
            Vector3 force = Vector3.zero;

            if (Input.GetMouseButton(2))
            {
                force += camera.transform.right * relativeForce.x;
                force += -camera.transform.up   * relativeForce.y;
            }
            return force;
        }


        private void ControlRotation()
        {
            if (Input.GetMouseButton(1))
            {
                this.transform.Rotate(Vector3.right, -Input.GetAxis("Mouse Y") * 90 * Time.deltaTime);
                this.transform.Rotate(Vector3.up, Input.GetAxis("Mouse X") * 90 * Time.deltaTime, Space.World);
            }         
        }


        private Vector3 normalizeAngles(Vector3 angles)
        {
            float MAX_VERTICAL_ANGLE = 85 ;


            angles.x = angles.x % 360.0f;
            //fmodf can return negative values, but this will make them all positive
            if (angles.y < 0.0f)
                angles.y += 360.0f;
            if (angles.y > 360.0f)
                angles.y -= 360.0f;

            if (angles.x > MAX_VERTICAL_ANGLE)
                angles.x = MAX_VERTICAL_ANGLE;
            else if (angles.x < -MAX_VERTICAL_ANGLE)
                angles.x = -MAX_VERTICAL_ANGLE;

            angles.z = 0;
            return angles;
        }
    }

