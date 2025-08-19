using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// This script must be attached to the cone that will hit the interactables.
/// It continuously maintains a list of all interactable objects within its collider.
/// </summary>
[RequireComponent(typeof(Collider))]
public class ConeVolumeHighlighter : MonoBehaviour
{
    private HashSet<Interactable> allHighlightedObjects;

    private void OnEnable()
    {
        allHighlightedObjects = new HashSet<Interactable>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out Interactable interactable))
            return;

        interactable.StartHover();
        allHighlightedObjects.Add(interactable);
        Debug.Log("[AssistVRCone] Object Entered Cone: " + interactable.gameObject.name);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out Interactable interactable))
            return;

        interactable.EndHover();
        allHighlightedObjects.Remove(interactable);
        Debug.Log("[AssistVRCone] Object Exited Cone: " + interactable.gameObject.name);
    }

    /// <summary>
    /// Returns a list of all interactable objects currently flagged within the cone.
    /// </summary>
    /// <returns>List of Interactable components.</returns>
    public List<Interactable> GetAllInteractables()
    {
        return allHighlightedObjects.ToList();
    }

    /// <summary>
    /// Logs a detailed message listing the names of all interactable game objects currently within the cone.
    /// This method should be called when the user presses the controller trigger.
    /// </summary>
    public void LogCurrentInteractables()
    {
        List<Interactable> currentObjects = GetAllInteractables();
        if (currentObjects.Count == 0)
        {
            Debug.Log("[AssistVRCone] No interactable objects currently in the cone.");
        }
        else
        {
            string objectNames = string.Join(", ", currentObjects.Select(x => x.gameObject.name).ToArray());
            Debug.Log($"[AssistVRCone] {currentObjects.Count} interactable object(s) in the cone: {objectNames}");
        }
    }
}
