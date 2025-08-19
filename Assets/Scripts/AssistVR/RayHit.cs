using cakeslice;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class RayHit : MonoBehaviour
{
    [SerializeField] private InputActionReference clickedObject_Right;
    [SerializeField] private Transform dominantHandTransform;
    [SerializeField] private Transform nonDominantHandTransform;
    [SerializeField] private LineRenderer dominantLine;
    [SerializeField] private LineRenderer nonDominantLine;
    //private LayerMask interactableLayer;
    SearchExperimentLevel searchExperimentLevel;
    GenerateObjects GenerateObjects;
    public bool triggerButtonDown;
    //public XRController rightHand;
    public ActionBasedController rightHand;
    public InputHelpers.Button triggerButton;
    private float timer = 0f;
    private Transform controller;
    public float maxDistance = 5f;
    public string hitObjectName;
    public GameObject hitObject;
    private GameObject prevHitObj;
    public bool prevTriggerDown;
    public bool hasHit;
    // Start is called before the first frame update
    void Start()
    {
        searchExperimentLevel = FindObjectOfType<SearchExperimentLevel>();
        GenerateObjects = GetComponent<GenerateObjects>();
    }

    // Update is called once per frame
    async void Update()
    {
        dominantLine.enabled = true;
        nonDominantLine.enabled = true;
        RaycastHit[] dominantHits;

        int layer = 12;
        int layerMask = 1 << layer;


        hitObjectName = "";
        triggerButtonDown = clickedObject_Right.action.IsPressed();
        //triggerButtonDown = false;
        //triggerButtonDown = rightHand.activateAction.action.IsPressed();
        //rightHand.inputDevice.IsPressed(triggerButton, out triggerButtonDown);
        //Debug.Log("Trigger Status: " + triggerButtonDown);

        
        controller = rightHand.transform;
        // Calculate the direction from the controller
        Vector3 controllerDirection = controller.forward;

        int LayerIgnoreRaycast = LayerMask.NameToLayer("Interactables");
        RaycastHit hit;
        //int layer = 12;
        //int layerMask = 1 << layer;
        hasHit = Physics.Raycast(dominantHandTransform.position, dominantHandTransform.forward, out hit, Mathf.Infinity, layerMask);
        //Physics.Raycast(controller.position, controllerDirection, out hit, maxDistance, layerMask);
        if (hasHit)
        {
            searchExperimentLevel = FindObjectOfType<SearchExperimentLevel>();
            if (hit.transform.gameObject.GetComponent<Interactable>() != null && !searchExperimentLevel.selectStatusDict.ContainsKey(hit.transform.gameObject.GetComponent<Interactable>()))
            {
                searchExperimentLevel.selectStatusDict[hit.transform.gameObject.GetComponent<Interactable>()] = false;
            }
            if (searchExperimentLevel.selectStatusDict.ContainsKey(hit.transform.gameObject.GetComponent<Interactable>()))
            {
                if (searchExperimentLevel.selectStatusDict[hit.transform.gameObject.GetComponent<Interactable>()] == false)
                {
                    if (hit.transform.gameObject.GetComponent<Outline>() == null)
                    {
                        Outline outline = hit.transform.gameObject.AddComponent<Outline>();
                        outline.OutlineColor = Color.red;
                        outline.OutlineWidth = 10f;
                        outline.OutlineMode = Outline.Mode.OutlineVisible;
                    }
                    else
                    {
                        hit.transform.gameObject.GetComponent<Outline>().OutlineColor = Color.red;
                    }
                }
                //if hit object select status == false: if outline is null: highlight red

                if (triggerButtonDown && !prevTriggerDown)
                {
                    if (searchExperimentLevel.selectStatusDict[hit.transform.gameObject.GetComponent<Interactable>()] == false)
                    {
                        searchExperimentLevel.selectStatusDict[hit.transform.gameObject.GetComponent<Interactable>()] = true;
                        if (hit.transform.gameObject.GetComponent<Outline>() == null)
                        {
                            Outline outline = hit.transform.gameObject.AddComponent<Outline>();
                            outline.OutlineColor = Color.green;
                            outline.OutlineWidth = 10f;
                            outline.OutlineMode = Outline.Mode.OutlineVisible;
                        }
                        else
                        {
                            hit.transform.gameObject.GetComponent<Outline>().OutlineColor = Color.green;
                        }
                    }
                    else
                    {
                        searchExperimentLevel.selectStatusDict[hit.transform.gameObject.GetComponent<Interactable>()] = false;
                        if (hit.transform.gameObject.GetComponent<Outline>() == null)
                        {
                            Outline outline = hit.transform.gameObject.AddComponent<Outline>();
                            outline.OutlineColor = Color.red;
                            outline.OutlineWidth = 10f;
                            outline.OutlineMode = Outline.Mode.OutlineVisible;
                        }
                        else
                        {
                            hit.transform.gameObject.GetComponent<Outline>().OutlineColor = Color.red;
                        }
                    }
                }
                //if trigger&&!prevtrigger:
                //if hit object select status == false:
                //select status = true, add/modify green outline
                //if hit object select status == true:
                //select status = false, modify red outline

                if (hit.transform.gameObject != prevHitObj && prevHitObj != null)
                {
                    if (searchExperimentLevel.selectStatusDict.ContainsKey(prevHitObj.GetComponent<Interactable>()))
                    {
                        if (searchExperimentLevel.selectStatusDict[prevHitObj.GetComponent<Interactable>()] == false)
                        {
                            if (prevHitObj.GetComponent<Outline>() != null)
                            {
                                Outline outline = prevHitObj.GetComponent<Outline>();
                                Destroy(outline);
                            }
                        }
                    }
                        
                }
                //if hit object != prev hit:
                //if select status == false:
                //remove outline if exists

                prevHitObj = hit.transform.gameObject;
            }
        }
        prevTriggerDown = triggerButtonDown;


    }
}
