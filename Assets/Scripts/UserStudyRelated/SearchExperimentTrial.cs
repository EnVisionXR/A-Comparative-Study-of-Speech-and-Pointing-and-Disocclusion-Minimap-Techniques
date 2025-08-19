using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
//using static Unity.Collections.NativeArray<T>;

public class SearchExperimentTrial
{
    public bool temptargetscloned = false;
    [ReadOnly] public SelectionTechniqueManager.SelectionTechnique levelTechnique;
    //public event EventHandler TrialStarted;
    public enum SearchExperimentTrialType
    { Search, Repeat }

    public SearchExperimentTrialType type;
    private static SearchExperimentLevel searchExperimentLevel;

    public static SearchExperimentTrial activeTrial = null;
    public static SearchTargetInteractable targetInteractable = null;
    public soundSystemHolder soundSystemHolder = GameObject.FindObjectOfType<soundSystemHolder>();

    public int trialIdx = 0;

    public int numberOfTargets = 1;

    private bool targetWasClicked = false;

    private float trialStartTime = 0f;
    private float trialCompleteTime = 0f;

    private int numAttempts = 0;

    public static bool isTrialOngoingNow = false;

    public float distToTarget = 0f;

    private List<Mesh> targetMeshList = new List<Mesh>();

    private void Start()
    {
        //searchExperimentLevel = FindObjectOfType<SearchExperimentLevel>();
        //miniMapInteractor = UnityEngine.Object.FindObjectOfType<MiniMapInteractor>();
    }



    public SearchExperimentTrial(in int _trialIdx, SearchExperimentTrialType t, in int _numberOfTargets)
    {
        this.trialIdx = _trialIdx;
        this.type = t;
        this.numberOfTargets = _numberOfTargets;
    }

    

    //public void StartTrial(in Vector3 searchTargetPosition)
    public void StartTrial(in List<Vector3> targetPositions, in int currentTargetPositionIdx, in int numberofTargets)
    {
        Debug.Log("-- Trial START --");
        isTrialOngoingNow = true;

        //miniMapInteractor.UpdateDuplicatesForMiniMap();

        Outline[] outls = UnityEngine.Object.FindObjectsOfType<Outline>();
        foreach (Outline outl in outls)
        {
            outl.gameObject.GetComponent<Outline>().enabled = false;
        }
        Vector3 searchTargetPosition = targetPositions[currentTargetPositionIdx];
        targetInteractable.OffHighlighting();
        targetInteractable.TeleportToPosition(searchTargetPosition);
        Debug.Log("Target interactable name " + targetInteractable.name);
        Mesh targetMesh = targetInteractable.GetComponent<MeshFilter>().mesh;
        Material targetMaterial = targetInteractable.GetComponent<MeshRenderer>().materials[0];


        if (numberOfTargets > 1)
        {
            for (int i = 1; i < numberOfTargets; i++)
            {
                GameObject newTarget = targetInteractable.InstantiateCopy(targetPositions[currentTargetPositionIdx + i]);
                newTarget.name = targetInteractable.name;
                //newTarget.name = targetInteractable.gameObject.name;
                //newTarget.tag = "temporary targets";
                if (newTarget.GetComponent<TemporaryTarget>() == null)
                {
                    newTarget.AddComponent<TemporaryTarget>();
                }
                
                // Added line
                //searchExperimentLevel.selectStatusDict[newTarget.GetComponent<Interactable>()] = false;
            }
        }

        temptargetscloned = true;

        //var objects = Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name == "Name");
        GameObject levelDistractors = GameObject.Find("Level1_128Objects");

        for (int i = 0; i < levelDistractors.transform.childCount; i++)
        {
            levelDistractors.transform.GetChild(i).gameObject.SetActive(true);
        }
        
        Interactable[] allObjects = UnityEngine.Object.FindObjectsOfType<Interactable>();
        foreach (Interactable obj in allObjects) 
        { 
            if (obj.gameObject.GetComponent<MeshFilter>().mesh.name == targetMesh.name && obj.gameObject.GetComponent<MeshRenderer>().materials[0].name == targetMaterial.name && obj.gameObject != targetInteractable.gameObject)
            {
                obj.gameObject.SetActive(false);
            }
        }

        //TrialStarted?.Invoke(this, EventArgs.Empty);

        //if ($"{levelTechnique}" == "OhMiniMap")
        //{
        //    MiniMapInteractor miniMapInteractor = UnityEngine.Object.FindObjectOfType<MiniMapInteractor>();
        //    miniMapInteractor.EnableMiniMapInteractor();
        //    //miniMapInteractor.CreateDuplicatesForMiniMap();
        //}
        //targetInteractable.GetComponent<MeshRenderer>().materials[0] = targetMaterial;

        SearchTargetInteractable.SetReferenceTransformForCurrentTrial(targetInteractable.transform);

        activeTrial = this;
        targetWasClicked = false;
        trialStartTime = Time.unscaledTime;
        numAttempts = 0;

        HandDistancesTraveled.StartRecording();

        //yield return null; // Wait for one frame
    }

    public void RecordTargetMiss()
    {
        soundSystemHolder.incorrectPlay();
        Debug.Log("Non-t was hit");
        numAttempts += 1;
    }

    public void RecordTargetHit()
    {
        soundSystemHolder.correctPlay();
        Debug.Log("Target was hit");
        numAttempts += 1;
        targetWasClicked = true;
        trialCompleteTime = Time.unscaledTime;
        EndTrial();
    }

    public void EndTrial()
    {
        temptargetscloned = false;
        ////Disable all outlines
        //Outline[] outls = UnityEngine.Object.FindObjectsOfType<Outline>();
        //foreach (Outline outl in outls)
        //{
        //    //Outline outline = prevHitObj.GetComponent<Outline>();
        //    //Destroy(outline);
        //    outl.gameObject.GetComponent<Outline>().enabled = false;
        //}
        var fname = SearchExperimentLogger.LogTrial(this);
        Debug.Log($"Wrote results file: {fname}");
        activeTrial = null;
        Debug.Log("-- Trial End --");

        

        isTrialOngoingNow = false;
        HandDistancesTraveled.FinishRecording();
        //TargetAreaOutline.DisableSearchOutlineAroundPosition();


    }

    public bool SuccessAtFirstAttempt()
    { return numAttempts == 1; }

    public bool WasCompleted()
    { return targetWasClicked; }

    public int GetNumAttempts()
    { return numAttempts; }

    public float ComputeTrialTime()
    { return trialCompleteTime - trialStartTime; }
}