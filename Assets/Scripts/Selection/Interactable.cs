using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class Interactable : MonoBehaviour
{
    MiniMapInteractor miniMapInteractor;
    SearchExperimentLevel searchExperimentLevel;
    [SerializeField] private Material hoverMaterial;
    public cakeslice.Outline interactionOutline = null;

    private Material defaultMaterial;
    private List<MeshRenderer> meshRenderers;

    public bool debug = false;

    public static Component currentInteractor = null;

    private void Start()
    {
        miniMapInteractor = FindObjectOfType<MiniMapInteractor>();
        //GameObject searchExperimentObject = GameObject.Find("StudyManager");
        //if (searchExperimentObject != null)
        //{
        //    searchExperimentLevel = searchExperimentObject.GetComponent<SearchExperimentLevel>();
        //}
        //searchExperimentLevel = FindObjectOfType<SearchExperimentLevel>();
    }
    public Material GetDefaultMaterial()
    {
        return defaultMaterial;
    }

    private void Awake()
    {
        meshRenderers = new List<MeshRenderer>(GetComponents<MeshRenderer>());
        if (meshRenderers.Count == 0)
            meshRenderers = new List<MeshRenderer>(GetComponentsInChildren<MeshRenderer>());
        defaultMaterial = meshRenderers[0].material;
    }

    public void StartHover()
    {
        dprint($"Start hover: {this.name}");

        foreach (var mr in meshRenderers)
            mr.material = hoverMaterial;
    }

    public void EndHover()
    {
        dprint($"End hover: {this.name}");

        foreach (var mr in meshRenderers)
            mr.material = defaultMaterial;
    }

    public void dprint(string msg)
    {
        if (debug) print(msg);
    }

    public void SetHoverMaterial(Material mat)
    {
        hoverMaterial = mat;
    }

    public void UpdateSelectionDict(Interactable o)
    {
        GameObject searchExperimentObject = GameObject.Find("StudyManager");
        searchExperimentLevel = searchExperimentObject.GetComponent<SearchExperimentLevel>();
        Debug.Log(searchExperimentLevel.selectStatusDict);
        if (searchExperimentLevel.selectStatusDict.ContainsKey(o))
        {
            if (searchExperimentLevel.selectStatusDict[o] == true)
            {
                Debug.Log(o.gameObject.name + " Selection status: false, case 1");
                searchExperimentLevel.selectStatusDict[o] = false;
                if (o.gameObject.GetComponent<Outline>() != null)
                {
                    Destroy(o.gameObject.GetComponent<Outline>());
                }
                //Debug.Log(miniMapInteractor.originalToDuplicateMap.ContainsKey(o.gameObject));
                if (miniMapInteractor.originalToDuplicate.ContainsKey(o))
                {
                    if (miniMapInteractor.originalToDuplicate[o] != null)
                    {
                        shapeItem_2 shapeItem_2 = miniMapInteractor.originalToDuplicate[o];
                        if (shapeItem_2.gameObject.GetComponent<Outline>() != null)
                        {
                            //Debug.Log("shape2 removed highlight!");
                            Destroy(shapeItem_2.gameObject.GetComponent<Outline>());
                        }
                        if (miniMapInteractor.originalToDuplicate_ForCirCumference.ContainsKey(shapeItem_2))
                        {
                            if (miniMapInteractor.originalToDuplicate_ForCirCumference[shapeItem_2] != null)
                            {
                                shapeItem_3 shapeItem_3 = miniMapInteractor.originalToDuplicate_ForCirCumference[shapeItem_2];
                                if (shapeItem_3.gameObject.GetComponent<Outline>() != null)
                                {
                                    //Debug.Log("shape3 removed highlight!");
                                    Destroy(shapeItem_3.gameObject.GetComponent<Outline>());
                                }
                            }
                        }
                    }
                }
                
            }
            else
            {
                Debug.Log(o.gameObject.name + " Selection status: true, case 2");
                searchExperimentLevel.selectStatusDict[o] = true;
                if (o.gameObject.GetComponent<Outline>() == null)
                {
                    Outline outline = o.gameObject.AddComponent<Outline>();
                    outline.OutlineColor = Color.green;
                    outline.OutlineWidth = 15f;
                    outline.OutlineMode = Outline.Mode.OutlineVisible;
                }
                else
                {
                    o.gameObject.GetComponent<Outline>().OutlineColor = Color.green;
                }
                //Debug.Log(miniMapInteractor);
                //Debug.Log(miniMapInteractor.originalToDuplicateMap);
                //Debug.Log(miniMapInteractor.originalToDuplicate[o]);
                
                if (miniMapInteractor.originalToDuplicate.ContainsKey(o))
                {
                    //Debug.Log("case 0");
                    if (miniMapInteractor.originalToDuplicate[o] != null)
                    {
                        //Debug.Log("case 1");
                        shapeItem_2 shapeItem_2 = miniMapInteractor.originalToDuplicate[o];
                        if (shapeItem_2.gameObject.GetComponent<Outline>() == null)
                        {
                            //Debug.Log("case 2");
                            Outline outline = shapeItem_2.gameObject.AddComponent<Outline>();
                            outline.OutlineColor = Color.green;
                            outline.OutlineWidth = 15f;
                            outline.OutlineMode = Outline.Mode.OutlineVisible;
                        }
                        else
                        {
                            //Debug.Log("case 3");
                            shapeItem_2.gameObject.GetComponent<Outline>().enabled = true;
                            shapeItem_2.gameObject.GetComponent<Outline>().OutlineColor = Color.green;
                        }
                        if (miniMapInteractor.originalToDuplicate_ForCirCumference.ContainsKey(shapeItem_2))
                        {
                            if (miniMapInteractor.originalToDuplicate_ForCirCumference[shapeItem_2] != null)
                            {
                                shapeItem_3 shapeItem_3 = miniMapInteractor.originalToDuplicate_ForCirCumference[shapeItem_2];
                                if (shapeItem_3.gameObject.GetComponent<Outline>() == null)
                                {
                                    Outline outline = shapeItem_3.gameObject.AddComponent<Outline>();
                                    outline.OutlineColor = Color.green;
                                    outline.OutlineWidth = 15f;
                                    outline.OutlineMode = Outline.Mode.OutlineVisible;
                                }
                                else
                                {
                                    shapeItem_3.gameObject.GetComponent<Outline>().enabled = true;
                                    shapeItem_3.gameObject.GetComponent<Outline>().OutlineColor = Color.green;
                                }
                            }
                        }
                    }
                }
            }
        }
        else
        {
            searchExperimentLevel.selectStatusDict[o] = true;
            Debug.Log(o.gameObject.name + " Selection status: true, case 3" + o.gameObject.transform.position);
            Debug.Log(searchExperimentLevel.selectStatusDict[o]);
            if (o.gameObject.GetComponent<Outline>() == null)
            {
                Outline outline = o.gameObject.AddComponent<Outline>();
                outline.OutlineColor = Color.green;
                outline.OutlineWidth = 15f;
                outline.OutlineMode = Outline.Mode.OutlineVisible;
            }
            else
            {
                o.gameObject.GetComponent<Outline>().enabled = true;
                o.gameObject.GetComponent<Outline>().OutlineColor = Color.green;
            }
            if (miniMapInteractor.originalToDuplicate.ContainsKey(o))
            {
                if (miniMapInteractor.originalToDuplicate[o] != null)
                {
                    //Debug.Log("shape2 highlighted!");
                    shapeItem_2 shapeItem_2 = miniMapInteractor.originalToDuplicate[o];
                    if (shapeItem_2.gameObject.GetComponent<Outline>() == null)
                    {
                        Outline outline = shapeItem_2.gameObject.AddComponent<Outline>();
                        outline.OutlineColor = Color.green;
                        outline.OutlineWidth = 15f;
                        outline.OutlineMode = Outline.Mode.OutlineVisible;
                    }
                    else
                    {
                        shapeItem_2.gameObject.GetComponent<Outline>().enabled = true;
                        shapeItem_2.gameObject.GetComponent<Outline>().OutlineColor = Color.green;
                    }
                    if (miniMapInteractor.originalToDuplicate_ForCirCumference.ContainsKey(shapeItem_2))
                    {
                        if (miniMapInteractor.originalToDuplicate_ForCirCumference[shapeItem_2] != null)
                        {
                            //Debug.Log("shape3 highlighted!");
                            shapeItem_3 shapeItem_3 = miniMapInteractor.originalToDuplicate_ForCirCumference[shapeItem_2];
                            if (shapeItem_3.gameObject.GetComponent<Outline>() == null)
                            {
                                Outline outline = shapeItem_3.gameObject.AddComponent<Outline>();
                                outline.OutlineColor = Color.green;
                                outline.OutlineWidth = 15f;
                                outline.OutlineMode = Outline.Mode.OutlineVisible;
                            }
                            else
                            {
                                shapeItem_3.gameObject.GetComponent<Outline>().enabled = true;
                                shapeItem_3.gameObject.GetComponent<Outline>().OutlineColor = Color.green;
                            }
                        }
                    }
                }
            }
        }
    }

    public void OnSelect()
    {
        if (ExperimentTrial.activeTrial != null)
        {
            if (TryGetComponent(out TargetInteractable _))
            {
                ExperimentTrial.activeTrial.RecordTargetHit();
                GetComponent<Object_collected>().ResetGameObject();
            }
            else
            {
                ExperimentTrial.activeTrial.RecordTargetMiss();
                GetComponent<Object_collected>().ResetGameObject();
            }
        }

        if (SearchExperimentTrial.activeTrial != null)
        {
            if (TryGetComponent(out SearchTargetInteractable _))
            {
                SearchExperimentTrial.activeTrial.RecordTargetHit();
                GetComponent<Object_collected>().ResetGameObject();
            }
            else
            {
                SearchExperimentTrial.activeTrial.RecordTargetMiss();
                GetComponent<Object_collected>().ResetGameObject();
            }
        }
    }


    public bool ValidateSelection()
    {
        if (ExperimentTrial.activeTrial != null)
        {
            if (TryGetComponent(out TargetInteractable _))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        if (SearchExperimentTrial.activeTrial != null)
        {
            if (TryGetComponent(out SearchTargetInteractable _))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        return false;
    }
}