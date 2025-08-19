using System.Linq;
using UnityEditor;
using UnityEngine;

public class ShapeAndColorChanger : MonoBehaviour
{
    //SpeechRecognition SpeechRecognition;
    SearchExperimentLevel searchExperimentLevel;
    private static readonly string[] shapeNames = { "cube", "sphere", "cylinder", "pyramid" };

    private void Start()
    {
        //SpeechRecognition = GetComponent<SpeechRecognition>();
        searchExperimentLevel = FindObjectOfType<SearchExperimentLevel>();
    }

    private static readonly string[] supportedShapes = { "cube", "sphere", "cylinder", "pyramid", "barrel", "cross", "pyramid cuboid", "truncated cylinder" };
    public void ChangeShapeAndColor(string originalColor, string originalShape, string targetColor, string targetShape)
    {
        // Find all game objects in the scene
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            // Check if the object has a MeshRenderer component
            MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                // Check if the object has a material with the original color
                bool hasOriginalColor = false;
                if (originalColor == null | originalColor == "all")
                {
                    hasOriginalColor = true;
                }
                else
                {
                    foreach (Material mat in meshRenderer.sharedMaterials)
                    {
                        if (mat.name.ToLower().Contains(originalColor.ToLower()))
                        {
                            hasOriginalColor = true;
                            break;
                        }
                    }
                }
                Debug.Log("hasOriginalColor: " + hasOriginalColor);

                bool hasOriginalShape = false;
                if (originalShape == null | originalShape == "all")
                {
                    hasOriginalShape = true;
                }
                else
                {
                    if (originalShape.ToLower().Contains(obj.tag.ToLower()))
                        hasOriginalShape = true;
                }
                Debug.Log("hasOriginalShape: " + hasOriginalShape);

                //if (hasOriginalColor && ContainsShapeSubstring(obj.name.ToLower(), originalShape.ToLower()))
                if (hasOriginalColor && hasOriginalShape)
                {
                    // Change the shape if specified
                    if (!string.IsNullOrEmpty(targetShape))
                    {
                        Debug.Log("Closest Shape Match is: " + NormalizeShapeString(targetShape));
                        ChangeShape(obj, NormalizeShapeString(targetShape));
                    }

                    // Change the color if specified
                    if (!string.IsNullOrEmpty(targetColor))
                    {
                        ChangeColor(obj, targetColor);
                    }
                }
            }
        }
    }

    public void SelectObject(string originalColor, string originalShape)
    {
        searchExperimentLevel = FindObjectOfType<SearchExperimentLevel>();
        // Find all game objects in the scene
        Interactable[] allObjects = UnityEngine.Object.FindObjectsOfType<Interactable>();

        foreach (Interactable obj in allObjects)
        {
            if (obj.gameObject.GetComponent<IsInPanel>() == null)
            {
                // Check if the object has a MeshRenderer component
                MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();
                if (meshRenderer != null)
                {
                    // Check if the object has a material with the original color
                    bool hasOriginalColor = false;
                    if (originalColor == "" | originalColor == "all")
                    {
                        hasOriginalColor = true;
                    }
                    else
                    {
                        foreach (Material mat in meshRenderer.sharedMaterials)
                        {
                            //if (ContainsShapeSubstring(mat.name.ToLower(), originalColor.ToLower()))
                            // && mat.name.ToString().Length - originalColor.ToLower().Length < 5
                            Debug.Log(mat.name + "mat.name.Length: " + mat.name.Length);
                            Debug.Log(originalColor + "originalColor.Length: " + originalColor.Length);
                            string go_mat_name = mat.name.Replace(" (Instance)", "");
                            Debug.Log("mat.name.Length - originalColor.Length: " + (go_mat_name.Length - originalColor.Length) + ((go_mat_name.Length - originalColor.Length) < 19));
                            if (go_mat_name.ToLower().Contains(originalColor.ToLower()) && (go_mat_name.Length - originalColor.Length) < 5)
                            {
                                hasOriginalColor = true;
                                //Debug.Log("hasoriginalcolor:" + hasOriginalColor);
                                break;
                            }
                        }
                    }
                    //Debug.Log("hasOriginalColor: " + hasOriginalColor);

                    bool hasOriginalShape = false;
                    if (originalShape == "" | originalShape == "all" | originalShape.Contains("object"))
                    {
                        hasOriginalShape = true;
                    }
                    else
                    {
                        //.Substring(0, originalShape.Length - 2))
                        if (ContainsShapeSubstring(obj.gameObject.name, originalShape.ToLower()) && obj.gameObject.tag != "homeObject")
                        {
                            hasOriginalShape = true;
                            //Debug.Log("has Original Shape: " + hasOriginalShape);
                        }
                            //if (originalShape.ToLower().Contains(obj.tag.ToLower()))
                            
                            
                    }
                    //Debug.Log("hasOriginalShape: " + hasOriginalShape);

                    //if (hasOriginalColor && ContainsShapeSubstring(obj.name.ToLower(), originalShape.ToLower()))
                    if (hasOriginalColor && hasOriginalShape)
                    {
                        Debug.Log("Assigning outline to: " + obj.name);
                        if (obj.gameObject.GetComponent<Outline>() == null)
                        {
                            obj.gameObject.AddComponent<Outline>();
                        }
                        if (obj.gameObject.GetComponent<Outline>().enabled == false)
                        {
                            obj.gameObject.GetComponent<Outline>().enabled = true;
                        }
                        Outline outline = obj.gameObject.GetComponent<Outline>();
                        outline.OutlineMode = Outline.Mode.OutlineVisible;
                        outline.OutlineColor = UnityEngine.Color.green;
                        outline.OutlineWidth = 10f;
                        searchExperimentLevel.selectStatusDict[obj] = true;
                        //SpeechRecognition.selectStatusDict[obj] = true;
                        //Debug.Log(obj.gameObject.name + " selection status: " + SpeechRecognition.selectStatusDict[obj]);
                        Debug.Log(obj.gameObject.name + " selection status: " + searchExperimentLevel.selectStatusDict[obj]);

                        //Interactable original = obj.GetComponent<shapeItem_3>().original;
                        //Select(original);
                    }
                }
            }
        }
    }



    bool ContainsShapeSubstring(string objectName, string shapeSubstring)
    {
        // Split the object name by non-word characters (e.g., "-", "_", etc.)
        string[] nameParts = System.Text.RegularExpressions.Regex.Split(objectName, @"\W+");

        // Check if any part of the name matches the shape substring
        foreach (string part in nameParts)
        {
            if (shapeSubstring.ToLower().Contains(part.ToLower()))
            {
                return true;
            }
        }

        return false;
    }

    void ChangeShape(GameObject obj, string targetShape)
    {
        Debug.Log("Trying to find mesh file at: " + "Meshes/" + targetShape.ToLower());
        // Load the mesh from the Assets folder based on the target shape name
        Mesh targetMesh = Resources.Load<Mesh>("Meshes/" + targetShape.ToLower());

        if (targetMesh != null)
        {
            MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                meshFilter.mesh = targetMesh;
                //obj.name = targetShape;
                obj.tag = targetShape;
            }
        }
        else
        {
            Debug.LogWarning("Mesh not found for target shape: " + targetShape);
        }
    }

    void ChangeColor(GameObject obj, string targetColor)
    {
        // Find a material with the target color and assign it to the object
        Material[] allMaterials = Resources.FindObjectsOfTypeAll<Material>();
        foreach (Material mat in allMaterials)
        {
            if (mat.name.ToLower().Contains(targetColor.ToLower()))
            {
                Renderer renderer = obj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material = mat;
                }
                break;
            }
        }
    }

    string NormalizeShapeString(string shapeString)
    {
        // Convert the shape string to lowercase
        string normalizedShape = shapeString.ToLower();

        // Calculate the Levenshtein distance between the normalized shape and each supported shape
        int minDistance = int.MaxValue;
        string closestMatch = null;

        foreach (string supportedShape in supportedShapes)
        {
            int distance = LevenshteinDistance(normalizedShape, supportedShape);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestMatch = supportedShape;
            }
        }

        return closestMatch;
    }

    int LevenshteinDistance(string source, string target)
    {
        int n = source.Length;
        int m = target.Length;
        int[,] distance = new int[n + 1, m + 1];

        // Initialize the first row and column
        for (int i = 0; i <= n; i++)
            distance[i, 0] = i;
        for (int j = 0; j <= m; j++)
            distance[0, j] = j;

        // Calculate the Levenshtein distance
        for (int i = 1; i <= n; i++)
        {
            for (int j = 1; j <= m; j++)
            {
                int cost = (source[i - 1] == target[j - 1]) ? 0 : 1;
                distance[i, j] = Mathf.Min(
                    Mathf.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1),
                    distance[i - 1, j - 1] + cost);
            }
        }

        return distance[n, m];
    }
}