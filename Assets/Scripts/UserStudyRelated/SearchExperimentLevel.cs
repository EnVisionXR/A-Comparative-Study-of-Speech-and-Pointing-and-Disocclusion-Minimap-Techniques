using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;
using Random = UnityEngine.Random;

public class SearchExperimentLevel : MonoBehaviour
{
    //private SpeechRecognition speechRecognition;
    // Additional Code
    private TargetAreaOutline[] targetAreaOutlines;
    private List<GameObject> targetInteractableList = new List<GameObject>();
    public enum ExperimentLevelState
    { Idle, BeforeNextTrial, RunningTrial, Finished }

    public SearchExperimentTrial currentTrial;
    private Queue<SearchExperimentTrial> remainingTrials;
    private List<SearchExperimentTrial> completedTrials;

    private string levelName;

    [ReadOnly] public ExperimentLevelState state = ExperimentLevelState.Idle;
    [ReadOnly] public SelectionTechniqueManager.SelectionTechnique levelTechnique;
    [ReadOnly] public int levelDensity = -1;

    private BoundaryCircleManager readinessCircleManager;

    private SelectionTechniqueManager slectionTechniqueDistributer;

    private TMP_Text experimentText;

    private List<Vector3> targetPositions;
    private List<string> targetMaterials;
    //private Material targetMaterial = Resources.Load("Target", typeof(Material)) as Material;
    private int currentTargetPositionIdx = 0;
    public Dictionary<Interactable, bool> selectStatusDict = new Dictionary<Interactable, bool>();

    private SearchExperimentManager searchExperimentManager;

    private void Start()
    {
        readinessCircleManager = FindObjectOfType<BoundaryCircleManager>();
        if (readinessCircleManager == null)
        { Debug.LogError("Did not find boundary circle manager"); };

        experimentText = GameObject.Find("ExperimentTextTMP").GetComponent<TMP_Text>();
        if (experimentText == null)
        { Debug.LogError("Did not find experiment text"); }

        slectionTechniqueDistributer = FindObjectOfType<SelectionTechniqueManager>();

        targetPositions = GetSearchPositions();
        targetMaterials = GetMaterials();
    }

    public void StartLevel(in int randomSeed, in int numTrialsPerlevel, in int numberOfTargets)
    {
        print("-> Level START <-");
        levelName = $"{levelTechnique}_dens{levelDensity}";

        selectStatusDict = new Dictionary<Interactable, bool>();

        GetComponent<LevelManager>().DisableAllLevels();
        GetComponent<LevelManager>().EnableDensityLevel(levelDensity);
        GetComponent<SelectionTechniqueManager>().ActivateTechnique(levelTechnique);

        SearchExperimentLogger.densityLevel = levelDensity;
        SearchExperimentLogger.num_targets = numberOfTargets;
        SearchExperimentLogger.selectionTechnique = levelTechnique;

        Random.InitState(randomSeed);

        currentTrial = null;
        completedTrials = new List<SearchExperimentTrial>();
        remainingTrials = new Queue<SearchExperimentTrial>();

        for (int trialIdx = 1; trialIdx < numTrialsPerlevel + 1; trialIdx++)
        {
            remainingTrials.Enqueue(new SearchExperimentTrial(trialIdx, SearchExperimentTrial.SearchExperimentTrialType.Search, numberOfTargets));
            remainingTrials.Enqueue(new SearchExperimentTrial(trialIdx, SearchExperimentTrial.SearchExperimentTrialType.Repeat, numberOfTargets));
        }

        TransitionToBeforeTrial();
    }

