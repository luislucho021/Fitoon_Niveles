using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floating : MonoBehaviour
{
    public float floatAmplitude = 0.5f;
    public float floatFrequency = 1f;
    public float offset = 0f;

    private Vector3 originalPosition;
    private float elapsedTime;


    private void Start()
    {
        originalPosition = transform.position;
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        offset = Mathf.Clamp(offset, 0f, floatFrequency);

        float yOffset = Mathf.Sin((elapsedTime + offset) * floatFrequency) * floatAmplitude;

        transform.position = originalPosition + new Vector3(0f, yOffset, 0f);
    }

}
