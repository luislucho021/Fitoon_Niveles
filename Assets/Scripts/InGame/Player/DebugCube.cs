using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCube : MonoBehaviour
{
    public float rotationSpeed = 45f; // Speed of rotation in degrees per second

    void Update()
    {
        // Calculate the angle based on time
        float angle = Mathf.Sin(Time.time * rotationSpeed * Mathf.Deg2Rad) * 45f;

        // Apply rotation to the object around its up axis
        transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }
}
