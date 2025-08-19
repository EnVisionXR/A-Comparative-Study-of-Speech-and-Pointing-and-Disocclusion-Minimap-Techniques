using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class SearchExperimentManager : MonoBehaviour
{
    public enum ExperimentState
    { Idle, BetweenLevels, RunningLevel }

    [Header("Experiment Settings")]
    [SerializeField] private string subjectId = "-1";

    [SerializeField] private SelectionTechniqueManager.SelectionTechnique selectionTechnique;

    [SerializeField] private int randomSeed = 1234;

    [SerializeField] public int numberOfTargets = 1;

    [Range(1, 3)]
    [SerializeField] public int perplexityLevelInt = 1;

    [SerializeField] public string azureCLUKey = "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6InEtMjNmYWxldlpoaEQzaG05Q1Fia1A1TVF5VSIsImtpZCI6InEtMjNmYWxldlpoaEQzaG05Q1Fia1A1TVF5VSJ9.eyJhdWQiOiJodHRwczovL2NvZ25pdGl2ZXNlcnZpY2VzLmF6dXJlLmNvbSIsImlzcyI6Imh0dHBzOi8vc3RzLndpbmRvd3MubmV0LzQ5YTUwNDQ1LWJkZmEtNGI3OS1hZGUzLTU0N2I0ZjM5ODZlOS8iLCJpYXQiOjE3MTQxMjczODgsIm5iZiI6MTcxNDEyNzM4OCwiZXhwIjoxNzE0MTMyODcxLCJhY3IiOiIxIiwiYWlvIjoiQVlRQWUvOFdBQUFBOTY5VjltUDZtamFlcGdXcWNuUW1nS09XcCtZUFdORUtsek5LM1MvWTFTRlBncmphd24zY3NJOHY1eTBlSjhRb1FDVnFHbk5pZG80ajM4VE1sTHlrc0tZNXNJTUZpbE01Y2dLRGVxQVJvMjg0Z2dCK1RZNmRvZjM2dFlTeHZzb2FmR3ZWZXB3UGZOWU9jTnJxMTArV0V6OXFtaFphVnZHVnN5RmhpZ25NeEJRPSIsImFtciI6WyJwd2QiLCJtZmEiXSwiYXBwaWQiOiI4ODk0NWRiMC0zYzBlLTQyZGYtOTRmYS01ODVkMWFkNGFhMjAiLCJhcHBpZGFjciI6IjIiLCJmYW1pbHlfbmFtZSI6IkNoZW4iLCJnaXZlbl9uYW1lIjoiSnVubG9uZyIsImdyb3VwcyI6WyIxYjI3MDgwNC1lYzg1LTQxNzEtODcxOC1lZGQwZGNkYTVmN2QiLCI5MWY4MmMwNS02ZDQ0LTQ1ZmEtYTVkZS1lOWFkYjQ0ZDExNTAiLCIzMWJlYWQwOS05YmRhLTQxZDgtODBjOS03YTk5MTFmMzhkMWEiLCIxNTEzM2MwYy02NzczLTQ0OTktOTQzMC0wMmFkNzgzOWQ2MDMiLCI4YzY4M2UxNC1mOWIyLTRkYjUtYmJhOC1kZmNjNzM5OGVhMDciLCJlYWNiNmIxNS01MDRhLTQ0MDUtODljNi0yYTJiZDg3OWFmYzAiLCI1ODcwNWMxNy0zNTBlLTRjNWYtYTljZC1jOGIzYjJhOTUyMDgiLCI2ZGUyZDkxOC01YWIxLTQ2N2UtOWM5NC0xM2EzODNiMTdmZjciLCIyMWVhM2UyMy0yYjY1LTRlOWQtOTYyMC05OWM2YzU4MjJkY2YiLCIxYjhiNDMyZS04ZjQyLTQ5Y2ItODlhNS01MTkzYzVhZDk2NWQiLCJiN2EwZjkzMi01OTY0LTQxYjItOWJiMC05YjhjYWRmNmI5OTkiLCJiZGIyYmMzOC0zZGMwLTQ1YjItYWQ5NC0wZjcwMTMyMzc1NWUiLCI5N2Q5MmIzOS0yYWUzLTQ5YTYtOTBjOC05OGQ1YjU5ZmUzOGEiLCIwMzI4MzUzOS04MjcxLTQ2YmMtOGViNS01NDE2OGM5MGI4ZTgiLCIyZDRjODI0MS1hNmExLTQ5MWUtYjgxYi0zMzVlYTRhN2Q1MjkiLCJkY2VlNzM0Ni1kNGRlLTQ2N2UtYjBiZi0zZWQ0NmFiMzU2MzIiLCI4MjA5OWM0Yi1jYTZiLTQ5MmUtOGYxMS01N2ZlMjkwM2I1NzgiLCJjNTU1MmY1MS04M2RmLTQ3MGMtYmFkNi1kMjgwMjViOWI0YjIiLCI4YmI5YWE3MS1iOTFkLTRlZWMtYTM0ZC04OWRlZTY3MWY5ODciLCJlZmU5NWQ3My01MWE4LTRlZGQtOGRhMy1lNWI5YTZjMTdjOTEiLCJjMjVlYzE3Yy01Y2RhLTQ3ODMtYWI1OS05OWMyMGU4M2JhYmIiLCI0YjRkYWQ3Zi0yMGRkLTQ4NmQtOGIwNC04MDQwYmFmZjdiMzgiLCI4NDIxNWQ4MS01MGQ4LTQ0NTktYmM4Ni1hMmY5MjI0MDk1YTEiLCJjYzJjZGQ4Yi1lYWNlLTRhNGItYTk1MC05Yjk4OWExODNiOTciLCIwZGNmMTk5Ni0zZDZhLTQ0YzAtYjE1NC1lMTI3YzEyNDIzNzAiLCJmYWU3YzFhZi04MzRiLTQ1NDUtOGJmOS1iNWYxZGQwZDUyZjciLCJlZTRjMDZiZi1kOTNmLTQxNmMtOWJlNC1iY2ZhOWU1MjY2YzEiLCJjNGVlN2NkMS1mZTAyLTRjYTctOTFjYS1lNzdkYTc1NjI1YjUiLCI2OThhOTNmYi0wNTNhLTRhYTktODBkMy1mYWI1NGI2NGQzMWEiLCIwY2JjZDdmYi0xZjE3LTQ4ZmMtYWMzZS00YTIyMTMxZmE5MmQiLCI2NzdhMGVmZS0xZWQ4LTRlZDYtOWZlNC04N2M3NTk1YzI2MGIiXSwiaXBhZGRyIjoiMTMxLjExMS41LjE0NyIsIm5hbWUiOiJKdW5sb25nIENoZW4iLCJvaWQiOiJmZTlhOGI1MC01NTU0LTQ3M2MtYTVmOS00YjQ2NjFiMjA5ZjQiLCJvbnByZW1fc2lkIjoiUy0xLTUtMjEtMzE0Mjk0MjY1Ni00MTU3MTk3NzM5LTI4MjA5MDU5MDktMjgzOTEzIiwicHVpZCI6IjEwMDMyMDAyMEMwNTczODMiLCJyaCI6IjAuQVVjQVJRU2xTZnE5ZVV1dDQxUjdUem1HNlpBaU1YM0lLRHhIb08yT1UzU2JiVzFIQUQ0LiIsInNjcCI6InVzZXJfaW1wZXJzb25hdGlvbiIsInN1YiI6IldqYTlwRm56U2RjY2NxN3JmRlRXNjYyc2Mwa1ZFcmVESEZQdE9ZNlJMTXMiLCJ0aWQiOiI0OWE1MDQ0NS1iZGZhLTRiNzktYWRlMy01NDdiNGYzOTg2ZTkiLCJ1bmlxdWVfbmFtZSI6ImpjMjM3NUBjYW0uYWMudWsiLCJ1cG4iOiJqYzIzNzVAY2FtLmFjLnVrIiwidXRpIjoiTnRrYUg1TUltVS1Gekc0TzV3Zy1BUSIsInZlciI6IjEuMCIsIndpZHMiOlsiYjc5ZmJmNGQtM2VmOS00Njg5LTgxNDMtNzZiMTk0ZTg1NTA5Il19.odXZe6AxztsAlpcezL3_aa_m7GPrIReSkaljlW1BGyxn9dgNoAaIbaJNV7qNszyOTphaREUE6iPf8tJOUwS6au-13CZ-XtI4xKSPUiF3uYQhPe6XslVjyqgPA3kKPu0ifQWjcanUoxUECJJR9eU0cMU0AryZYT0WLRJU0YJwUiSNwGTYDkBC26H0E8Is_JEzRhmflE6ysmZStxG1mQwvbhyr8geBOGk19xx7JgvQFiT3ljcblKwJToySZ3V76992hDZzH-mJu-TNGEvFw9SpF6kb4Z-gTixEJLIo68vqKJ6X1UXzmaY5hSNKLB9i357WUAgHg8mkyVPCjP30h3iVZw";

    [Range(0, 15)]
    [SerializeField] private float pauseBetweenLevelsDuration = 4f;

    [SerializeField] private int numTrialsPerLevel = 10;

    

    [Header("Current Experiment Status")]
    [ReadOnly] public static ExperimentState state = ExperimentState.Idle;

    [ReadOnly][SerializeField] private int numRemainingLevels = -1;

    [ReadOnly][SerializeField] private float pauseTimeRemaining = -1f;

    private Queue<SearchExperimentLevel> remainingLevels;
    private List<SearchExperimentLevel> finishedLevels;
    private SearchExperimentLevel currentLevel;

    

    [SerializeField] private TMP_Text experimentText;
    public HideViewOfSpheresController Mimir;

    private void Start()
    {
        SearchExperimentLogger.softwareStartTime = (int)DateTimeOffset.Now.ToUnixTimeSeconds();
        SearchExperimentLogger.CreateLoggingDirectory(Application.streamingAssetsPath, "density_data");
        
    }

    public void SkipTrial()
    {
        if (currentLevel)
        {
            currentLevel.TransitionToBeforeTrial();
        }
    }


    public void ClearExperiment()
    {
        if (Application.IsPlaying(gameObject) && state != ExperimentState.Idle)
        { return; }

        // Remove all Experiment Levels that might have stayed from the editor
        gameObject.GetComponents<SearchExperimentLevel>().ToList().ForEach(x => DestroyImmediate(x));

        // Reset the states and other values that might have been modified through editor
        state = ExperimentState.Idle;
    }

    public void StartExperiment()
    {
        if (state != ExperimentState.Idle) { return; }
        ClearExperiment();

        SearchExperimentLogger.subjectId = subjectId;

        List<SearchExperimentLevel> levels = new List<SearchExperimentLevel>();

        SearchExperimentLevel level = gameObject.AddComponent<SearchExperimentLevel>();
        level.levelTechnique = selectionTechnique;
        level.levelDensity = perplexityLevelInt;
        levels.Add(level);

        level.selectStatusDict = new Dictionary<Interactable, bool>();

        //foreach (int densityLevel in LevelManager.densityLevelIntegers)
        //    {
        //    SearchExperimentLevel level = gameObject.AddComponent<SearchExperimentLevel>();
        //    level.levelTechnique = selectionTechnique;
        //    level.levelDensity = densityLevel;
        //    levels.Add(level);
        //}

        // shuffle the levels using linq
        levels = levels.OrderBy(a => Guid.NewGuid()).ToList();

        remainingLevels = new Queue<SearchExperimentLevel>(levels);
        finishedLevels = new List<SearchExperimentLevel>();

        print($"===> Experiment START <===");
        print($"Will run {remainingLevels.Count} levels");

        SetAllowSwitching(false);

        TransitionToPause();
    }

    private void TransitionToNextLevel()
    {
        if (currentLevel)
        {
            finishedLevels.Add(currentLevel);
        }

        if (remainingLevels.Count == 0)
        {
            SetAllowSwitching(true);
            currentLevel = null;
            state = ExperimentState.Idle;
            print("===> Experiment END <===");
            return;
        }

        currentLevel = remainingLevels.Dequeue();
        currentLevel.StartLevel(randomSeed, numTrialsPerLevel, numberOfTargets);
        state = ExperimentState.RunningLevel;
    }

    private void TransitionToPause()
    {
        if (remainingLevels.Count != 0)
            Mimir.ShowTheBarrier();
        pauseTimeRemaining = pauseBetweenLevelsDuration;
        state = ExperimentState.BetweenLevels;
    }

    private void SetAllowSwitching(bool value)
    {
        LevelManager.allowKeyLevelSwitching = value;
        SelectionTechniqueManager.allowKeySelectionTechniqueSwitching = value;
    }

    private void Update()
    {
        switch (state)
        {
            case ExperimentState.Idle:
                break;

            case ExperimentState.RunningLevel:

                if (currentLevel.state == SearchExperimentLevel.ExperimentLevelState.Finished)
                    TransitionToPause();

                numRemainingLevels = remainingLevels.Count;

                break;

            case ExperimentState.BetweenLevels:

                experimentText.text = $"Level ready in:\n{pauseTimeRemaining:0.#} s.";

                pauseTimeRemaining -= Time.deltaTime;
                if (pauseTimeRemaining < 0f)
                    TransitionToNextLevel();

                break;
        }
    }
}