    public void EndLevel()
    {
        state = ExperimentLevelState.Finished;
        ComputeLevelStats();

        print("-> Level END <-");

        GetComponent<LevelManager>().DisableAllLevels();
        GetComponent<SelectionTechniqueManager>().DisableAllTechniques();

        ClonedObject[] clonedObjects = FindObjectsOfType<ClonedObject>();
        foreach (ClonedObject clonedObject in clonedObjects)
        {
            if (clonedObject.gameObject.GetComponent<Interactable>() != null)
            {
                if (selectStatusDict.ContainsKey(clonedObject.gameObject.GetComponent<Interactable>()))
                {
                    selectStatusDict.Remove(clonedObject.gameObject.GetComponent<Interactable>());
                }
            }
            Destroy(clonedObject.gameObject);
        }

        TemporaryTarget[] temporaryTargets = FindObjectsOfType<TemporaryTarget>();
        foreach (TemporaryTarget temporaryTarget in temporaryTargets)
        {
            GameObject obj = temporaryTarget.gameObject;
            if (obj.layer == 12)
            {
                //if (obj.GetComponent<Interactable>() != null)
                //{
                if (selectStatusDict.ContainsKey(obj.GetComponent<Interactable>()))
                {
                    selectStatusDict.Remove(obj.GetComponent<Interactable>());
                }
                //}
                
                // If using DiscPIM
                MiniMapInteractor miniMapInteractor = FindObjectOfType<MiniMapInteractor>();
                if (miniMapInteractor != null)
                {
                    miniMapInteractor.originalToDuplicateMap.Remove(obj);
                    miniMapInteractor.originalToDuplicateCir.Remove(obj);
                }
                Destroy(obj);
            }
        }
    }

