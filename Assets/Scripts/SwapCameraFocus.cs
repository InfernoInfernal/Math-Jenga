using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple script to move the camera's pivot point between different gameobjects in a list with Q and E keys
/// </summary>
public class SwapCameraFocus : MonoBehaviour
{
    public List<GameObject> cameraFocusPoints;
    private int focusIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = cameraFocusPoints[0].transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            focusIndex--;
            if (focusIndex < 0)
                focusIndex = cameraFocusPoints.Count - 1;

            transform.position = cameraFocusPoints[focusIndex].transform.position;
        }
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            focusIndex++;
            if (focusIndex >= cameraFocusPoints.Count)
                focusIndex = 0;

            transform.position = cameraFocusPoints[focusIndex].transform.position;
        }
    }
}
