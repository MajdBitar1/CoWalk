using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using XPXR.Recorder.Models;

public class DataCollection : MonoBehaviour
{
    [SerializeField] GroupManager _GroupMan;
    [SerializeField] PlayerFeedbackManager _PlayerFeedbackMan;
    [SerializeField] FootstepsManager _FootstepsMan;
    [SerializeField] Toggle _TracingStateToggle;
    private PlayerMovementData _PlayerOneData,_PlayerTwoData;
    private float _SeparationDistance;
    private float _AverageCycleDuration;
    private int _RhythmState = 0;
    private float _DeltaSpeed;
    private int _counter = 0;
    private bool _TracingState = false;
    
    void OnEnable()
    {
        GameManager.OnPlayerListUpdated += ChangeTracingState;
    }
    void OnDisable()
    {
        GameManager.OnPlayerListUpdated -= ChangeTracingState;
    }
    void OnApplicationQuit()
    {
        if(_TracingState)
        {
            StopTracing();
        }
    }
    public void ChangeTracingState()
    {
        _TracingState = !_TracingState;
        if(_TracingState)
        {
            StartTracing();
        }
        else
        {
            StopTracing();
        }
        _TracingStateToggle.isOn = _TracingState;
    }
    private void StartTracing()
    {
        XPXRManager.Recorder.StartSession();
    }
    private void StopTracing()
    {
        StartCoroutine( EndTrackingRoutine() );
    }
    IEnumerator EndTrackingRoutine()
    {
        yield return new WaitUntil(() => XPXRManager.Recorder.TransfersState() == 0);
        XPXRManager.Recorder.EndTracing();
        _TracingState = false;
    }

    void LateUpdate()
    {
        if (!_TracingState) return;
        if(_counter >= 30)
        {
            UpdateData();
            LogData();
            _counter = 0;
        }
        _counter++;
    }

    public int RhythmStatus()
    {
        if (_GroupMan.GetRhythmState() )
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
        _PlayerOneData = _GroupMan.GetPlayerOneData();
        _PlayerTwoData = _GroupMan.GetPlayerTwoData();
        _DeltaSpeed = _GroupMan.GetDeltaSpeed();
        _SeparationDistance = _GroupMan.GetSeparationDistance();
        _AverageCycleDuration = _GroupMan.GetAverageCycleForBoth();
        _RhythmState = RhythmStatus();
    }
    private void LogData()
    {
        XPXRManager.Recorder.AddInternalEvent(XPXR.Recorder.Models.SystemType.QuantitativeValue,"LocalPlayerData","Speed", new QuantitativeValue(_PlayerOneData.Speed) );
        XPXRManager.Recorder.AddInternalEvent(XPXR.Recorder.Models.SystemType.QuantitativeValue,"LocalPlayerData","CycleDuration", new QuantitativeValue(_PlayerOneData.CycleDuration) );
        ////////////////////////////////////////////////////////////////// CHECK THIS ////////////////////////////////////////////////////////////////////
        XPXRManager.Recorder.AddInternalEvent(XPXR.Recorder.Models.SystemType.WorldPosition,"LocalPlayerData","Direction", new WorldPosition(_PlayerOneData.Direction,transform.rotation) );
        ////////////////////////////////////////////////////////////////// CHECK THIS ////////////////////////////////////////////////////////////////////
        
        XPXRManager.Recorder.AddInternalEvent(XPXR.Recorder.Models.SystemType.QuantitativeValue,"RemotePlayerData","Speed", new QuantitativeValue(_PlayerTwoData.Speed) );
        XPXRManager.Recorder.AddInternalEvent(XPXR.Recorder.Models.SystemType.QuantitativeValue,"RemotePlayerData","CycleDuration", new QuantitativeValue(_PlayerTwoData.CycleDuration) );
        ////////////////////////////////////////////////////////////////// CHECK THIS ////////////////////////////////////////////////////////////////////
        XPXRManager.Recorder.AddInternalEvent(XPXR.Recorder.Models.SystemType.WorldPosition,"RemotePlayerData","Direction", new WorldPosition(_PlayerTwoData.Direction,transform.rotation) );
        ////////////////////////////////////////////////////////////////// CHECK THIS ////////////////////////////////////////////////////////////////////

        ///GROUP DATA
        XPXRManager.Recorder.AddInternalEvent(XPXR.Recorder.Models.SystemType.QuantitativeValue,"GroupData","SeparationDistance", new QuantitativeValue(_SeparationDistance) );
        XPXRManager.Recorder.AddInternalEvent(XPXR.Recorder.Models.SystemType.QuantitativeValue,"GroupData","AverageCycleDuration", new QuantitativeValue(_AverageCycleDuration) );
        XPXRManager.Recorder.AddInternalEvent(XPXR.Recorder.Models.SystemType.QuantitativeValue,"GroupData","DeltaSpeed", new QuantitativeValue(_DeltaSpeed) );

        ///STATES
        ///-1 is BrokenAura, 0 is Aura NOT Broken but NOT enabled, 1 is Aura is Enabled BUT INVIEW, 2 is Aura is Enabled and Visualizing
        XPXRManager.Recorder.AddInternalEvent(XPXR.Recorder.Models.SystemType.QuantitativeValue,"FeatureState","AuraEnabled", new QuantitativeValue(_PlayerFeedbackMan.AuraStatus() ) );
        // XPXRManager.Recorder.AddInternalEvent(XPXR.Recorder.Models.SystemType.QuantitativeValue,"FeatureState","SeeingOtherPlayer", new QuantitativeValue(_FootstepsMan.InViewStatus() ) );
        XPXRManager.Recorder.AddInternalEvent(XPXR.Recorder.Models.SystemType.QuantitativeValue,"FeatureState","RhythmEnabled", new QuantitativeValue( _RhythmState ) );
        //XPXRManager.Recorder.AddInternalEvent(XPXR.Recorder.Models.SystemType.QuantitativeValue,"FeatureState","OneOftheLocksEnabled", new QuantitativeValue(_PlayerFeedbackMan.OneOfTheLocksIsOn ) );
        ///

    }
}