    public void TransitionToNextTrial()
    {
        //GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("temporary targets");
        TemporaryTarget[] temporaryTargets = FindObjectsOfType<TemporaryTarget>();
        foreach (TemporaryTarget temporaryTarget in temporaryTargets)
        {
            GameObject obj = temporaryTarget.gameObject;
            if (obj.layer == 12)
            {
                Destroy(obj);
                if (selectStatusDict.ContainsKey(obj.GetComponent<Interactable>()))
                {
                    selectStatusDict.Remove(obj.GetComponent<Interactable>());
                }
            }
        }

        //Disable all outlines
        Outline[] outls = UnityEngine.Object.FindObjectsOfType<Outline>();
        foreach (Outline outl in outls)
        {
            outl.gameObject.GetComponent<Outline>().enabled = false;
        }
        foreach (Interactable go in FindObjectsOfType<Interactable>())
        {
            if (go.gameObject.GetComponent<Outline>() != null)
            {
                Destroy(go.gameObject.GetComponent<Outline>());
            }
            selectStatusDict[go] = false;
        }
        
        FindObjectOfType<GrabbingHand>().ClearGrabbed();
        // Reset because they were probably moved by this technique
        var gz = FindObjectOfType<GravityZone>();
        if (gz != null) gz.ResetInteractables();

        currentTrial = remainingTrials.Dequeue();
        // check if it's a repeat trial
        if (currentTrial.type == SearchExperimentTrial.SearchExperimentTrialType.Repeat)
        {
            experimentText.text = "Repeat\nTarget\nSelection";
        }
        else
        {
            experimentText.text = "Search\nfor\nTarget";
            //currentTargetPositionIdx++;
            currentTargetPositionIdx += currentTrial.numberOfTargets;
            if (currentTargetPositionIdx >= targetPositions.Count - 9)
            {
                currentTargetPositionIdx = 0;
            }
            Debug.Log("Current Target Position Index: " + currentTargetPositionIdx);
        }

        Debug.Log("target position length: " + targetPositions.Count);

        Vector3 camPosition = FindObjectOfType<Camera>().transform.position;
        currentTrial.distToTarget = Vector3.Distance(camPosition, targetPositions[currentTargetPositionIdx]);

        // Additional Code
        //List<GameObject> targetInteractableList = new List<GameObject>();
        List<ClonedObject> clonedObjects = new List<ClonedObject>(FindObjectsOfType<ClonedObject>());
        
        //SearchTargetInteractable[] targetInteractables = FindObjectsOfType<SearchTargetInteractable>();

        foreach (ClonedObject clonedObject in clonedObjects)
        {
            GameObject clonedGO = clonedObject.gameObject;
            Destroy(clonedGO);
        }

        //List<SearchTargetInteractable> searchTargetInteractables = new List<SearchTargetInteractable>(FindObjectsOfType<SearchTargetInteractable>());

        //for (int i = 0; i < searchTargetInteractables.Count; i++)
        //{
        //    if (searchTargetInteractables[i].gameObject.GetComponent<ClonedObject>() != null || searchTargetInteractables[i].gameObject.GetComponent<TemporaryTarget>() != null)
        //    {
        //        searchTargetInteractables.RemoveAt(i);
        //    }

        //}

        List<SearchTargetInteractable> searchTargetInteractables = new List<SearchTargetInteractable>(FindObjectsOfType<SearchTargetInteractable>());

        searchTargetInteractables = searchTargetInteractables.Where(interactable => interactable.gameObject.GetComponent<ClonedObject>() == null && interactable.gameObject.GetComponent<TemporaryTarget>() == null).ToList();

        for (int i = 0; i < searchTargetInteractables.Count; i++)
        {
            Debug.Log("searchtargetInteractables[" + i + "]: " + searchTargetInteractables[i]);
        }

        List<TargetInteractable> targetInteractables = new List<TargetInteractable>(FindObjectsOfType<TargetInteractable>());

        for (int i = 0; i < targetInteractables.Count; i++)
        {
            Debug.Log("targetInteractables[" + i + "]: " + targetInteractables[i]);
            if (targetInteractables[i].gameObject.GetComponent<ClonedObject>() != null || targetInteractables[i].gameObject.GetComponent<TemporaryTarget>() != null)
            {
                targetInteractables.RemoveAt(i);
            }
        }



        //foreach (SearchTargetInteractable targetInteractable in targetInteractables)
        //{
        //    if (targetInteractable.gameObject.tag == "temporary targets")
        //    {
        //        //remove element from list
        //    }
        //}

        int randomTargetInteractableIndex = Random.Range(0, searchTargetInteractables.Count);

        Debug.Log("Random target index: " + randomTargetInteractableIndex);

        int randomMaterialIndex = Random.Range(0, targetMaterials.Count);
        string objectMaterial = targetMaterials[randomMaterialIndex];
        Material objecttargetAppearance = Resources.Load(objectMaterial, typeof(Material)) as Material;
        if (currentTrial.type == SearchExperimentTrial.SearchExperimentTrialType.Search)
        {
            SearchExperimentTrial.targetInteractable = searchTargetInteractables[randomTargetInteractableIndex];
            Debug.Log("Target interactable assigned: " + SearchExperimentTrial.targetInteractable.gameObject.name);
            //Material targetMaterial = objecttargetAppearance;
            //SearchExperimentTrial.targetInteractable.GetComponent<MeshRenderer>().materials[0] = objecttargetAppearance;
        }

        GameObject[] gos = GameObject.FindGameObjectsWithTag("homeObject");
        foreach (GameObject go in gos)
            Destroy(go);

        // Draw Home Object and White Home Region
        GameObject homeObject = Instantiate(SearchExperimentTrial.targetInteractable.gameObject, new Vector3(-0.75f, 1.2f, -8.2f), Quaternion.identity);
        homeObject.tag = "homeObject";
        homeObject.layer = 2;
        GameObject homeRegion = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        homeRegion.tag = "homeObject";

        homeRegion.GetComponent<Transform>().position = new Vector3(-0.75f, 1.2f, -8.2f);
        homeRegion.GetComponent<Transform>().localScale = new Vector3(0.7f, 0.7f, 0.7f);
        Material homeBlur = Resources.Load("region of interest", typeof(Material)) as Material;
        homeRegion.GetComponent<Renderer>().material = homeBlur;
        homeObject.AddComponent<ClonedObject>();

        //SearchExperimentTrial.targetInteractable = FindObjectOfType<SearchTargetInteractable>();
        //currentTrial.StartTrial(targetPositions[currentTargetPositionIdx]);
        currentTrial.StartTrial(targetPositions, currentTargetPositionIdx, currentTrial.numberOfTargets);
        //TargetAreaOutline.EnableSearchOutlineAroundPosition(
        //    camPosition,
        //    targetPositions[currentTargetPositionIdx],
        //    currentTrial.type == SearchExperimentTrial.SearchExperimentTrialType.Search);

        targetAreaOutlines = FindObjectsOfType<TargetAreaOutline>();
        for (int i = 0; i < currentTrial.numberOfTargets; i++)
        {
            //Debug.Log("Positioning " + targetAreaOutlines[i].gameObject.name + " to " + targetPositions[currentTargetPositionIdx + i]);
            targetAreaOutlines[i].EnableSearchOutlineAroundPosition(
            camPosition,
            targetPositions[currentTargetPositionIdx+i],
            currentTrial.type == SearchExperimentTrial.SearchExperimentTrialType.Search);
            
        }

        //if (currentTrial.numberOfTargets > 1)
        //{
        //    for (int i = 1; i < currentTrial.numberOfTargets; i++)
        //    {
        //        GameObject newTarget = targetInteractable.InstantiateCopy(targetPositions[currentTargetPositionIdx + i]);
        //        //newTarget.name = targetInteractable.gameObject.name;
        //        newTarget.tag = "temporary targets";
        //    }
        //}

        slectionTechniqueDistributer.clearCurrentTechnique(levelTechnique);

        // Re-calculate direction because the target position was just modified
        if (gz != null) gz.UpdateSearchTargetDirection(SearchExperimentTrial.targetInteractable);

        //currentTargetPositionIdx += currentTrial.numberOfTargets;

        state = ExperimentLevelState.RunningTrial;
    }

