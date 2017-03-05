using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class ToggleGroup : MonoBehaviour {

    [Serializable]
    public class ToggleGroupEvent : UnityEvent<Toggle>{}

    [SerializeField]
    private List<Toggle> toggles = new List<Toggle>();
    [SerializeField]
    private Toggle currentOption;

    [SerializeField]
    public ToggleGroupEvent onToggleChanged;


    // Use this for initialization
    void Start()
    {
        toggles.ForEach(t => t.isOn = false);
        
        if (currentOption == null)
            currentOption = toggles[0];

        currentOption.isOn = true;
        currentOption.interactable = false;
       // currentOption.onValueChanged.
    }


    void Update()
    {      
        var newOption = toggles.Find(t => t.isOn && t.interactable);

        if (newOption)
        {
            // Reset old state
            currentOption.isOn = false;
            currentOption.interactable = true;

            // Update new state            
            currentOption = newOption;
            currentOption.interactable = false;

            onToggleChanged.Invoke(currentOption);
        }
    }
}


