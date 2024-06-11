using System.Threading;
using UnityEngine;
using XPXR;
using XPXR.Recorder;

public class XPXRManager : MonoBehaviour
{
    ////// Properties of control //////////////////////////////////
    private static XPXRManager _singleton = null; // Use for certify that there is only one XPXRManager
    private static XPXRRecorder _recorder;
    private static bool _isAwake = false; // Indicate if unity call the Awake method of the XPXRManager

    ////// Public Properties  /////////////////////////////////////
    public static XPXRRecorder Recorder
    {
        get
        {
            // Verification in case Trace is call in the Awake Unity Event for
            if (_recorder == null)
            {
                if (_isAwake == false)
                {
                    throw new XPXRException("Current Scene is not correctly setup for ExperimentXR. Resolution :\n- Please setup the scene in the menu ExperiementXR > Setup the scene\n- Don't call ExperimentXR in a Unity event Awake method.");
                }
                else
                {
                    throw new XPXRException("ExperimentXR is currently starting, verify the status of ExperimentXR with 'XPXRManager.IsReady'");
                }
            }
            else
            {
                return _recorder;
            }
        }
        private set { }
    }

    public static bool IsReady { get; private set; } = false;
    public XPXRConfig config;
    private CancellationTokenSource _cancellationTokenSource;

    private void Awake()
    {
        _isAwake = true;
        // Verify if the Experiment component is not already started
        if (_singleton == null)
        {
            _singleton = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Debug.LogWarning("XPXR : ExperimentXR is already set in the scene");
            Destroy(this.gameObject);
            return;
        }

        // Start the init of Experiment

        // Launch of the Trace module
        this._cancellationTokenSource = new CancellationTokenSource();

#if UNITY_2022_2_OR_NEWER
        _recorder = new XPXRRecorder(this.config, base.destroyCancellationToken);
#else
        _recorder = new XPXRRecorder(this.config, this._cancellationTokenSource.Token);
#endif
        IsReady = true;
    }

    void OnApplicationQuit()
    {
        this._cancellationTokenSource.Cancel();
        _recorder.QuitApplication();
    }
}