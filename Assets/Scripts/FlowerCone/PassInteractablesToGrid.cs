using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PassInteractablesToGrid : MonoBehaviour
{
    [SerializeField] private InputActionReference assistVRConeActionRef; // formerly flowerConeActionRef
    [SerializeField] private InputActionReference sameAsSelectRef;

    [SerializeField] private GameObject assistVRCone; // renamed from cone
    [SerializeField] private ConeVolumeHighlighter coneVolumeHighlighter;
    [SerializeField] private GameObject rayGameObject;

    //[SerializeField] private GameGrid grid;  // This grid functionality is temporarily disabled.
    [SerializeField] private Transform rayStartPoint;

    [SerializeField] private GrabbingHand grabbingHand;

    public enum AssistVRMode
    {
        Highlighting, SelectingWithRay, None
    }

    [ReadOnly] private AssistVRMode mode;

    private void OnEnable()
    {
        TransitionToHighlighting();
    }

    public void AtTrialStart()
    {
        TransitionToHighlighting();
    }

    /// <summary>
    /// (Temporarily disabled) Original function that creates a grid for displaying all interactables in the cone.
    /// </summary>
    /// <returns></returns>
    /*
    private uint CallGridInitialize()
    {
        List<Interactable> allHighlightedObjects = coneVolumeHighlighter.GetAllInteractables();
        EndInteractablesHover(allHighlightedObjects);

        if (allHighlightedObjects.Count == 0)
            return 0;

        if (allHighlightedObjects.Count == 1)
        {
            grabbingHand.CallPickUpObject(allHighlightedObjects[0]);
            grid.DestroyGrid();
            return 1;
        }

        grid.DestroyGrid();
        if (SceneManager.GetActiveScene().name.Contains("Editor Eco"))
        {
            grid.SetGridTransformToCameraForward();     
        }
        grid.CreateGrid(allHighlightedObjects);
        return 2;
    }
    */

    private void EndInteractablesHover(List<Interactable> interactables)
    {
        interactables.ForEach(x => x.EndHover());
    }

    /// <summary>
    /// Use the ray to select a single interactable object.
    /// </summary>
    /// <returns></returns>
    private bool SelectWithRay()
    {
        if (Physics.Raycast(rayStartPoint.position, rayStartPoint.forward, out RaycastHit hit, Mathf.Infinity))
        {
            if (hit.collider.CompareTag("GridInteractable"))
            {
                Interactable og = hit.collider.transform.parent.GetComponent<GridCell>().originalInteractable;
                grabbingHand.CallPickUpObject(og);
                // grid.DestroyGrid();  // (disabled)
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Process input from the controller.
    /// </summary>
    private void ProcessInput()
    {
        // If the same-as-select action is pressed, reset to highlighting mode.
        if (sameAsSelectRef.action.WasPressedThisFrame())
        {
            // grid.DestroyGrid();  // (disabled)
            TransitionToHighlighting();
            return;
        }

        // Process input only when the trigger (assistVRConeActionRef) is pressed.
        if (!assistVRConeActionRef.action.WasPressedThisFrame())
        {
            return;
        }

        switch (mode)
        {
            case AssistVRMode.None:
                break;

            case AssistVRMode.Highlighting:
                // Temporarily disable the grid/menu functionality.
                // uint gridInitCode = CallGridInitialize();
                // if (gridInitCode == 0) { }
                // else if (gridInitCode == 1)
                //     TransitionToNone();
                // else
                //     TransitionToSelecting();

                // Instead, print the names of all interactable objects in the cone.
                //List<Interactable> objectsInCone = coneVolumeHighlighter.GetAllInteractables();
                //if (objectsInCone.Count == 0)
                //{
                //    Debug.Log("AssistVRCone: No interactable objects found in cone.");
                //}
                //else
                //{
                //    string objectNames = string.Join(", ", objectsInCone.ConvertAll(obj => obj.gameObject.name));
                //    Debug.Log("AssistVRCone: Objects within cone: " + objectNames);
                //}
                coneVolumeHighlighter.LogCurrentInteractables(); 
                break;

            case AssistVRMode.SelectingWithRay:
                if (SelectWithRay())
                    TransitionToNone();
                break;
        }
    }

    private void TransitionToHighlighting()
    {
        assistVRCone.SetActive(true);
        rayGameObject.SetActive(false);
        mode = AssistVRMode.Highlighting;
    }

    private void TransitionToSelecting()
    {
        assistVRCone.SetActive(false);
        rayGameObject.SetActive(true);
        mode = AssistVRMode.SelectingWithRay;
    }

    private void TransitionToNone()
    {
        assistVRCone.SetActive(false);
        rayGameObject.SetActive(false);
        mode = AssistVRMode.None;

        // For debugging, return to highlighting mode.
        TransitionToHighlighting();
    }

    private void Update()
    {
        ProcessInput();
    }
}
