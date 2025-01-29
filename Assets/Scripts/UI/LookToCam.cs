using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookToCam : MonoBehaviour
{
    public GameObject cam;

    void Update()
    {
        transform.forward = cam.transform.forward;
    }
}
