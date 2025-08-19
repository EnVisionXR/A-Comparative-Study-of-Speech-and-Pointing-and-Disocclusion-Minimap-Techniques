using UnityEngine;
//using UnityEditor.Scripting.Python;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System.Diagnostics;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using UnityEngine.XR.Interaction.Toolkit;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Text;
using UnityEngine.Networking;
//using UnityEngine.Rendering.HighDefinition;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;
using UnityEngine.InputSystem;
//using UnityEngine.Windows;
//using Unity.VisualScripting.YamlDotNet.Core.Tokens;
//using System.Diagnostics;

// Azure Speech-to-text

public class SpeechRecognition : MonoBehaviour
{
    SearchExperimentManager searchExperimentManager;
    DraggablePanel draggablePanel;
    SearchExperimentLevel searchExperimentLevel;
    //AzureCLU AzureCLU;
    ShapeAndColorChanger shapeAndColorChanger;
    RayHit RayHit;
    SpeechSynthesis speechSynthesis;
    //PostRequest PostRequest;
    private Camera mainCamera;
    [SerializeField] private InputActionReference primaryButtonPressed;
    [SerializeField] private InputActionReference secondaryButtonPressed;
    //private GameObject CameraController;
    private GameObject AudioSourceObject;
    private AudioSource audioSource;
    static public bool sceneDescript;
    static public bool prevSceneDescriptState_;
    static public bool mainObjectActivate;
    static public bool prevMainObjectState_;
    static public bool searchObjectActivate;
    static public bool prevSearchObjectState;
    static public bool cancelActivate;
    static public bool prevCancelState_;
    static public string match_obj_string;
    public Dictionary<string, string> objectAppearance;
    private bool prevPrimaryButtonState_ = false;
    private bool primaryButtonDown;
    //public XRController rightHand;
    public ActionBasedController rightHand;
    public InputHelpers.Button primaryButton;
    private float doubleclickDelay = 0.5f, passedTimeSinceLastClick;
    private int unrecognizeCount = 0;
    private bool previouslyPressed = false;
    

