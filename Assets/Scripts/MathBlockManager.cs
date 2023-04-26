using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;

/// <summary>
/// Enum that directly maps the material type to the mastery level from deserialized JSON in MathBlockData
/// </summary>
public enum BlockType
{
    Glass,
    Wood,
    Stone
}

/// <summary>
/// Manager class for deserializing API data and constructing jenga stacks from it
/// </summary>
public class MathBlockManager : MonoBehaviour
{
    //API URL used to deserialize JSON for stack spawning
    [SerializeField]
    private string apiUrl = "https://ga1vqcu3o1.execute-api.us-east-1.amazonaws.com/Assessment/stack";
    
    //Material used for highlighted blocks
    [SerializeField]
    private Material selectionMaterial;

    //UI element to display data about highlighted blocks
    [SerializeField]
    private TextMeshProUGUI blockDetailsTextbox;

    //Block prefabs used for spawning
    [Header("Math Block Prefabs")]
    [SerializeField]
    private GameObject glassBlockPrefab;
    [SerializeField]
    private GameObject woodBlockPrefab;
    [SerializeField]
    private GameObject stoneBlockPrefab;

    //Stacks to be spawned
    [Header("Grade Stacks")]
    [SerializeField]
    private GameObject grade6Stack;
    [SerializeField]
    private GameObject grade7Stack;
    [SerializeField]
    private GameObject grade8Stack;

    //Private fields used to track the current highlighted block
    private GameObject currentBlock;
    private Material oldMaterial;

    void Start()
    {
        //Dictionary for material types
        var mathBlockPrefabs = new Dictionary<BlockType, GameObject> 
        {
            {BlockType.Glass, glassBlockPrefab},
            {BlockType.Wood, woodBlockPrefab},
            {BlockType.Stone, stoneBlockPrefab},
        };

        List<MathBlockData> mathBlockData = GetMathBlockDataFromApi(apiUrl);

        //Create stacks for grades 6, 7, and 8. If more grades are desired, they can be added here.
        CreateStack(grade6Stack, mathBlockPrefabs,
            mathBlockData.Where(d => d.grade.Equals("6th Grade")).ToList());
        CreateStack(grade7Stack, mathBlockPrefabs,
            mathBlockData.Where(d => d.grade.Equals("7th Grade")).ToList());
        CreateStack(grade8Stack, mathBlockPrefabs,
            mathBlockData.Where(d => d.grade.Equals("8th Grade")).ToList());
    }

    void Update()
    {
        //When the cursor is over a math block, highlight it and display some of its data to the UI
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100)
            && (hit.transform.gameObject.tag == "GlassBlock"
            || hit.transform.gameObject.tag == "WoodBlock"
            || hit.transform.gameObject.tag == "StoneBlock"))
        {
            if (hit.transform.gameObject != currentBlock)
            {
                if (currentBlock != null)
                    currentBlock.GetComponent<MeshRenderer>().material = oldMaterial;
                currentBlock = hit.transform.gameObject;
                oldMaterial = currentBlock.GetComponent<MeshRenderer>().material;
                currentBlock.GetComponent<MeshRenderer>().material = selectionMaterial;
                MathBlockData d = currentBlock.GetComponent<MathBlockComponent>().mathBlockData;
                blockDetailsTextbox.text = $"{d.grade}: {d.domain}<br><br>{d.cluster}<br><br>{d.standardid}: {d.standarddescription}";
            }
        }
        else if (currentBlock != null)
        {
            currentBlock.GetComponent<MeshRenderer>().material = oldMaterial;
            currentBlock = null;
            oldMaterial = null;
            blockDetailsTextbox.text = "[Grade level]: [Domain]<br><br>[Cluster]<br><br>[Standard ID]: [Standard Description]";
        }
    }

    /// <summary>
    /// Deserializes stack API data into a list of MathBlockDatas and sorts them in preperation for stack spawning
    /// </summary>
    /// <param name="apiUrl">API URL to get deserialize JSON for stacks</param>
    /// <returns></returns>
    public List<MathBlockData> GetMathBlockDataFromApi(string apiUrl)
    {
        string json;
        using (WebClient client = new WebClient())
        {
            json = client.DownloadString(apiUrl);
        }

        //Sort ascending by grade, then domain, then cluster, then standard id
        return JsonConvert.DeserializeObject<List<MathBlockData>>(json)
            .OrderBy(j => j.grade)
            .ThenBy(j => j.domain)
            .ThenBy(j => j.cluster)
            .ThenBy(j => j.standardid).ToList();
    }

    /// <summary>
    /// Creates a jenga stack of math blocks for a specific grade using prefab blocks, and assigning
    /// </summary>
    /// <param name="stackParent">The game object that will be parented to blocks and serve as the spawner's position starting point</param>
    /// <param name="mathBlockPrefabs">A dictionary mapping block type prefabs to their deserialized mastery</param>
    /// <param name="mathBlockDataset">List of MathBlockData deserialized JSON from the API used to determine block count and type</param>
    public void CreateStack(GameObject stackParent, Dictionary<BlockType, GameObject> mathBlockPrefabs, List<MathBlockData> mathBlockDataset)
    {
        int stackSequence = 1;
        Vector3 spawnPosition = new Vector3(
            stackParent.transform.position.x - 1f,
            stackParent.transform.position.y - 0.3f,
            stackParent.transform.position.z);
        Quaternion spawnRotation = stackParent.transform.rotation;

        //Build stack by sets of 3 blocks, and rotating each set perpendicular to the previous full set
        foreach (var mathBlockData in mathBlockDataset)
        {
            //Prepare spawner for creating instance in a given stack sequence
            switch (stackSequence)
            {
                case 1:
                    spawnPosition = new Vector3(
                        spawnPosition.x + 1f,
                        spawnPosition.y + 0.6f,
                        spawnPosition.z);
                    spawnRotation = Quaternion.Euler(0, 0, 0);
                    break;
                case 2:
                    spawnPosition += Vector3.forward;
                    break;
                case 3:
                    spawnPosition += Vector3.back * 2f;
                    break;
                case 4:
                    spawnPosition = new Vector3(
                        spawnPosition.x,
                        spawnPosition.y + 0.6f,
                        spawnPosition.z + 1f);
                    spawnRotation = Quaternion.Euler(0, 90, 0);
                    break;
                case 5:
                    spawnPosition += Vector3.right;
                    break;
                case 6:
                    spawnPosition += Vector3.left * 2f;
                    stackSequence = 0;
                    break;
            }
            stackSequence++;

            //Create block instance
            var newInstance = Instantiate(mathBlockPrefabs[(BlockType)mathBlockData.mastery], spawnPosition, spawnRotation);
            //Make minute alteration in scale to replicate actual jenga blocks
            newInstance.transform.localScale = new Vector3(
                newInstance.transform.localScale.x + UnityEngine.Random.Range(-0.01f, 0f),
                newInstance.transform.localScale.y + UnityEngine.Random.Range(-0.01f, 0f),
                newInstance.transform.localScale.z + UnityEngine.Random.Range(-0.01f, 0f));
            newInstance.transform.parent = stackParent.transform;
            newInstance.GetComponent<MathBlockComponent>().mathBlockData = mathBlockData; //Attach the data to the component script
        }
    }
}