    public void TransitionToBeforeTrial()
    {
        targetAreaOutlines = FindObjectsOfType<TargetAreaOutline>();
        foreach(TargetAreaOutline targetAreaOutline in targetAreaOutlines)
        {
            targetAreaOutline.DisableSearchOutlineAroundPosition();
        }
        

        if (currentTrial != null)
        {
            completedTrials.Add(currentTrial);
        }

        if (remainingTrials.Count == 0)
        {
            EndLevel();
            return;
        }

        readinessCircleManager.SetWaitForUserReady();

        if (remainingTrials.Peek().type == SearchExperimentTrial.SearchExperimentTrialType.Search)
            experimentText.text = "Start\nSearch";
        else
            experimentText.text = "Start\nRepeat";

        state = ExperimentLevelState.BeforeNextTrial;
    }

    private void Update()
    {
        switch (state)
        {
            case ExperimentLevelState.Idle:
            case ExperimentLevelState.Finished:
                break;

            case ExperimentLevelState.BeforeNextTrial:
                if (readinessCircleManager.UserConfirmedReadiness())
                    TransitionToNextTrial();
                break;

            case ExperimentLevelState.RunningTrial:
                if (currentTrial.WasCompleted())
                {
                    // Reset because they were probably moved by this technique
                    var gz = FindObjectOfType<GravityZone>();
                    if (gz != null) gz.ResetInteractables();

                    TransitionToBeforeTrial();
                }

                break;
        }
    }

    private void ComputeLevelStats()
    {
        float numTrials = completedTrials.Count / 2;

        int numFirstAttemptSearchTrials = 0, numFirstAttemptSelectTrials = 0;
        float totalSearchTime = 0, totalSelectTime = 0;

        foreach (SearchExperimentTrial et in completedTrials)
        {
            if (et.SuccessAtFirstAttempt())
            {
                if (et.type == SearchExperimentTrial.SearchExperimentTrialType.Search)
                    numFirstAttemptSearchTrials += 1;
                else
                    numFirstAttemptSelectTrials += 1;
            }

            if (et.type == SearchExperimentTrial.SearchExperimentTrialType.Search)
                totalSearchTime += et.ComputeTrialTime();
            else
                totalSelectTime += et.ComputeTrialTime();
        }

        float firstAttemptSearchPercentage = (float)numFirstAttemptSearchTrials / (float)numTrials;
        float firstAttemptSelectPercentage = (float)numFirstAttemptSelectTrials / (float)numTrials;

        float avgSearchTime = (float)totalSearchTime / (float)numTrials;
        float avgSelectTime = (float)totalSelectTime / (float)numTrials;

        print($"> {levelName}: search at first attempt: {firstAttemptSearchPercentage}, search time: {avgSearchTime}");
        print($"> {levelName}: select at first attempt: {firstAttemptSelectPercentage}, select time: {avgSelectTime}");
    }

