using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;

public class ReadLogMessage : MonoBehaviour
{
    SpeechSynthesis SpeechSynthesis;
    public string output = "";
    public string stack = "";

    void OnEnable()
    {
        SpeechSynthesis = GetComponent<SpeechSynthesis>();
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    async void HandleLog(string logString, string stackTrace, LogType type)
    {
        output = logString;
        if (type == LogType.Log)
        {
            SpeechSynthesis.SpeakText(logString);
            Debug.LogError(logString);
        }
        else if (type == LogType.Warning)
        {
            if (logString.Substring(0, 16) == "Selected Objects")
            {
                string selectedObjects = logString.Substring(17, logString.Length - 1);
                var selectObjectList = JsonConvert.DeserializeObject<List<string>>(selectedObjects);
                
                foreach (string selectedObject in selectObjectList)
                {
                    //Debug.Log("Select:" + selectedObject);
                    GameObject gameObject = GameObject.Find(selectedObject);
                    if (gameObject.GetComponent<Outline>() == null)
                    {
                        Outline outline = gameObject.AddComponent<Outline>();

                        outline.OutlineMode = Outline.Mode.OutlineAll;
                        outline.OutlineColor = Color.red;
                        outline.OutlineWidth = 10f;
                        await Task.Delay(5000);
                        Destroy(outline);
                    }
                }
                //var outline = gameObject.AddComponent<Outline>();

                //outline.OutlineMode = Outline.Mode.OutlineAll;
                //outline.OutlineColor = Color.yellow;
                //outline.OutlineWidth = 5f;
            }
            

            //SpeechSynthesis.SpeakText(logString);
            //Debug.LogError(logString);
        }
        
        stack = stackTrace;
    }
}