using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;
using System.Collections.Generic;

public class SceneGraphExporter : EditorWindow
{
    private static IEnumerable<string> material_list;

    [MenuItem("Window/Export Scene Graph")]
    private static void ExportSceneGraph()
    {
        // Get the current scene
        UnityEngine.SceneManagement.Scene scene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();

        // Create a dictionary to store the scene graph
        material_list = new List<string>(new string[] {"UT2", "UT3", "UT4", "UT5", "black cotton", "black leather", "black plastic", "blue cotton", "blue leather", "blue plastic", "brown cotton", "brown leather", "brown plastic", "green cotton", "green leather", "green plastic", "grey cotton", "grey leather", "grey plastic", "orange cotton", "orange leather", "orange plastic", "purple cotton", "purple leather", "purple plastic", "red cotton", "red leather", "red plastic", "white cotton", "white leather", "white plastic", "yellow cotton", "yellow leather", "yellow plastic"});
        Dictionary<string, object> sceneGraph = new Dictionary<string, object>();
        sceneGraph["name"] = scene.name;
        sceneGraph["type"] = "Scene";
        sceneGraph["children"] = GetChildren(scene.GetRootGameObjects());

        // Convert the scene graph to JSON
        string json = JsonConvert.SerializeObject(sceneGraph, Formatting.Indented);

        // Save the JSON to a file
        string outputPath = EditorUtility.SaveFilePanel("Export Scene Graph", "", "scene_graph.json", "json");
        if (!string.IsNullOrEmpty(outputPath))
        {
            System.IO.File.WriteAllText(outputPath, json);
            Debug.Log("Scene graph exported to: " + outputPath);
        }
    }

    private static List<object> GetChildren(GameObject[] gameObjects)
    {
        List<object> children = new List<object>();

        foreach (GameObject gameObject in gameObjects)
        {
            Dictionary<string, object> child = new Dictionary<string, object>();
            child["name"] = gameObject.name;
            //child["type"] = "GameObject";
            child["components"] = GetComponentData(gameObject);

            // Get children recursively
            Transform transform = gameObject.transform;
            if (transform.childCount > 0)
                child["children"] = GetChildren(GetChildGameObjects(transform));

            children.Add(child);
        }

        return children;
    }

    private static List<object> GetComponentData(GameObject gameObject)
    {
        List<object> componentData = new List<object>();

        Component[] components = gameObject.GetComponents<Component>();
        bool print_details = false;
        foreach (Component component in components)
        {
            if (component is MeshRenderer)
            {
                MeshRenderer meshRenderer = (MeshRenderer)component;
                foreach (string material in material_list)
                {
                    if (meshRenderer.sharedMaterials[0] != null)
                    {
                        //Debug.Log(meshRenderer.sharedMaterials[0].name);
                        if (meshRenderer.sharedMaterials[0].name == material)
                        {
                            print_details = true;
                        }
                    }
                    
                }
            }
        }

        Dictionary<string, object> data = new Dictionary<string, object>();

        foreach (Component component in components)
        {

            if (component is Transform)
            {
                //data["type"] = component.GetType().ToString();
                Transform transform = (Transform)component;
                data["position"] = GetVector3Data(transform.position);
                data["rotation"] = GetVector3Data(transform.rotation.eulerAngles);
                //data["scale"] = GetVector3Data(transform.localScale);
                //if(print_details)
                //    componentData.Add(data);
            }
            else if (component is MeshRenderer)
            {
                //data["type"] = component.GetType().ToString();
                MeshRenderer meshRenderer = (MeshRenderer)component;
                //Debug.Log(meshRenderer.sharedMaterials.Length);
                //Debug.Log(meshRenderer.sharedMaterials[0].name);
                if (meshRenderer.sharedMaterials[0] != null)
                {
                    data["appearance"] = meshRenderer.sharedMaterials[0].name;
                }
            }
        }
        if (print_details)
            componentData.Add(data);

        return componentData;
    }

    private static GameObject[] GetChildGameObjects(Transform transform)
    {
        GameObject[] childGameObjects = new GameObject[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            childGameObjects[i] = transform.GetChild(i).gameObject;
        }

        return childGameObjects;
    }

    private static Dictionary<string, float> GetVector3Data(Vector3 vector)
    {
        Dictionary<string, float> data = new Dictionary<string, float>();
        data["x"] = vector.x;
        data["y"] = vector.y;
        data["z"] = vector.z;
        return data;
    }
}
