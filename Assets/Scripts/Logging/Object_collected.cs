using UnityEngine;

public class Object_collected : MonoBehaviour
{
    private Vector3 dumpsterLocation = new Vector3(100, 100, 100);
    private new Rigidbody rigidbody;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector3 originalScale;
    private Transform originalParent;
    SearchExperimentLevel searchExperimentLevel;

    private void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        rigidbody = GetComponent<Rigidbody>();
        originalScale = new Vector3(0.2f, 0.2f, 0.2f);
        originalParent = transform.parent;
        searchExperimentLevel = FindObjectOfType<SearchExperimentLevel>();
    }

    public void ResetGameObject()
    {
        ////Disable all outlines
        //Outline[] outls = UnityEngine.Object.FindObjectsOfType<Outline>();
        //foreach (Outline outl in outls)
        //{
        //    outl.gameObject.GetComponent<Outline>().enabled = false;
        //}
        //foreach (Interactable go in FindObjectsOfType<Interactable>())
        //{
        //    searchExperimentLevel.selectStatusDict[go] = false;
        //}
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        rigidbody.velocity = Vector3.zero;
        transform.localScale = originalScale;

        transform.SetParent(originalParent);
    }

    public void ResetOriginalScale()
    {
        transform.localScale = originalScale;
    }

    public void MoveOutsideReach()
    {
        transform.position = dumpsterLocation;
    }
}