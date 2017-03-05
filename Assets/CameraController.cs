using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CameraController : MonoBehaviour
{

        private Camera camera;

        Vector2 lastMousePos;
       

        public float MoveSpeed      = 10.0f;
        public float RotationSpeed  = 0.01f;

        private Vector3 rotation;
        private Vector3 position;



        private Vector3 translationForce   = Vector3.zero;
        private Vector3 rotationForce      = Vector3.zero;


        void Start()
        {
            camera          = this.gameObject.GetComponent<Camera>();
            this.rotation = camera.transform.rotation.eulerAngles;
            this.position = camera.transform.position;
        }
       
        

        void Update()
        {
           
            
            var mousePosX   = new Vector2(Input.mousePosition.x / (float)Screen.width, Input.mousePosition.y / (float)Screen.height);

                var delta = mousePosX - lastMousePos;
            
                translationForce += ControlWASD();
                translationForce += ControlPan(delta);

                
                translationForce = Vector3.ClampMagnitude(translationForce, 1);
                
                camera.transform.position += (translationForce * MoveSpeed);



                rotation += ControlRotation(delta) * RotationSpeed;
                //rotation = rotation.ClampCircular(glm.Radians(85.0), 0, 0);
                rotation = normalizeAngles(rotation);
                //var x = camera.transform.rotation.eulerAngles;
                camera.transform.rotation = Quaternion.Euler(rotation);
                
                rotationForce       /= 1.1f;
                translationForce    /= 1.1f;

                if (rotationForce.magnitude <= 0.0001) rotationForce = Vector3.zero;               
                if (translationForce.magnitude <= 0.0001) translationForce = Vector3.zero;
            

            
            lastMousePos = mousePosX;                    
        }



        private float translationForceFactor = 0.1f;
        private float rotationForceFactor = 1.0f;
        

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


        private Vector3 ControlPan(Vector2 deltaPos)
        {

            var relativeForce = translationForceFactor * deltaPos;
            
            Vector3 force = Vector3.zero;

            if (Input.GetMouseButton(2))
            {
                force += camera.transform.right * relativeForce.x;
                force += -camera.transform.up   * relativeForce.y;
            }
            return force;
        }


        private Vector3 ControlRotation(Vector2 deltaPos)
        {

            var x  = Input.GetAxis("Mouse X");
            var y = Input.GetAxis("Mouse Y");
            //var relativeForce =  * deltaPos;

            Vector3 force = Vector3.zero;
            if (Input.GetMouseButton(1))
            {
                Debug.Log("asdasd");
                //force.x += x * rotationForceFactor;
                force.y += y * rotationForceFactor;
                force.z += 0;         
            }

            return force;
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

