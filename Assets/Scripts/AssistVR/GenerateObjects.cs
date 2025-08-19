using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
//using Unity.VisualScripting;
//using System.Numerics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GenerateObjects : MonoBehaviour
{
    //public GameObject[] myObjects;
    public int numberOfObjects;
    public float distributionDimension;
    public float objectSpacing;
    private static Collider[] neighbours;
    private bool obstacleTrue;
    //private List<string> objectColorList = new List<string>(new string[] { "black", "blue", "brown", "green", "grey", "orange", "purple", "red", "white" });
    //private List<string> objectTextureList = new List<string>(new string[] { "plastic", "cotton", "leather" });
    //private List<string> objectMaterialList = new List<string>(new string[] { "red plastic", "green plastic", "blue plastic", "white plastic", "purple plastic", "Unknown1", "Unknown2", "Unknown3", "Unknown4", "Unknown5"});
    private List<string> objectMaterialList = new List<string>(new string[] { "1", "2", "3" });
    private List<Vector3> spawnPositions = new List<Vector3>();
    private GameObject newObject;
    RayHit RayHit;
    public Dictionary<GameObject, bool> selectStatusDict = new Dictionary<GameObject, bool>();
    private GameObject objectToSelect;

    private void Start()
    {
        RayHit = GetComponent<RayHit>();
        

    }
    async void Update()
    {

        //if (RayHit.hitObjectName)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //foreach (GameObject o in Object.FindObjectsOfType<GameObject>())
            //{
            //    if (o.name.Contains("Object")|o.name.Contains("Cube")|o.name.Contains("Capsule")| o.name.Contains("Sphere") | o.name.Contains("Cylinder"))
            //    {
            //        Destroy(o);
            //    }
            //}
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);

            await Task.Delay(50);

            for (int i = 1; i < numberOfObjects; i++)
            {
                //int randomIndex = Random.Range(0, myObjects.Length);
                //int randomIndex = Random.Range(0, 10);
                int randomIndex = Random.Range(0, 4);
                int randomColorIndex = Random.Range(0, 9);
                int randomTextureIndex = Random.Range(0, 3);
                int randomMaterialIndex = Random.Range(0, 3);

                //Vector3 randomSpawnPosition = new Vector3(Random.Range(-distributionDimension/2, distributionDimension/2), Random.Range(-distributionDimension / 2, distributionDimension / 2), Random.Range(-distributionDimension / 2, distributionDimension / 2));

                //var neighbours = Physics.OverlapSphere(randomSpawnPosition, objectSpacing);
                Vector3 randomSpawnPosition = new Vector3();

                do
                {
                    randomSpawnPosition = new Vector3(Random.Range(-distributionDimension / 2, distributionDimension / 2), Random.Range(-distributionDimension / 2, distributionDimension / 2), Random.Range(-distributionDimension / 2, distributionDimension / 2));
                    neighbours = Physics.OverlapSphere(randomSpawnPosition, objectSpacing);
                    obstacleTrue = Physics.CheckSphere(randomSpawnPosition, objectSpacing);
                    if (spawnPositions.Any())
                    {
                        foreach (Vector3 existingPosition in spawnPositions)
                        {
                            float dist = Vector3.Distance(existingPosition, randomSpawnPosition);
                            if (dist < objectSpacing)
                            {
                                obstacleTrue = true;
                            }
                        }
                        
                    }
                    //foreach (Collider coll in neighbours)
                    //{
                    //    Debug.Log("Found obstacle: " + coll.name);
                    //}
                } while (neighbours.Length > 0 | obstacleTrue);
                spawnPositions.Add(randomSpawnPosition);
                //foreach (Vector3 position in spawnPositions)
                //{
                //    Debug.Log("Position: " + position);
                //}
                

                if (randomIndex == 0)
                {
                    newObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                }
                else if (randomIndex == 1)
                {
                    newObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                }
                else if (randomIndex == 2)
                {
                    newObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                }
                else if (randomIndex == 3)
                {
                    newObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                }
                else if (randomIndex == 5)
                {
                    //newObject = Resources.Load<GameObject>("Objects/UnknownObject1");
                    //newObject = GameObject.Find("Assets/Unknown Objects/UnknownObject1");
                    var myVector = new UnityEngine.Vector3(0.0f, 0.0f, 0.0f);
                    newObject = GameObject.Instantiate(Resources.Load("Objects/UnknownObject1"), myVector, UnityEngine.Quaternion.identity) as GameObject;
                }
                else if (randomIndex == 6)
                {
                    //newObject = Resources.Load<GameObject>("Objects/UnknownObject2");
                    var myVector = new UnityEngine.Vector3(0.0f, 0.0f, 0.0f);
                    newObject = GameObject.Instantiate(Resources.Load("Objects/UnknownObject2"), myVector, UnityEngine.Quaternion.identity) as GameObject;
                }
                else if (randomIndex == 7)
                {
                    //newObject = Resources.Load<GameObject>("Objects/UnknownObject3");
                    var myVector = new UnityEngine.Vector3(0.0f, 0.0f, 0.0f);
                    newObject = GameObject.Instantiate(Resources.Load("Objects/UnknownObject3"), myVector, UnityEngine.Quaternion.identity) as GameObject;
                }
                else if (randomIndex == 8)
                {
                    //newObject = Resources.Load<GameObject>("Objects/UnknownObject4");
                    var myVector = new UnityEngine.Vector3(0.0f, 0.0f, 0.0f);
                    newObject = GameObject.Instantiate(Resources.Load("Objects/UnknownObject4"), myVector, UnityEngine.Quaternion.identity) as GameObject;
                }
                else if (randomIndex == 9)
                {
                    var myVector = new UnityEngine.Vector3(0.0f, 0.0f, 0.0f);
                    newObject = GameObject.Instantiate(Resources.Load("Objects/UnknownObject5"), myVector, UnityEngine.Quaternion.identity) as GameObject;
                }
                else if (randomIndex == 10)
                {
                    var myVector = new UnityEngine.Vector3(0.0f, 0.0f, 0.0f);
                    newObject = GameObject.Instantiate(Resources.Load("Objects/UnknownObject6"), myVector, UnityEngine.Quaternion.identity) as GameObject;
                }
                else if (randomIndex == 11)
                {
                    var myVector = new UnityEngine.Vector3(0.0f, 0.0f, 0.0f);
                    newObject = GameObject.Instantiate(Resources.Load("Objects/UnknownObject7"), myVector, UnityEngine.Quaternion.identity) as GameObject;
                }
                //else if (randomIndex == 11)
                //{
                //    //newObject = Resources.Load<GameObject>("Objects/UnknownObject4");
                //    var myVector = new UnityEngine.Vector3(0.0f, 0.0f, 0.0f);
                //    newObject = GameObject.Instantiate(Resources.Load("Objects/UnknownObject8"), myVector, UnityEngine.Quaternion.identity) as GameObject;
                //}
                else if (randomIndex == 4)
                {
                    var myVector = new UnityEngine.Vector3(0.0f, 0.0f, 0.0f);
                    newObject = GameObject.Instantiate(Resources.Load("Objects/Pyramid"), myVector, UnityEngine.Quaternion.identity) as GameObject;
                }
                //else if (randomIndex == 4)
                //{
                //    newObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
                //}
                //string objectColor = objectColorList[randomColorIndex];
                //string objectTexture = objectTextureList[randomTextureIndex];
                string objectMaterial = objectMaterialList[randomMaterialIndex];
                //Material objecttargetAppearance = Resources.Load(objectColor + " " + objectTexture, typeof(Material)) as Material;
                Material objecttargetAppearance = Resources.Load(objectMaterial, typeof(Material)) as Material;
                if (newObject.transform.childCount > 0)
                {
                    GameObject child = newObject.transform.GetChild(0).gameObject;
                    if (child.GetComponent<Renderer>() == null)
                    {
                        child.AddComponent<Renderer>();
                    }
                    child.GetComponent<Renderer>().material = objecttargetAppearance;
                }
                else
                {
                    if (newObject.GetComponent<Renderer>() == null)
                    {
                        newObject.AddComponent<Renderer>();
                    }
                    newObject.GetComponent<Renderer>().material = objecttargetAppearance;
                }
                
                newObject.GetComponent<Transform>().position = randomSpawnPosition;
                newObject.GetComponent<Transform>().rotation = Random.rotation;
                if (i == 1)
                {
                    objectToSelect = newObject;
                    int LayerIgnoreRaycast = LayerMask.NameToLayer("Ignore Raycast");

                    GameObject regionOfInterest = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    regionOfInterest.layer = 2;
                    Debug.Log("Current Layer: " + regionOfInterest.layer);
                    regionOfInterest.GetComponent<Transform>().position = randomSpawnPosition;
                    regionOfInterest.GetComponent<Transform>().localScale = new Vector3(3, 3, 3);
                    Material blur = Resources.Load("region of interest", typeof(Material)) as Material;
                    regionOfInterest.GetComponent<Renderer>().material = blur;

                    // Draw Home Object and White Home Region
                    GameObject homeObject = Instantiate(newObject, new Vector3(3, -1, -11), Quaternion.identity);
                    GameObject homeRegion = GameObject.CreatePrimitive(PrimitiveType.Sphere);

                    homeRegion.layer = 2;
                    homeRegion.GetComponent<Transform>().position = new Vector3(3, -1, -11);
                    homeRegion.GetComponent<Transform>().localScale = new Vector3(3, 3, 3);
                    Material homeBlur = Resources.Load("region of interest", typeof(Material)) as Material;
                    homeRegion.GetComponent<Renderer>().material = homeBlur;

                }
                //Debug.Log("Random Position: " + randomSpawnPosition);
                //Instantiate(newObject, randomSpawnPosition, Random.rotation);
            }

            

            //GameObject[] allObjects = FindObjectsOfType<GameObject>();
            foreach (GameObject go in FindObjectsOfType<GameObject>())
            {
                selectStatusDict[go] = false;
            }

            if (RayHit.hasHit && RayHit.triggerButtonDown && !RayHit.prevTriggerDown)
            {
                //Debug.Log("Detected trigger pressed!");
                bool prevSelectStat = selectStatusDict[RayHit.hitObject];
                if (prevSelectStat)
                {
                    Outline outline = RayHit.hitObject.GetComponent<Outline>();
                    Destroy(outline);
                    selectStatusDict[RayHit.hitObject] = false;
                }
                else
                {
                    Outline outline = RayHit.hitObject.AddComponent<Outline>();
                    outline.OutlineMode = Outline.Mode.OutlineAll;
                    outline.OutlineColor = UnityEngine.Color.green;
                    outline.OutlineWidth = 10f;
                    selectStatusDict[RayHit.hitObject] = true;
                }
            }

            await Task.Delay(1000);

        }
    }

    public void SelectObject(GameObject hitObject)
    {
        //Debug.Log("Detected trigger pressed!");
        bool prevSelectStat = selectStatusDict[RayHit.hitObject];
        if (prevSelectStat)
        {
            Outline outline = RayHit.hitObject.GetComponent<Outline>();
            Destroy(outline);
            selectStatusDict[RayHit.hitObject] = false;
        }
        else
        {
            Outline outline = RayHit.hitObject.GetComponent<Outline>();
            //outline.OutlineMode = Outline.Mode.OutlineAll;
            outline.OutlineColor = UnityEngine.Color.green;
            //outline.OutlineWidth = 10f;
            selectStatusDict[RayHit.hitObject] = true;
            if (RayHit.hitObject == objectToSelect)
            {
                Debug.Log("Target Object Selected!");
            }
        }
    }
    

    // Update is called once per frame
    //void Update()
    //{
        
    //}
}
