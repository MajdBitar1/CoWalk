using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using XPXR.Recorder.Models;

public class DataCollection : MonoBehaviour
{
    [SerializeField] GroupManager _GroupMan;
    private NetworkPlayerInfo _PlayerOneData, _PlayerTwoData;
    private float _SeparationDistance;
    private float _AverageCycleDuration;
    private int _RhythmState = 0;
    private float _DeltaSpeed;
    private float _counter = 0;
    private bool _TracingState = false;
    
    void OnEnable()
    {
        GameManager.OnPlayerListUpdated += InitialState;
    }
    void OnDisable()
    {
        GameManager.OnPlayerListUpdated -= InitialState;
    }
    void OnApplicationQuit()
    {
        StartCoroutine(EndTrackingRoutine());
    }
    public void InitialState()
    {
        _PlayerOneData = GameManager.LocalPlayerObject.GetComponent<NetworkPlayerInfo>();
        _PlayerTwoData = GameManager.RemotePlayerObject.GetComponent<NetworkPlayerInfo>();
        ChangeTracingState(true);
        _PlayerOneData.RPC_Update_Tracing(true);
        _PlayerTwoData.RPC_Update_Tracing(true);
    }
    public void ChangeTracingState(bool state)
    {
        if (_TracingState == state) return;
        _TracingState = state;
        if(_TracingState)
        {
            StartTracing();
        }
        else
        {
            StopTracing();
        }
    }
    private void StartTracing()
    {
        XPXRManager.Recorder.StartSession();
    }
    private void StopTracing()
    {
        XPXRManager.Recorder.StopSession();
        _TracingState = false;
    }
    IEnumerator EndTrackingRoutine()
    {
        yield return new WaitUntil(() => XPXRManager.Recorder.TransfersState() == 0);
        XPXRManager.Recorder.EndTracing();
    }

    void LateUpdate()
    {
        if (!_TracingState) return;
        if(_counter >= Time.deltaTime * 30f)
        {
            UpdateData();
            LogData();
            _counter = 0;
        }
        _counter += Time.deltaTime;
    }

    public int RhythmStatus()
    {
        if (_PlayerOneData.RhythmState)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    private void UpdateData()
    {
        _DeltaSpeed = _GroupMan.GetDeltaSpeed();
        _SeparationDistance = _GroupMan.GetSeparationDistance();
        _AverageCycleDuration = _GroupMan.GetAverageCycleForBoth();
        _RhythmState = RhythmStatus();
    }
    private void LogData()
    {
        XPXRManager.Recorder.AddInternalEvent(XPXR.Recorder.Models.SystemType.QuantitativeValue,"LocalPlayerData","LocalSpeed", new QuantitativeValue(_PlayerOneData.Speed) );
        XPXRManager.Recorder.AddInternalEvent(XPXR.Recorder.Models.SystemType.QuantitativeValue,"LocalPlayerData","LocalCycleDuration", new QuantitativeValue(_PlayerOneData.CycleDuration) );
        ////////////////////////////////////////////////////////////////// CHECK THIS ////////////////////////////////////////////////////////////////////
        XPXRManager.Recorder.AddInternalEvent(XPXR.Recorder.Models.SystemType.WorldPosition,"LocalPlayerData","LocalDirection", new WorldPosition(_PlayerOneData.Direction,transform.rotation) );
        ////////////////////////////////////////////////////////////////// CHECK THIS ////////////////////////////////////////////////////////////////////
        
        XPXRManager.Recorder.AddInternalEvent(XPXR.Recorder.Models.SystemType.QuantitativeValue,"RemotePlayerData","RemoteSpeed", new QuantitativeValue(_PlayerTwoData.Speed) );
        XPXRManager.Recorder.AddInternalEvent(XPXR.Recorder.Models.SystemType.QuantitativeValue,"RemotePlayerData","RemoteCycleDuration", new QuantitativeValue(_PlayerTwoData.CycleDuration) );
        ////////////////////////////////////////////////////////////////// CHECK THIS ////////////////////////////////////////////////////////////////////
        XPXRManager.Recorder.AddInternalEvent(XPXR.Recorder.Models.SystemType.WorldPosition,"RemotePlayerData","RemoteDirection", new WorldPosition(_PlayerTwoData.Direction,transform.rotation) );
        ////////////////////////////////////////////////////////////////// CHECK THIS ////////////////////////////////////////////////////////////////////

        ///GROUP DATA
        XPXRManager.Recorder.AddInternalEvent(XPXR.Recorder.Models.SystemType.QuantitativeValue,"GroupData","GrpSeparationDistance", new QuantitativeValue(_SeparationDistance) );
        XPXRManager.Recorder.AddInternalEvent(XPXR.Recorder.Models.SystemType.QuantitativeValue,"GroupData","GrpAverageCycleDuration", new QuantitativeValue(_AverageCycleDuration) );
        XPXRManager.Recorder.AddInternalEvent(XPXR.Recorder.Models.SystemType.QuantitativeValue,"GroupData","GrpDeltaSpeed", new QuantitativeValue(_DeltaSpeed) );

        ///STATES
        ///-1 is BrokenAura, 0 is Aura NOT Broken but NOT enabled, 1 is Aura is Enabled BUT INVIEW, 2 is Aura is Enabled and Visualizing
        XPXRManager.Recorder.AddInternalEvent(XPXR.Recorder.Models.SystemType.QuantitativeValue,"FeatureState","FAuraEnabled", new QuantitativeValue(_GroupMan.AuraCodedValue) );
        // XPXRManager.Recorder.AddInternalEvent(XPXR.Recorder.Models.SystemType.QuantitativeValue,"FeatureState","SeeingOtherPlayer", new QuantitativeValue(_FootstepsMan.InViewStatus() ) );
        XPXRManager.Recorder.AddInternalEvent(XPXR.Recorder.Models.SystemType.QuantitativeValue,"FeatureState","FRhythmEnabled", new QuantitativeValue( _RhythmState ) );
        //XPXRManager.Recorder.AddInternalEvent(XPXR.Recorder.Models.SystemType.QuantitativeValue,"FeatureState","OneOftheLocksEnabled", new QuantitativeValue(_PlayerFeedbackMan.OneOfTheLocksIsOn ) );
        ///
    }
}
