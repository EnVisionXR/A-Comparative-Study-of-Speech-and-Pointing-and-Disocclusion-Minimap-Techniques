using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Unity.XR.CoreUtils;

public class DraggablePanel : MonoBehaviour
{
    [SerializeField] private InputActionReference grab_left;
    [SerializeField] private InputActionReference grab_right;
    [SerializeField] private Transform dominantHandTransform;
    [SerializeField] private Transform nonDominantHandTransform;
    public bool leftGrabButtonDown = false;
    public bool prevLeftGrabButtonDown = false;
    public bool rightGrabButtonDown = false;
    public bool prevRightGrabButtonDown = false;
    SearchExperimentLevel searchExperimentLevel; // Reference to the SearchExperimentLevel script
    SpeechRecognition speechRecognition;
    public Text speechFeedbackText; // Reference to the Text component for speech feedback
    public GameObject selectedObjectsContent; // Reference to the container for the list of selected objects
    public GameObject[] selectedObjectContainers;

    private bool[] containerOccupied = new bool[8];
    private bool isDragging = false;
    private Vector3 originalPanelPosition;
    private Quaternion originalPanelRotation;
    private Dictionary<GameObject, Interactable> containerContent = new Dictionary<GameObject, Interactable>();
    private Dictionary<Interactable, bool> interactableInContainer = new Dictionary<Interactable, bool>();

    private void Start()
    {
        // Initialize the speech feedback text
        speechRecognition = FindObjectOfType<SpeechRecognition>();
        speechFeedbackText.text = "<Recognized speech will appear here>";
        foreach (Interactable go in FindObjectsOfType<Interactable>())
        {
            interactableInContainer[go] = false;
        }
    }        

    private void Update()
    {
        searchExperimentLevel = FindObjectOfType<SearchExperimentLevel>();
        if (SearchExperimentTrial.isTrialOngoingNow == false)
        {
            speechFeedbackText.text = "<Recognized speech will appear here>";
            foreach (GameObject selectedObjectContainer in selectedObjectContainers)
            {
                ResetContainer(selectedObjectContainer);
            }
            foreach (Interactable go in FindObjectsOfType<Interactable>())
            {
                if (go.gameObject.GetComponent<Outline>() != null)
                {
                    Destroy(go.gameObject.GetComponent<Outline>());
                }
                searchExperimentLevel.selectStatusDict[go] = false;
            }
        }
        //Debug.Log("Left grab down: " + leftGrabButtonDown);
        //Debug.Log("Is dragging: " + isDragging);
        leftGrabButtonDown = grab_left.action.IsPressed();
        rightGrabButtonDown = grab_right.action.IsPressed();
        // Check if the left controller's grabbing button is pressed
        if ((leftGrabButtonDown | rightGrabButtonDown) && !isDragging)
        {
            // Start dragging the panel
            StartDragging();
            
        }
        else if ((leftGrabButtonDown | rightGrabButtonDown) && isDragging)
        {
            // Update the panel's position and rotation based on the left controller
            UpdatePanelTransform();
        }
        else
        {
            // Release the panel
            StopDragging();
        }

        UpdateSelectedObjectsList(searchExperimentLevel.selectStatusDict);

        //if (SearchExperimentTrial.isTrialOngoingNow)
        //{
        //    // Update the list of selected objects
        //    UpdateSelectedObjectsList();
        //}
        
        //prevLeftGrabButtonDown = leftGrabButtonDown;

    }

    private void StartDragging()
    {
        isDragging = true;
        originalPanelPosition = transform.position;
        originalPanelRotation = transform.rotation;
    }

    private void UpdatePanelTransform()
    {
        if (leftGrabButtonDown)
        {
            Vector3 handPosition = nonDominantHandTransform.position;
            Vector3 offset = nonDominantHandTransform.TransformVector(new Vector3(0.2f, 0.15f, 0f));
            transform.position = handPosition + offset;
            //transform.position = nonDominantHandTransform.position + new Vector3(0.2f, 0.15f, 0f);
            transform.rotation = nonDominantHandTransform.rotation;
        }
        else
        {
            Vector3 handPosition = dominantHandTransform.position;
            Vector3 offset = dominantHandTransform.TransformVector(new Vector3(-0.2f, 0.15f, 0f));
            transform.position = handPosition + offset;
            //transform.position = dominantHandTransform.position + new Vector3(-0.2f, 0.15f, 0f);
            transform.rotation = dominantHandTransform.rotation;
        }
        
    }

    private void StopDragging()
    {
        isDragging = false;
    }

    private void UpdateSelectedObjectsList(Dictionary<Interactable, bool> selectStatusDict)
    {
        //searchExperimentLevel = FindObjectOfType<SearchExperimentLevel>();
        int numberOfSelectedObjects = 0;

        foreach (KeyValuePair<Interactable, bool> kvp in selectStatusDict)
        {
            //if (kvp.Value)
            //{
            //    if (kvp.Key != null)
            //    {
            //        Debug.Log(kvp.Key.gameObject.name + " select status true!");
            //    }
            //}
            //else
            //{
            //    if (kvp.Key != null)
            //    {
            //        Debug.Log(kvp.Key.gameObject.name + "select status false!");
            //    }
            //}
            
            if (kvp.Value && numberOfSelectedObjects < 8)
            {
                if (kvp.Key != null)
                {
                    if (selectedObjectContainers[numberOfSelectedObjects].transform.childCount > 0)
                    {
                        for (int i=0; i< selectedObjectContainers[numberOfSelectedObjects].transform.childCount; i++)
                        {
                            Destroy(selectedObjectContainers[numberOfSelectedObjects].transform.GetChild(i).gameObject);
                        }
                        
                    }
                    // Instantiate the GameObject if the value in the dictionary is true
                    GameObject model = Instantiate(kvp.Key.gameObject, selectedObjectContainers[numberOfSelectedObjects].transform);
                    //Debug.Log("instantiating object in panel: " + model.name);
                    
                    model.AddComponent<IsInPanel>();
                    Vector3 containerPosition = selectedObjectContainers[numberOfSelectedObjects].transform.position;
                    Vector3 offset = selectedObjectContainers[numberOfSelectedObjects].transform.TransformVector(new Vector3(0f, 25f, 0f));
                    model.transform.position = containerPosition + offset;
                    model.transform.rotation = selectedObjectContainers[numberOfSelectedObjects].transform.rotation;
                    model.transform.localScale = Vector3.one * 25f;
                    if (model.tag == "UnknownObject1")
                    {
                        model.transform.localScale = new Vector3(1f, 1.4f, 1f) * 25f;
                    }
                }

                if (selectedObjectContainers[numberOfSelectedObjects].GetComponent<Text>() != null)
                {
                    Text containerText = selectedObjectContainers[numberOfSelectedObjects].GetComponent<Text>();
                    containerText.text = kvp.Key.gameObject.name;
                }

                numberOfSelectedObjects++;
            }
        }

        if (numberOfSelectedObjects < 8)
        {
            for (int i = numberOfSelectedObjects; i < 8; i++)
            {
                ResetContainer(selectedObjectContainers[i]);
            }
        }
    }


    private void ResetContainer(GameObject selectedObjectContainer)
    {

        foreach (Transform child in selectedObjectContainer.transform)
        {
            Destroy(child.gameObject);
        }
        if (selectedObjectContainer.GetComponent<Text>() != null)
        {
            Text containerText = selectedObjectContainer.GetComponent<Text>();
            containerText.text = "<Selected Object>";
        }
    }
    public void SetSpeechFeedback(string feedback)
    {
        speechFeedbackText.text = feedback;
    }
}