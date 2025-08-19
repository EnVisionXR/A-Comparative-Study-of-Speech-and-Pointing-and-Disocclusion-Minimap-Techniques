using cakeslice;
using UnityEngine;

public class TargetAreaOutline : MonoBehaviour
{
    cakeslice.Outline searchOutline;
    Transform _transform;
    Vector3 searchOffset = Vector3.zero;

    void Start()
    {
        searchOutline = gameObject.GetComponent<cakeslice.Outline>();
        if (searchOutline == null) Debug.LogError("Search Area outline not found!");

        searchOutline.enabled = true;
        searchOutline.enabled = false;

        _transform = transform;
    }

    /// <summary>
    /// Enable a shader outline around the position, which should be same
    /// as vector of where the target is.
    /// </summary>
    /// <param name="searchPosition"></param>
    public void EnableSearchOutlineAroundPosition(Vector3 camPosition, Vector3 searchPosition, bool useNewOffset)
    {
        if (useNewOffset)
            searchOffset = Random.insideUnitSphere * 1f;

        //print($"offset: {searchOffset}");

        // direction of the object from camera
        Vector3 objDir = (searchPosition - camPosition).normalized;
        // 5 meters from the camera position in the direction of search position
        objDir = camPosition + objDir * 10f;
        // Add random offset
        objDir += searchOffset;

        //Debug.Log("Calculated transform position: " + objDir);
        _transform.position = objDir;

        searchOutline.enabled = true;
    }

    public void DisableSearchOutlineAroundPosition()
    {
        searchOutline.enabled = false;
    }
}
