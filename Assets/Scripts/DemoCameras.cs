using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoCameras : MonoBehaviour
{
    public List<GameObject> cameras;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeCamera(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeCamera(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChangeCamera(3);
        }
    }

    public void ChangeCamera(int num)
    {
        switch (num)
        {
            case 1:
                cameras[0].SetActive(true);
                cameras[1].SetActive(false);
                cameras[2].SetActive(false);
                break;
            case 2:
                cameras[0].SetActive(false);
                cameras[1].SetActive(true);
                cameras[2].SetActive(false);
                break;
            case 3:
                cameras[0].SetActive(false);
                cameras[1].SetActive(false);
                cameras[2].SetActive(true);
                break;
            default:
                break;
        }
    }
}
