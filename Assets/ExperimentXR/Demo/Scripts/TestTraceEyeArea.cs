using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TestTraceEyeArea : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(nameof(testdeAPI));
    }

    public IEnumerator testdeAPI()
    {
        XPXRManager.Recorder.StartSession();
        Debug.Log("Start tracing");
        XPXRManager.Recorder.StartSession(environmentProperties: new Dictionary<string, string>(){
            {"proprieté 1", Random.Range(0,12).ToString()},
            {"proprieté 2", Random.Range(0,12).ToString()},
            {"proprieté 3", Random.Range(0,12).ToString()},
            {"proprieté 4", Random.Range(0,12).ToString()}
        });
#if UNITY_EDITOR
        yield return new WaitForSeconds(120f);
        XPXRManager.Recorder.StopSession();
        yield return new WaitForSeconds(1f);
        XPXRManager.Recorder.StopSession();
        Debug.Log("End of the trial");
        while (XPXRManager.Recorder.TransfersState() != 0)
        {
            yield return new WaitForSeconds(1);
        }
        XPXRManager.Recorder.EndTracing();
        Debug.Log("End of the record");

        EditorApplication.ExitPlaymode();
#endif
        yield return null;
    }

    IEnumerator stop()
    {
        yield return new WaitForSeconds(1f);
        XPXRManager.Recorder.StopSession();
        yield return new WaitForSeconds(1f);
        XPXRManager.Recorder.StopSession();
        Debug.Log("End of the trial");
        while (XPXRManager.Recorder.TransfersState() != 0)
        {
            yield return new WaitForSeconds(1);
        }
        XPXRManager.Recorder.EndTracing();
        Debug.Log("End of the record");
    }
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.Start, OVRInput.Controller.LTouch | OVRInput.Controller.LHand))
        {
            StartCoroutine(nameof(this.stop));
        }
    }
}
