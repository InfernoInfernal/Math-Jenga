using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Component button script for enabling the Test My Stack gameplay
/// Removes all glass blocks from the scene and enables gravity for the remaining wood and stone blocks
/// </summary>
public class TestMyStack : MonoBehaviour
{
    Button button;
    
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        button.interactable = false;

        foreach(GameObject block in GameObject.FindGameObjectsWithTag("GlassBlock"))
        {
            Destroy(block);
        }
        foreach (GameObject block in GameObject.FindGameObjectsWithTag("WoodBlock"))
        {
            block.GetComponent<Rigidbody>().useGravity = true;
        }
        foreach (GameObject block in GameObject.FindGameObjectsWithTag("StoneBlock"))
        {
            block.GetComponent<Rigidbody>().useGravity = true;
        }
    }
}