    public void Start()
    {
        searchExperimentManager = FindObjectOfType<SearchExperimentManager>();
        draggablePanel = FindObjectOfType<DraggablePanel>();
        searchExperimentLevel = FindObjectOfType<SearchExperimentLevel>();
        //primaryButtonActionReference = rightHand.interactions.FirstOrDefault(i => i.name == "ActivateSpeech");
        sceneDescript = false;
        prevSceneDescriptState_ = false;
        mainObjectActivate = false;
        prevMainObjectState_ = false;
        searchObjectActivate = false;
        prevSearchObjectState = false;
        cancelActivate = true;
        prevCancelState_ = false;
        audioSource = GetComponent<AudioSource>();
        RayHit = GetComponent<RayHit>();
        shapeAndColorChanger = GetComponent<ShapeAndColorChanger>();
        //AzureCLU = GetComponent<AzureCLU>();
        //PostRequest = GetComponent<PostRequest>();
        mainCamera = Camera.main;
        //CameraController = GameObject.Find("CameraController");
        //AudioSourceObject = GameObject.Find("AudioSourceObject");
        //cameraFieldOfView = GetComponent<CameraFieldOfView>();
        speechSynthesis = GetComponent<SpeechSynthesis>();
        //audioSource.transform.position = mainCamera.transform.position;
        ParseSceneGraph();

        // Initially no objects are selected
        foreach (Interactable go in FindObjectsOfType<Interactable>())
        {
            searchExperimentLevel.selectStatusDict[go] = false;
        }

    }
    private async void Update()
    {
        //audioSource.transform.position = mainCamera.transform.position;
        //audioSource.spatialBlend = 0;
        passedTimeSinceLastClick += Time.deltaTime;
        primaryButtonDown = false;//OVRInput.Get(OVRInput.Button.One);
        //rightHand.inputDevice.IsPressed(primaryButton, out primaryButtonDown);
        //primaryButtonDown = rightHand.selectAction.action.IsPressed();
        //if (rightHand != null && rightHand.isActiveAndEnabled && primaryButtonActionReference != null)
        //{
        //    // Check if the primary button is pressed
        //    if (primaryButtonActionReference.action.IsPressed())
        //    {
        //        UnityEngine.Debug.Log("Primary button pressed!");
        //        // Add your custom logic here
        //    }
        //}
        //if (primaryButtonPressed.action.WasPressedThisFrame())
        //{
        //    UnityEngine.Debug.Log("Primary button pressed!");
        //}
        //UnityEngine.Debug.Log("Primary Button Down: " + primaryButtonDown);
        prevSceneDescriptState_ = sceneDescript;
        prevMainObjectState_ = mainObjectActivate;
        prevSearchObjectState = searchObjectActivate;
        prevCancelState_ = cancelActivate;

        //if (Input.GetKeyDown(KeyCode.C))
        //if (primaryButtonDown && !prevPrimaryButtonState_)
        if (primaryButtonPressed.action.WasPressedThisFrame())
        {
            if (previouslyPressed)
            {
                //pressedFirstTime = false;
                if (passedTimeSinceLastClick < doubleclickDelay)
                {
                    DoubleClick();
                    previouslyPressed = false;
                    passedTimeSinceLastClick = 0;
                }
            }
            else
            {
                previouslyPressed = true;
                passedTimeSinceLastClick = 0;
            }
        }

        if (secondaryButtonPressed.action.WasPressedThisFrame())
        {
            bool select_correct = true;
            bool has_selection = false;
            bool all_targets_selected = true;
            searchExperimentLevel = FindObjectOfType<SearchExperimentLevel>();

            if (ExperimentTrial.activeTrial != null)
            {
                GameObject targetGO = ExperimentTrial.targetInteractable.gameObject;
                if (searchExperimentLevel.selectStatusDict.ContainsKey(targetGO.GetComponent<Interactable>()))
                {
                    if (searchExperimentLevel.selectStatusDict[targetGO.GetComponent<Interactable>()] == false)
                    {
                        all_targets_selected = false;
                    }
                    else
                    {
                    }
                }
                else
                {
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
                            }
                        }
                        else
                        {
                        }
                    }
                }
            }

            if (SearchExperimentTrial.activeTrial != null)
            {
                GameObject targetGO = SearchExperimentTrial.targetInteractable.gameObject;
                if (searchExperimentLevel.selectStatusDict.ContainsKey(targetGO.GetComponent<Interactable>()))
                {
                    if (searchExperimentLevel.selectStatusDict[targetGO.GetComponent<Interactable>()] == false)
                    {
                        all_targets_selected = false;
                    }
                    else
                    {
                    }
                }
                else
                {
                }

                SearchTargetInteractable[] allTargetInteractables = FindObjectsOfType<SearchTargetInteractable>();
                foreach (SearchTargetInteractable targetInteractable in allTargetInteractables)
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
                            }
                            else
                            {
                            }
                        }
                        else
                        {
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
        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    await AzureCLU.StartAsync();
        //}



        //if (Input.GetKeyDown(KeyCode.S))
        if (previouslyPressed && passedTimeSinceLastClick > doubleclickDelay)
        {
            SingleClick();
            previouslyPressed = false;
        }

        sceneDescript = false;
        mainObjectActivate = false;
        searchObjectActivate = false;
        prevPrimaryButtonState_ = primaryButtonDown;
    }

    async public void SingleClick()
    {
        //foreach (KeyValuePair<string, string> entry in objectAppearance)
        //{
        //    UnityEngine.Debug.LogError("Object Appearance: " + entry.Key + entry.Value);
        //}

        UnityEngine.Debug.LogError("Single click detected, starting voice recognition...");
        cancelActivate = false;
        UnityEngine.Debug.LogError("Speak into your microphone.");
        //PythonRunner.RunFile($"{Application.dataPath}/chatgpt-retrieval/chatgpt.py", "Test Input to Python Script");

        // Attach the audio source to the new game object
        AudioClip clip = Resources.Load<AudioClip>("VA_activate");
        audioSource.clip = clip;

        // Play the audio clip
        audioSource.Play();

        //cancelActivate = true;

        var speechConfig = SpeechConfig.FromSubscription("XXXX", "uksouth");
        speechConfig.SpeechRecognitionLanguage = "en-US";
        using var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
        SpeechRecognizer speechRecognizer = new SpeechRecognizer(speechConfig, audioConfig);
        SpeechToText(speechRecognizer);
    }

    public void DoubleClick()
    {
        // Cancel selection of all objects
        foreach (Interactable go in FindObjectsOfType<Interactable>())
        {
            searchExperimentLevel.selectStatusDict[go] = false;
        }
        UnityEngine.Debug.LogError("Double click detected, cancelling audio...");
        UnityEngine.Debug.LogError("Double click detected");
        UnityEngine.Debug.LogError("Cancel");
        UnityEngine.Debug.LogError("Stopped all audio sources in the scene.");
        cancelActivate = true;
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource audioSource in allAudioSources)
        {
            if (audioSource.gameObject == AudioSourceObject)
            {
                audioSource.Stop();
            }
        }
        //foreach (AudioSource audioSource in allAudioSources)
        //{
        //    if (audioSource.gameObject == CameraController || audioSource.gameObject == AudioSourceObject)
        //    {
        //        audioSource.Stop();
        //    }
        //}
        AudioClip clip_cancel = Resources.Load<AudioClip>("Cancel_success");
        audioSource.clip = clip_cancel;
        // Play the audio clip
        audioSource.Play();
    }

    public async Task SpeechToText(SpeechRecognizer speechRecognizer)
    {
        var speechRecognitionResult = await speechRecognizer.RecognizeOnceAsync();
        //OutputSpeechRecognitionResult(speechRecognitionResult);
        StartCoroutine(OutputSpeechRecognitionResult(speechRecognitionResult));
    }


    //public async void OutputSpeechRecognitionResult(SpeechRecognitionResult speechRecognitionResult)
    public IEnumerator OutputSpeechRecognitionResult(SpeechRecognitionResult speechRecognitionResult)
    {
        switch (speechRecognitionResult.Reason)
        {
            case ResultReason.RecognizedSpeech:
                string userQuery = speechRecognitionResult.Text;
                if (userQuery.Contains("this") == true && RayHit.hitObjectName != "")
                {
                    userQuery = userQuery.Replace("this", RayHit.hitObjectName);
                }
                UnityEngine.Debug.LogError($"RECOGNIZED: Text={speechRecognitionResult.Text}");
                draggablePanel.SetSpeechFeedback("Recognized: " + speechRecognitionResult.Text);
                // Attach the audio source to the new game object
                AudioClip clip_stop = Resources.Load<AudioClip>("VA_stop");
                audioSource.clip = clip_stop;
                // Play the audio clip
                audioSource.Play();

                //await Task.Delay(100);

                string url = "https://cognitivelanguage0122.cognitiveservices.azure.com/language/:analyze-conversations?api-version=2022-10-01-preview";
                string bodyJsonString = "{\"kind\":\"Conversation\",\"analysisInput\":{\"conversationItem\":{\"id\":\"PARTICIPANT_ID_HERE\",\"text\":\"" + userQuery + "\",\"modality\":\"text\",\"language\":\"en\",\"participantId\":\"PARTICIPANT_ID_HERE\"}},\"parameters\":{\"projectName\":\"AIGC4XR-UserStudy\",\"verbose\":true,\"deploymentName\":\"UserStudy-0501\",\"stringIndexType\":\"TextElement_V8\"}}";
                //string bodyJsonString = "{\"kind\":\"Conversation\",\"analysisInput\":{\"conversationItem\":{\"id\":\"PARTICIPANT_ID_HERE\",\"text\":\"" + userQuery + "\",\"modality\":\"text\",\"language\":\"en\",\"participantId\":\"PARTICIPANT_ID_HERE\"}},\"parameters\":{\"projectName\":\"UserQueryUnderstanding\",\"verbose\":true,\"deploymentName\":\"SceneEdit-0121\",\"stringIndexType\":\"TextElement_V8\"}}";
                //CoroutineWithData cd = new CoroutineWithData(this, PostRequest.Post(url, bodyJsonString));
                //yield return cd.coroutine;
                //UnityEngine.Debug.Log("result is " + cd.result);  //  'success' or 'fail'

                var request = new UnityWebRequest(url, "POST");
                byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
                request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("authorization", searchExperimentManager.azureCLUKey);
                //request.SetRequestHeader("authorization", "Bearer XXX");
                request.SetRequestHeader("Apim-Request-Id", "4ffcac1c-b2fc-48ba-bd6d-b69d9942995a");
                yield return request.SendWebRequest();
                UnityEngine.Debug.LogError("Status Code: " + request.responseCode);

                UnityEngine.Debug.LogError(request.downloadHandler.text);
                JObject response = JObject.Parse(request.downloadHandler.text);
                var topIntent = response["result"]["prediction"]["topIntent"];
                string topIntent_string = topIntent.ToString();
                UnityEngine.Debug.LogError("Top Intent: " + topIntent);

                if (topIntent_string == "CancelAll")
                {
                    foreach (Interactable go in FindObjectsOfType<Interactable>())
                    {
                        if (go.gameObject.GetComponent<Outline>() != null)
                        {
                            Destroy(go.gameObject.GetComponent<Outline>());
                        }
                        searchExperimentLevel.selectStatusDict[go] = false;
                    }
                    //foreach (KeyValuePair<Interactable, bool> kvp in searchExperimentLevel.selectStatusDict)
                    //{
                    //    searchExperimentLevel.selectStatusDict[kvp.Key] = false;
                    //}
                }

                if (topIntent_string == "ModifyAppearance")
                {
                    var entities = response["result"]["prediction"]["entities"];
                    string originalColor_text = "";
                    string originalShape_text = "";
                    string targetColor_text = "";
                    string targetShape_text = "";
                    //JToken objectOfInterest_jtoken = entities.SelectToken("$[?(@.category == 'ObjectOfInterest')].text");
                    //string objectOfInterest_text = objectOfInterest_jtoken.ToString();
                    JToken originalColor = entities.SelectToken("$[?(@.category == 'Original Color')].text");
                    if (originalColor != null)
                        originalColor_text = originalColor.ToString();
                    UnityEngine.Debug.LogError("Original Color: " + originalColor);

                    JToken originalShape = entities.SelectToken("$[?(@.category == 'Original Shape')].text");
                    UnityEngine.Debug.LogError("Original Shape: " + originalShape);
                    if (originalShape != null)
                        originalShape_text = originalShape.ToString();

                    JToken targetColor = entities.SelectToken("$[?(@.category == 'Target Color')].text");
                    UnityEngine.Debug.LogError("Target Color: " + targetColor);
                    if (targetColor != null)
                        targetColor_text = targetColor.ToString();

                    JToken targetShape = entities.SelectToken("$[?(@.category == 'Target Shape')].text");
                    UnityEngine.Debug.LogError("Target Shape: " + targetShape);
                    if (targetShape != null)
                        targetShape_text = targetShape.ToString();

                    if(targetColor != null | targetShape != null)
                    {
                        shapeAndColorChanger.ChangeShapeAndColor(originalColor_text, originalShape_text, targetColor_text, targetShape_text);
                    }
                    

                    //Interactable o.OnSelect();

                    //if (objectOfInterest_text == "all")
                    //{
                    //    List<GameObject> objectsOfInterest = new List<GameObject>();
                    //    foreach (KeyValuePair<string, string> entry in objectAppearance)
                    //    {
                    //        string[] words = entry.Value.Split(' ');

                    //        if (originalColor != null && words[0] == originalColor.ToString())
                    //        {
                    //            objectsOfInterest.Add(GameObject.Find(entry.Key));
                    //        }
                    //        else if (originalShape != null && words[1] == originalShape.ToString())
                    //        {
                    //            objectsOfInterest.Add(GameObject.Find(entry.Key));
                    //        }
                    //    }
                    //    foreach (GameObject objectOfInterest in objectsOfInterest)
                    //    {
                    //        string objectTargetColor = "";
                    //        string objectTargetMaterial = "";
                    //        UnityEngine.Debug.LogError("Changing object " + objectOfInterest.name);
                    //        string[] originalAppearance = objectOfInterest.GetComponent<Renderer>().material.ToString().Split(' ');

                    //        if (targetColor == null)
                    //        {
                    //            objectTargetColor = originalAppearance[0];
                    //        }
                    //        else
                    //        {
                    //            objectTargetColor = targetColor.ToString();
                    //        }

                    //        if (targetShape == null)
                    //        {
                    //            objectTargetMaterial = originalAppearance[1];
                    //        }
                    //        else
                    //        {
                    //            objectTargetMaterial = targetShape.ToString();
                    //        }

                    //        Material objecttargetAppearance = Resources.Load(objectTargetColor + " " + objectTargetMaterial, typeof(Material)) as Material;
                    //        objectOfInterest.GetComponent<Renderer>().material = objecttargetAppearance;
                    //    }
                    //}
                    //else
                    //{
                    //    GameObject objectOfInterest = GameObject.Find(objectOfInterest_text);
                    //    UnityEngine.Debug.LogError("Changing object " + objectOfInterest.name);
                    //    string[] originalAppearance = objectOfInterest.GetComponent<Renderer>().material.ToString().Split(' ');

                    //    if (targetColor == null)
                    //    {
                    //        targetColor = originalAppearance[0];
                    //    }

                    //    if (targetShape == null)
                    //    {
                    //        targetShape = originalAppearance[1];
                    //    }

                    //    Material targetAppearance = Resources.Load(targetColor + " " + targetShape, typeof(Material)) as Material;
                    //    objectOfInterest.GetComponent<Renderer>().material = targetAppearance;
                    //}





                }

                if (topIntent_string == "Select")
                {
                    UnityEngine.Debug.Log("Top Intent: Select");
                    var entities = response["result"]["prediction"]["entities"];
                    string originalColor_text = "";
                    string originalShape_text = "";
                    //JToken objectOfInterest_jtoken = entities.SelectToken("$[?(@.category == 'ObjectOfInterest')].text");
                    //string objectOfInterest_text = objectOfInterest_jtoken.ToString();
                    JToken originalColor = entities.SelectToken("$[?(@.category == 'Original Color')].text");
                    if (originalColor != null)
                    {
                        originalColor_text = originalColor.ToString();
                    }
                    
                    UnityEngine.Debug.LogError("Original Color: " + originalColor);

                    JToken originalShape = entities.SelectToken("$[?(@.category == 'Original Shape')].text");
                    if (originalShape != null)
                        originalShape_text = originalShape.ToString();
                    UnityEngine.Debug.LogError("Original Shape: " + originalShape);

                    if (originalColor != null | originalShape != null)
                    {
                        shapeAndColorChanger.SelectObject(originalColor_text, originalShape_text);
                    }


                    //if (RayHit.hasHit && RayHit.triggerButtonDown && !RayHit.prevTriggerDown)
                    //{
                    //    //Debug.Log("Detected trigger pressed!");
                    //    bool prevSelectStat = selectStatusDict[RayHit.hitObject];
                    //    if (prevSelectStat)
                    //    {
                    //        Outline outline = RayHit.hitObject.GetComponent<Outline>();
                    //        Destroy(outline);
                    //        selectStatusDict[RayHit.hitObject] = false;
                    //    }
                    //    else
                    //    {
                    //        Outline outline = RayHit.hitObject.AddComponent<Outline>();
                    //        outline.OutlineMode = Outline.Mode.OutlineAll;
                    //        outline.OutlineColor = UnityEngine.Color.green;
                    //        outline.OutlineWidth = 10f;
                    //        selectStatusDict[RayHit.hitObject] = true;
                    //    }
                    //}

                }

                //else
                //{
                //    PythonRunner.RunFile($"{Application.dataPath}/chatgpt-retrieval/chatgpt.py", userQuery);
                //}

                
                //var task = Task.Run(() => PythonRunner.RunFile($"{Application.dataPath}/chatgpt-retrieval/chatgpt.py", userQuery));
                //if (task.Wait(TimeSpan.FromSeconds(50)))
                //    return;
                //else
                //    throw new Exception("Timed out");
                

                break;
            case ResultReason.NoMatch:
                unrecognizeCount += 1;
                // Attach the audio source to the new game object
                AudioClip clip_fail2 = Resources.Load<AudioClip>("VA_fail");
                audioSource.clip = clip_fail2;
                // Play the audio clip
                audioSource.Play();

                UnityEngine.Debug.LogError($"NOMATCH: Speech could not be recognized.");
                draggablePanel.SetSpeechFeedback("Speech could not be recognized.");
                //speechSynthesis.SpeakText("Sorry, I didn't recognize your voice. Please try again.");
                cancelActivate = false;
                if (unrecognizeCount > 2)
                {
                    //speechSynthesis.SpeakText("Sorry, I didn't get that. Please try saying 'Where am I', 'What is near me', or 'Where is the <objectname>'.");
                    UnityEngine.Debug.LogError("Sorry, I didn't get that.");
                }
                break;
            case ResultReason.Canceled:
                var cancellation = CancellationDetails.FromResult(speechRecognitionResult);
                draggablePanel.SetSpeechFeedback($"Speech recognition cancelled. Reason: {cancellation.Reason}");
                UnityEngine.Debug.LogError($"CANCELED: Reason={cancellation.Reason}");
                //speechSynthesis.SpeakText("Sorry, speech recognition was cancelled. Please try again.");
                if (cancellation.Reason == CancellationReason.Error)
                {
                    //UnityEngine.Debug.Log($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                    UnityEngine.Debug.LogError($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                    UnityEngine.Debug.LogError($"CANCELED: Did you set the speech resource key and region values?");
                }
                break;
        }
        // sceneDescript = false;
        // prevSceneDescriptState_ = sceneDescript;
    }


    void ParseSceneGraph()
    {
        string jsonFilePath = Path.Combine(Application.dataPath, "scene_graph.json");
        //Debug.LogError("Importance Values Loaded from:" + jsonFilePath);

        if (File.Exists(jsonFilePath))
        {
            string jsonContent = File.ReadAllText(jsonFilePath);
            SceneGraph sceneGraphContent = JsonConvert.DeserializeObject<SceneGraph>(jsonContent);

            if (sceneGraphContent != null)
            {
                objectAppearance = new Dictionary<string, string>();
                
                TraverseSceneGraph(sceneGraphContent.children);

                UnityEngine.Debug.LogError("Importance values and descriptions loaded successfully.");
            }
            else
            {
                UnityEngine.Debug.LogError("Failed to deserialize JSON content.");
            }
        }
        else
        {
            UnityEngine.Debug.LogError("JSON file not found at path: " + jsonFilePath);
        }
    }

    void TraverseSceneGraph(List<SceneGraph> children)
    {
        if (children == null || children.Count == 0)
            return;

        foreach (SceneGraph child in children)
        {
            //List<ComponentData> components = child.components;
            bool isEmpty = !child.components.Any();
            //if (child.name != null)
            if (!isEmpty)
            {
                if (!objectAppearance.ContainsKey(child.name))
                {
                    objectAppearance.Add(child.name, child.components[0].appearance);
                    //UnityEngine.Debug.LogError(child.components);
                    //UnityEngine.Debug.LogError(child.components[0]);
                    //UnityEngine.Debug.LogError(child.components[0].position);
                    //UnityEngine.Debug.LogError(child.components[0].appearance);
                    //objectAppearance.Add(child.name, child.name);
                }
            }

            TraverseSceneGraph(child.children);
        }
    }

    public string GetAppearance(string objectName)
    {
        if (objectAppearance.ContainsKey(objectName))
        {
            return objectAppearance[objectName];
        }

        //Debug.LogWarning("Importance value not found for object: " + objectName);
        return "";
    }


    [System.Serializable]
    public class SceneGraph
    {
        public string name;
        //public string type;
        public List<SceneGraph> children;
        public List<ComponentData> components;
        //public string appearance;
        public string description;
    }

    [System.Serializable]
    public class ComponentData
    {
        public Dictionary<string, float> position;
        public Dictionary<string, float> rotation;
        public string appearance;
        // Add other component properties as needed
    }

    //[System.Serializable]
    //public class ComponentValue
    //{
    //    public string attribute;
    //    public string appearance;
    //    // Add other component properties as needed
    //}
}
