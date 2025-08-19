using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;

public class ValidateSelection : MonoBehaviour
{
    SearchExperimentLevel searchExperimentLevel;
    [SerializeField] private InputActionReference secondaryButtonPressed;
    // Start is called before the first frame update
    void Start()
    {
        
    }



    // Update is called once per frame
    void Update()
    {
        if (secondaryButtonPressed.action.WasPressedThisFrame())
        {
            searchExperimentLevel = FindObjectOfType<SearchExperimentLevel>();
            bool select_correct = true;
            bool has_selection = false;
            bool all_targets_selected = true;

            if (ExperimentTrial.activeTrial != null)
            {
                searchExperimentLevel = FindObjectOfType<SearchExperimentLevel>();
                GameObject targetGO = ExperimentTrial.targetInteractable.gameObject;
                if (searchExperimentLevel.selectStatusDict.ContainsKey(targetGO.GetComponent<Interactable>()))
                {
                    if (searchExperimentLevel.selectStatusDict[targetGO.GetComponent<Interactable>()] == false)
                    {
                        all_targets_selected = false;
                        Debug.Log("Main target not selected");
                    }
                    else
                    {
                        Debug.Log("Main target selected");
                    }
                }
                else
                {
                    all_targets_selected = false;
                    Debug.Log("selecctStatusDict does not contain main target");
                }

                TargetInteractable[] allTargetInteractables = FindObjectsOfType<TargetInteractable>();
                foreach (TargetInteractable targetInteractable in allTargetInteractables)
                {
                    //if (targetInteractable.gameObject.tag == "temporary targets")
                    if (targetInteractable.gameObject.GetComponent<TemporaryTarget>() != null)
                    {
                        GameObject clonetargetGO = targetInteractable.gameObject;
                        if (searchExperimentLevel.selectStatusDict.ContainsKey(clonetargetGO.GetComponent<Interactable>()))
                        {
                            if (searchExperimentLevel.selectStatusDict[clonetargetGO.GetComponent<Interactable>()] == false)
                            {
                                all_targets_selected = false;
                                Debug.Log("Temporary target not selected");
                            }
                        }
                        else
                        {
                            Debug.Log("Temporary target selected");
                        }
                    }
                    else
                    {
                        all_targets_selected = false;
                        Debug.Log("Temporary target not in selectStatusDict");
                    }
                }
            }

            if (SearchExperimentTrial.activeTrial != null)
            {
                GameObject targetGO = SearchExperimentTrial.targetInteractable.gameObject;
                if (searchExperimentLevel.selectStatusDict.ContainsKey(targetGO.GetComponent<Interactable>()))
                {
                    //Debug.Log("Checking if main target is selected: " + targetGO.name);
                    if (searchExperimentLevel.selectStatusDict[targetGO.GetComponent<Interactable>()] == false)
                    {
                        //Debug.Log("Main target not selected!");
                        all_targets_selected = false;
                    }
                    else
                    {
                        //Debug.Log("Main target selected!");
                    }
                }
                else
                {
                    all_targets_selected = false;
                }

                SearchTargetInteractable[] allTargetInteractables = FindObjectsOfType<SearchTargetInteractable>();
                foreach (SearchTargetInteractable targetInteractable in allTargetInteractables)
                {
                    //if (targetInteractable.gameObject.tag == "temporary targets")
                    if (targetInteractable.gameObject.GetComponent<TemporaryTarget>() != null)
                    {
                        //Debug.Log("Checking if temp. target is selected: " + targetInteractable.gameObject.name);
                        GameObject clonetargetGO = targetInteractable.gameObject;
                        //Debug.Log(clonetargetGO.transform.position);
                        if (searchExperimentLevel.selectStatusDict.ContainsKey(clonetargetGO.GetComponent<Interactable>()))
                        {
                            if (searchExperimentLevel.selectStatusDict[clonetargetGO.GetComponent<Interactable>()] == false)
                            {
                                all_targets_selected = false;
                                Debug.Log("Temp. target not selected!");
                            }
                            else
                            {
                                Debug.Log("Temp. target is selected!");
                            }
                        }
                        else
                        {
                            all_targets_selected = false;
                            Debug.Log("selectStatusDict does not contain temp target key");
                        }
                    }

                }
            }


            foreach (Interactable go in FindObjectsOfType<Interactable>())
            {
                if (searchExperimentLevel.selectStatusDict.ContainsKey(go))
                {
                    if (searchExperimentLevel.selectStatusDict[go] == true)
                    {
                        has_selection = true;
                        if (!go.ValidateSelection())
                        {
                            select_correct = false;
                        }
                    }
                }
                else
                {
                    searchExperimentLevel.selectStatusDict[go] = false;
                }
            }
            UnityEngine.Debug.Log("Secondary button pressed!");
            if (has_selection && select_correct && all_targets_selected)
            {
                if (ExperimentTrial.activeTrial != null)
                {
                    ExperimentTrial.activeTrial.RecordTargetHit();
                    GetComponent<Object_collected>().ResetGameObject();
                }
                if (SearchExperimentTrial.activeTrial != null)
                {
                    SearchExperimentTrial.activeTrial.RecordTargetHit();
                    if (GetComponent<Object_collected>() != null)
                    {
                        GetComponent<Object_collected>().ResetGameObject();
                    }
                }
            }
            //else if (has_selection && !(select_correct))
            else if (has_selection && !(select_correct && all_targets_selected))
            {
                if (ExperimentTrial.activeTrial != null)
                {
                    ExperimentTrial.activeTrial.RecordTargetMiss();
                    GetComponent<Object_collected>().ResetGameObject();
                }
                if (SearchExperimentTrial.activeTrial != null)
                {
                    SearchExperimentTrial.activeTrial.RecordTargetMiss();
                    if (GetComponent<Object_collected>() != null)
                    {
                        GetComponent<Object_collected>().ResetGameObject();
                    }
                }
            }
            else
            {
                UnityEngine.Debug.LogError("None objects selected!");
            }
        }
    }

}
