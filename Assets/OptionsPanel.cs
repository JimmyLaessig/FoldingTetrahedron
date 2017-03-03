using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsPanel : MonoBehaviour {

    [SerializeField]
    private Text fpsText;

    [SerializeField]
    private Text subdivisionText;

    private SierpinskiTetrahedron tetrahedron;
    
    
    // Use this for initialization
    void Start ()
    {
        tetrahedron     = GameObject.Find("SierpinskiTetrahedron").GetComponent<SierpinskiTetrahedron>();
        fpsText         = GameObject.Find("LabelFPS").GetComponent<Text>();
        subdivisionText = GameObject.Find("SubdivisionsText").GetComponent<Text>();
    }

    float updateTimeStep = 1.0f;


    float counter = 1.0f;

	// Update is called once per frame
	void Update ()
    {
        if (counter >= updateTimeStep)
        {

            var fps = ((int)(10f / Time.deltaTime)) / 10f;

            fpsText.text = fps + " FPS";
            counter -= updateTimeStep;
        }
        counter += Time.smoothDeltaTime;

        subdivisionText.text = "" + tetrahedron.NumSubDivisions;
    }
}