    private List<string> GetMaterials()
    {
        List<string> targetMaterials = new List<string>
        {
            "purple plastic",
            "blue plastic",
            "red plastic",
            "green plastic"
        };
        return targetMaterials; 
    }

    private List<Vector3> GetSearchPositions()
    {
        List<Vector3> searchPositions = new List<Vector3>
        {
            new Vector3(0.199000001f,1.36899996f,-6.91900015f),
            new Vector3(0.365999997f,2.09500003f,-5.13100004f),
            new Vector3(-0.23199999f,1.08500004f,-4.42799997f),
            new Vector3(-0.29499998f,3.1070001f,-4.03499985f),
            new Vector3(-0.569999993f,2.5999999f,-2.5f),
            new Vector3(0.720000029f,1.89999998f,-2.5f),
            new Vector3(-1.02499998f,1f,-2.5f),
            // remove below
            new Vector3(1.1799999f,1.13f,-0.109999999f),
            new Vector3(-1.91999996f,2.21000004f,-0.109999999f),
            new Vector3(-0.280000001f,2.21000004f,-0.109999999f),
            new Vector3(-0.280000001f,0.370000005f,-0.109999999f),
            new Vector3(1.02999997f,0.879999995f,1.48000002f),
            new Vector3(-2.99000001f,0.930000007f,1.48000002f),
            new Vector3(-2.01999998f,0.930000007f,0.709999979f),
            new Vector3(-0.347000003f,2.20600009f,1.4800000f),
            new Vector3(0.298999995f,1.63900006f,2.49399996f),
            new Vector3(1.12399995f,1.70500004f,1.48000002f),
            new Vector3(-0.152999997f,0.270000011f,1.53999996f),
            new Vector3(-1.65999997f,0.379999995f,1.53999996f),
            new Vector3(-0.629999995f,1.33000004f,3.31999993f),
            new Vector3(1.11699998f,2.11800003f,3.31999993f),
            new Vector3(-1.48000002f,2.1400001f,3.31999993f),
            new Vector3(-2.97000003f,2.01999998f,3.31999993f),
            // remove above
            new Vector3(-1.40400004f,2.13000011f,-5.92000008f),
            new Vector3(1.04700005f,1.90100002f,-5.92000008f),
            new Vector3(1.20799994f,2.046f,-3.51999998f),
            new Vector3(-0.633000016f,1.99100006f,-3.74000001f),
            new Vector3(0.375f,1.44299996f,-0.0430000015f),
            new Vector3(-2.1099999f,1.78199995f,-1.80999994f),
            new Vector3(-1.93900001f,1.11500001f,-5.2329998f),
            ////
            //new Vector3(0.199000001f,1.36899996f,-6.91900015f),
            //new Vector3(0.365999997f,2.09500003f,-5.13100004f),
            //new Vector3(-0.23199999f,1.08500004f,-4.42799997f),
            //new Vector3(-0.29499998f,3.1070001f,-4.03499985f),
            //new Vector3(-0.569999993f,2.5999999f,-2.5f),
            //new Vector3(0.720000029f,1.89999998f,-2.5f),
            //new Vector3(-1.02499998f,1f,-2.5f),
            //new Vector3(-1.40400004f,2.13000011f,-5.92000008f),
            //new Vector3(1.04700005f,1.90100002f,-5.92000008f),
            //new Vector3(1.20799994f,2.046f,-3.51999998f),
            //new Vector3(-0.633000016f,1.99100006f,-3.74000001f),
            //new Vector3(0.375f,1.44299996f,-0.0430000015f),
            //new Vector3(-2.1099999f,1.78199995f,-1.80999994f),
            //new Vector3(-1.93900001f,1.11500001f,-5.2329998f),
            ////
            //new Vector3(0.199000001f,1.36899996f,-6.91900015f),
            //new Vector3(0.365999997f,2.09500003f,-5.13100004f),
            //new Vector3(-0.23199999f,1.08500004f,-4.42799997f),
            //new Vector3(-0.29499998f,3.1070001f,-4.03499985f),
            //new Vector3(-0.569999993f,2.5999999f,-2.5f),
            //new Vector3(0.720000029f,1.89999998f,-2.5f),
            //new Vector3(-1.02499998f,1f,-2.5f),
            //new Vector3(-1.40400004f,2.13000011f,-5.92000008f),
            //new Vector3(1.04700005f,1.90100002f,-5.92000008f),
            //new Vector3(1.20799994f,2.046f,-3.51999998f),
            //new Vector3(-0.633000016f,1.99100006f,-3.74000001f),
            //new Vector3(0.375f,1.44299996f,-0.0430000015f),
            //new Vector3(-2.1099999f,1.78199995f,-1.80999994f),
            //new Vector3(-1.93900001f,1.11500001f,-5.2329998f),
            ////
            //new Vector3(0.199000001f,1.36899996f,-6.91900015f),
            //new Vector3(0.365999997f,2.09500003f,-5.13100004f),
            //new Vector3(-0.23199999f,1.08500004f,-4.42799997f),
            //new Vector3(-0.29499998f,3.1070001f,-4.03499985f),
            //new Vector3(-0.569999993f,2.5999999f,-2.5f),
            //new Vector3(0.720000029f,1.89999998f,-2.5f),
            //new Vector3(-1.02499998f,1f,-2.5f),
            //new Vector3(-1.40400004f,2.13000011f,-5.92000008f),
            //new Vector3(1.04700005f,1.90100002f,-5.92000008f),
            //new Vector3(1.20799994f,2.046f,-3.51999998f),
            //new Vector3(-0.633000016f,1.99100006f,-3.74000001f),
            //new Vector3(0.375f,1.44299996f,-0.0430000015f),
            //new Vector3(-2.1099999f,1.78199995f,-1.80999994f),
            //new Vector3(-1.93900001f,1.11500001f,-5.2329998f),
            ////
            //new Vector3(0.199000001f,1.36899996f,-6.91900015f),
            //new Vector3(0.365999997f,2.09500003f,-5.13100004f),
            //new Vector3(-0.23199999f,1.08500004f,-4.42799997f),
            //new Vector3(-0.29499998f,3.1070001f,-4.03499985f),
            //new Vector3(-0.569999993f,2.5999999f,-2.5f),
            //new Vector3(0.720000029f,1.89999998f,-2.5f),
            //new Vector3(-1.02499998f,1f,-2.5f),
            //new Vector3(-1.40400004f,2.13000011f,-5.92000008f),
            //new Vector3(1.04700005f,1.90100002f,-5.92000008f),
            //new Vector3(1.20799994f,2.046f,-3.51999998f),
            //new Vector3(-0.633000016f,1.99100006f,-3.74000001f),
            //new Vector3(0.375f,1.44299996f,-0.0430000015f),
            //new Vector3(-2.1099999f,1.78199995f,-1.80999994f),
            //new Vector3(-1.93900001f,1.11500001f,-5.2329998f)
        };
        //List<Vector3> searchPositions_var = new List<Vector3>();

        //foreach (Vector3 position in searchPositions)
        //{
        //    Vector3 modifiedPosition = position + new Vector3(0f, 0f, 0.0001f);
        //    searchPositions_var.Add(modifiedPosition);
        //}
        //searchPositions = searchPositions.Concat(searchPositions_var).ToList();

        //foreach (Vector3 position in searchPositions)
        //{
        //    Vector3 modifiedPosition = position + new Vector3(0f, 0.0001f, 0f);
        //    searchPositions_var.Add(modifiedPosition);
        //}
        //searchPositions = searchPositions.Concat(searchPositions_var).ToList();

        //foreach (Vector3 position in searchPositions)
        //{
        //    Vector3 modifiedPosition = position + new Vector3(0.0001f, 0f, 0f);
        //    searchPositions_var.Add(modifiedPosition);
        //}
        //searchPositions = searchPositions.Concat(searchPositions_var).ToList();

        searchPositions = searchPositions.OrderBy(a => Guid.NewGuid()).ToList();

        return searchPositions;
    }
}