using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;

public class OutlineManager : MonoBehaviour
{
    SearchExperimentLevel searchExperimentLevel;
    [SerializeField] private InputActionReference rightGrabPressed;
    // Start is called before the first frame update
    void Start()
    {
        searchExperimentLevel = FindObjectOfType<SearchExperimentLevel>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rightGrabPressed)
        {
            //if (searchExperimentLevel.selectStatusDict[hit.transform.gameObject.GetComponent<Interactable>()] == false)
            //{
            //    searchExperimentLevel.selectStatusDict[hit.transform.gameObject.GetComponent<Interactable>()] = true;
            //    if (hit.transform.gameObject.GetComponent<Outline>() == null)
            //    {
            //        Outline outline = hit.transform.gameObject.AddComponent<Outline>();
            //        outline.OutlineColor = Color.green;
            //        outline.OutlineWidth = 10f;
            //        outline.OutlineMode = Outline.Mode.OutlineVisible;
            //    }
            //    else
            //    {
            //        hit.transform.gameObject.GetComponent<Outline>().OutlineColor = Color.green;
            //    }
            //}
            //else
            //{
            //    searchExperimentLevel.selectStatusDict[hit.transform.gameObject.GetComponent<Interactable>()] = false;
            //    if (hit.transform.gameObject.GetComponent<Outline>() == null)
            //    {
            //        Outline outline = hit.transform.gameObject.AddComponent<Outline>();
            //        outline.OutlineColor = Color.red;
            //        outline.OutlineWidth = 10f;
            //        outline.OutlineMode = Outline.Mode.OutlineVisible;
            //    }
            //    else
            //    {
            //        hit.transform.gameObject.GetComponent<Outline>().OutlineColor = Color.red;
            //    }
            //}
        }
        
    }
}
