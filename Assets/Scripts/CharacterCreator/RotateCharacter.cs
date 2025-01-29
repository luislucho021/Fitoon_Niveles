using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCharacter : MonoBehaviour
{
    private Vector3 initialMousePosition;
    private Vector3 initialRotation;

    private bool isRotating = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    isRotating = true;
                    initialMousePosition = Input.mousePosition;
                    initialRotation = transform.eulerAngles;
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isRotating = false;
        }

        if (isRotating)
        {
            float deltaX = Input.mousePosition.x - initialMousePosition.x;

            float rotationAngle = deltaX * 0.5f; // Puedes ajustar la velocidad de rotación aquí

            Vector3 newRotation = initialRotation + new Vector3(0, -rotationAngle, 0);
            transform.eulerAngles = newRotation;
        }
    }
}
