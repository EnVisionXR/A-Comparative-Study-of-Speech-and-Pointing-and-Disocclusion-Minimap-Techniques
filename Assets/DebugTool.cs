using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTool : MonoBehaviour
{
    SearchExperimentLevel searchExperimentLevel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            searchExperimentLevel = FindObjectOfType<SearchExperimentLevel>();
            Debug.Log(searchExperimentLevel);
            Debug.Log(searchExperimentLevel.selectStatusDict);
            foreach (KeyValuePair<Interactable, bool> kvp in searchExperimentLevel.selectStatusDict)
            {
                if (kvp.Value == true)
                {
                    Debug.Log(kvp.Key.gameObject.name + " select status: true");
                }
            }
        }
    }
}
