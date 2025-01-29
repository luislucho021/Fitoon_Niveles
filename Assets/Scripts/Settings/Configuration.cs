using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Configuration : MonoBehaviour
{
    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Application.targetFrameRate = 60;
        //Debug.Log("Target framerate set to " + Application.targetFrameRate);
        Screen.sleepTimeout = SleepTimeout.NeverSleep; //Para que no se apague el telefono
    }
}
