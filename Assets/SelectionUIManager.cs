using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SelectionUIManager : MonoBehaviour
{
    public Dictionary<Interactable, bool> selectStatusDict = new Dictionary<Interactable, bool>(); // Dictionary containing selection status
    public GameObject miniatureShapePrefab; // Prefab for miniature shapes

    private List<GameObject> miniatureShapes = new List<GameObject>(); // List to store instantiated miniature shapes
    private List<Text> objectNameTexts = new List<Text>(); // List to store object name text components

    void Start()
    {
        // Iterate through the selection status dictionary
        foreach (KeyValuePair<Interactable, bool> kvp in selectStatusDict)
        {
            Interactable interactable = kvp.Key;
            bool isSelected = kvp.Value;

            if (isSelected)
            {
                // Instantiate miniature shape prefab
                GameObject miniatureShape = Instantiate(miniatureShapePrefab, transform);
                miniatureShapes.Add(miniatureShape);

                // Instantiate object name text
                Text objectNameText = miniatureShape.AddComponent<Text>();
                objectNameText.text = interactable.name;
                objectNameTexts.Add(objectNameText);

                // Position the miniature shape and text within the Panel
                // (e.g., using a layout system like Grid Layout Group)
            }
        }
    }

    // Method to update the selection status dictionary
    public void UpdateSelectionStatus(Interactable interactable, bool isSelected)
    {
        if (selectStatusDict.ContainsKey(interactable))
        {
            selectStatusDict[interactable] = isSelected;
        }
        else
        {
            selectStatusDict.Add(interactable, isSelected);
        }

        // Call a method to refresh the UI panel
        RefreshUIPanel();
    }

    private void RefreshUIPanel()
    {
        // Clear the existing miniature shapes and object name texts
        foreach (GameObject go in miniatureShapes)
        {
            Destroy(go);
        }
        miniatureShapes.Clear();
        objectNameTexts.Clear();

        // Re-instantiate the miniature shapes and object name texts based on the updated selection status
        foreach (KeyValuePair<Interactable, bool> kvp in selectStatusDict)
        {
            Interactable interactable = kvp.Key;
            bool isSelected = kvp.Value;

            if (isSelected)
            {
                GameObject miniatureShape = Instantiate(miniatureShapePrefab, transform);
                miniatureShapes.Add(miniatureShape);

                Text objectNameText = miniatureShape.AddComponent<Text>();
                objectNameText.text = interactable.name;
                objectNameTexts.Add(objectNameText);

                // Position the miniature shape and text within the Panel
                // (e.g., using a layout system like Grid Layout Group)
            }
        }
    }
}