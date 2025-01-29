using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Linq;

public class InteractionScript : MonoBehaviour
{
    #region VARIABLES

    public Vector3 trackable;
    private List<float> magList = new List<float>();
    private float total_time = 0f;
    private List<float> timeList = new List<float>();
    private List<(float, float)> cadenceList = new List<(float, float)>();

    private float cadence = 0f;

    #endregion

    #region REFERENCES

    public ARFaceManager ar;
    public Camera cam;

    #endregion

    #region GAMELOOP METHODS

    void Start()
    {
        magList.Clear();
        timeList.Clear();
        cadenceList.Clear();
        total_time = 0f;
        cadence = 0f;
    }

    void FixedUpdate()
    {
        CheckTrackable();
        HandleData();
        ComputeData();
    }

    void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 50;
        style.normal.textColor = Color.blue;
    }

    #endregion

    #region METHODS

    void CheckTrackable()
    {
        trackable = new Vector3(0f, 0f, 0f);

        foreach (var _t in ar.trackables)
        {
            trackable = _t.transform.position;
            return;
        }
    }

    void HandleData()
    {
        if (trackable == null) { return; }

        // 1. MAG SCREEN LIST
        Vector2 magScreen = cam.WorldToScreenPoint(trackable);
        magScreen.x /= cam.pixelWidth;
        magScreen.y /= cam.pixelHeight;
        magList.Add(magScreen.sqrMagnitude);

        // 1. TIME LIST
        timeList.Add(total_time);
        total_time += Time.deltaTime;
    }

    void ComputeData()
    {
        if (magList.Count < 100) { return; }

        List<float> smooth = SmoothMagnitudes();

        List<float> diffback = DiffBackwards(smooth);
        List<float> diffforw = DiffForward(smooth);
        List<float> accback = DiffBackwards(diffback);
        List<float> accforw = DiffForward(diffforw);

        List<int> valleys = ValleySearch(diffback, diffforw);
        List<int> stepslow = ValleyPrecision(valleys, accback, accforw, 0.01f);
        List<int> stepshigh = ValleyPrecision(valleys, accback, accforw, 0.001f);

        int stepslowcount = stepslow.Sum();
        int stepshighcount = stepshigh.Sum();

        float dt = timeList[^1] - timeList[0];

        float cadencelow = stepslowcount * 60f / dt;
        float cadencehigh = stepshighcount * 60f / dt;

        cadence = cadencelow < 100 ? cadencehigh : cadencelow; // TODO. Almacenar en lista para suavizar valores.
        cadenceList.Add((timeList[^1], cadence));

        magList.RemoveAt(0);
        timeList.RemoveAt(0);
    }

    List<int> ValleyPrecision(List<int> _valleys, List<float> _back, List<float> _forw, float _precision)
    {
        // TODO. If back.count != forw.count --> error

        List<int> output = new List<int>();

        for (var i = 0; i < _valleys.Count; i++)
        {
            output.Add((_valleys[i] != 0 && (_back[i] + _forw[i]) > _precision) ? 1 : 0);
        }

        return output;
    }

    List<int> ValleySearch(List<float> _back, List<float> _forw)
    {
        // TODO. If back.count != forw.count --> error

        List<int> output = new List<int>();

        for (var i = 0; i < _back.Count; i++)
        {
            output.Add((_back[i] > 0 && _forw[i] > 0) ? 1 : 0);
        }

        return output;
    }


    List<float> DiffBackwards(List<float> _input)
    {
        List<float> output = new List<float>();
        output.Add(0f);

        for (var i = 1; i < _input.Count; i++)
        {
            output.Add(_input[i] - _input[i - 1]);
        }

        return output;
    }

    List<float> DiffForward(List<float> _input)
    {
        List<float> output = new List<float>();

        for (var i = 0; i < _input.Count - 1; i++)
        {
            output.Add(_input[i] - _input[i + 1]);
        }

        output.Add(0f);

        return output;
    }

    List<float> SmoothMagnitudes()
    {
        List<float> output = new List<float>();
        output.Add(magList[0]);
        output.Add(magList[1]);
        output.Add(magList[2]);

        for (var i = 3; i < magList.Count - 3; i++)
        {
            float a = magList[i - 3] * 0.006f;
            float b = magList[i - 2] * 0.061f;
            float c = magList[i - 1] * 0.242f;
            float d = magList[i - 0] * 0.383f;
            float e = magList[i + 1] * 0.242f;
            float f = magList[i + 2] * 0.061f;
            float g = magList[i + 3] * 0.006f;
            output.Add(a + b + c + d + e + f + g);
        }

        output.Add(magList[^3]);
        output.Add(magList[^2]);
        output.Add(magList[^1]);

        return output;
    }

    #endregion
}
