using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public bool isReset = false;

    private Vector3 startPosition;
    private Quaternion startRotation;

    // Start is called before the first frame update
    void Awake()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    private void Update()
    {
        if (isReset)
        {
            ResetPosition();
            isReset = false;
        }
    }

    public void ResetPosition()
    {
        Camera.main.orthographic = true;
        Camera.main.orthographicSize = 160;
        transform.position = startPosition;
        transform.rotation = startRotation;
        print("Camera Reset");
    }


}
