using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    SearchExperimentManager searchExperimentManager;
    public static bool allowKeyLevelSwitching = true;
    private int densityLevel = 0;
    public static readonly List<int> densityLevelIntegers = new List<int> { };
    //public static readonly List<int> densityLevelIntegers = new List<int> { 3 };
    //public static readonly List<int> densityLevelIntegers = new List<int> { 1, 2, 3 };

    [SerializeField] private List<GameObject> densityLevels;
    [SerializeField] private SelectionTechniqueManager techniqueDistributer;

    public GrabbingHand grabbingHand;

    private void Start()
    {
        searchExperimentManager = FindObjectOfType<SearchExperimentManager>();
        densityLevel = searchExperimentManager.perplexityLevelInt;
        densityLevelIntegers.Add(densityLevel);
        //densityLevelIntegers.Add();
        DisableAllLevels();
    }

    private void Update()
    {
        if (allowKeyLevelSwitching && Input.GetKeyDown(KeyCode.Space))
        {
            densityLevel = (densityLevel + 1) % 4;

            EnableDensityLevel(densityLevel);
        }
    }

    public void EnableDensityLevel(int lvl)
    {
        techniqueDistributer.DisableAllTechniques();
        ResetSceneInteractables();

        if (lvl == 0)
        {
            DisableAllLevels();
            return;
        }

        //if (!densityLevelIntegers.Contains(lvl))
        //{
        //    throw new System.Exception($"Only density levels of {densityLevelIntegers}");
        //}

        // backup reference that allows having all the levels
        //for (int i = 0; i < lvl; i++)
        //{
        //    densityLevels[i].SetActive(true);
        //}
        densityLevels[lvl-1].SetActive(true);
    }

    public void DisableAllLevels()
    {
        densityLevels.ForEach(x => x.SetActive(false));
    }

    private void ResetSceneInteractables()
    {
        grabbingHand.ClearGrabbed();
    }
}