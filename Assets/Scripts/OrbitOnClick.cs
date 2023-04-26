using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple script for rotating the camera and labels on their axis when the right mouse button is clicked
/// </summary>
public class OrbitOnClick : MonoBehaviour
{
    public float rotationSpeedMultiplier = 20f;
    public List<GameObject> additionalOrbits;
    
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            transform.RotateAround(transform.parent.transform.position, Vector3.down, Time.deltaTime * rotationSpeedMultiplier);
            foreach (GameObject orbit in additionalOrbits)
            {
                orbit.transform.RotateAround(orbit.transform.parent.transform.position, Vector3.down, Time.deltaTime * rotationSpeedMultiplier);
            }
        }
    }
}
