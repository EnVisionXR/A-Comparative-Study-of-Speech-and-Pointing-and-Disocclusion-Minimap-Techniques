using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;
using Unity.XR.CoreUtils;
using Microsoft.CognitiveServices.Speech;

public class DiscPIMPanel : MonoBehaviour
{
    SearchExperimentManager searchExperimentManager;
    SearchExperimentLevel searchExperimentLevel; // Reference to the SearchExperimentLevel script
    public Text numObjectFeedbackText; // Reference to the Text component for speech feedback


    private void Start()
    {
        // Initialize the speech feedback text
        numObjectFeedbackText.text = "Awaiting trial start...";
        
    }

    private void Update()
    {
        if (SearchExperimentTrial.isTrialOngoingNow == false)
        {
            numObjectFeedbackText.text = "Awaiting trial start...";
        }

        else
        {
            searchExperimentManager = FindObjectOfType<SearchExperimentManager>();
            searchExperimentLevel = FindObjectOfType<SearchExperimentLevel>();
            int totalTargets = searchExperimentManager.numberOfTargets;
            int selectedTargets = searchExperimentLevel.selectStatusDict.Count(x => x.Value);
            int remainingTargets = totalTargets - selectedTargets;
            numObjectFeedbackText.text = "Total targets: " + totalTargets + "\r\nSelected: " + selectedTargets + "\r\nRemaining: " + remainingTargets;
        }
        
        // Update the list of selected objects
        //UpdateSelectedObjectsList();
        //prevLeftGrabButtonDown = leftGrabButtonDown;

    }

    

}