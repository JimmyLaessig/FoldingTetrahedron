using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour {



	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
        if (Input.GetMouseButton(1))
        {

            //var mouseVertical = Input.GetAxis("Mouse X");
            //var mouseHorizontal = Input.GetAxis("Mouse Y");


            //this.transform.Rotate(Vector3.up, -3.0F * mouseVertical);
            //this.transform.Rotate(Vector3.right, 3.0F * mouseHorizontal,Space.World);
        }
        if (Input.GetMouseButton(2))
        {
           // Camera.main.transform.Translate(Camera.main.transform.right * Input.GetAxis("Mouse X") * 0.1f);
           // Camera.main.transform.Translate(Camera.main.transform.up * Input.GetAxis("Mouse Y") * 0.1f);
        }
        //Camera.main.transform.Translate(Camera.main.transform.forward * Input.mouseScrollDelta.y * 0.1f);
    }
}
