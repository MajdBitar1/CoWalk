using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using XPXR.Recorder.Models;

public class DataCollection : MonoBehaviour
{
    [SerializeField] GroupManager _GroupMan;
    [SerializeField] PlayerFeedbackManager _PlayerFeedbackMan;
    private PlayerMovementData _PlayerOneData,_PlayerTwoData;
    private float SeparationDistance;
    private float AverageCycleDuration;
    private float DeltaSpeed;
    // Start is called before the first frame update

    private int counter = 0;
    private bool BothPlayersReady = false;
    
    void OnEnable()
    {
        GroupManager.OnPlayersReady += PlayersReadyCheck;
    }
    void OnDisable()
    {
        GroupManager.OnPlayersReady -= PlayersReadyCheck;
    }
    void Start()
    {

    }

    void OnApplicationQuit()
    {
        XPXRManager.Recorder.StopSession();
        StartCoroutine( EndTrackingRoutine() );
    }
    IEnumerator EndTrackingRoutine()
    {
        yield return new WaitUntil(() => XPXRManager.Recorder.TransfersState() == 0);
        XPXRManager.Recorder.EndTracing();
    }

    void PlayersReadyCheck()
    {
        BothPlayersReady = true;
        XPXRManager.Recorder.StartSession();
    }

    void LateUpdate()
    {
        if (!BothPlayersReady) return;
        _PlayerOneData = _GroupMan.GetPlayerOneData();
        _PlayerTwoData = _GroupMan.GetPlayerTwoData();
        DeltaSpeed = _GroupMan.GetDeltaSpeed();
        SeparationDistance = _GroupMan.GetSeparationDistance();
        AverageCycleDuration = _GroupMan.GetAverageCycleForBoth();

        if(counter >= 30)
        {
            LogData();
            counter = 0;
        }
        counter++;
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
        XPXRManager.Recorder.AddInternalEvent(XPXR.Recorder.Models.SystemType.QuantitativeValue,"GroupData","SeparationDistance", new QuantitativeValue(SeparationDistance) );
        XPXRManager.Recorder.AddInternalEvent(XPXR.Recorder.Models.SystemType.QuantitativeValue,"GroupData","AverageCycleDuration", new QuantitativeValue(AverageCycleDuration) );
        XPXRManager.Recorder.AddInternalEvent(XPXR.Recorder.Models.SystemType.QuantitativeValue,"GroupData","DeltaSpeed", new QuantitativeValue(DeltaSpeed) );

        ///STATES
        XPXRManager.Recorder.AddInternalEvent(XPXR.Recorder.Models.SystemType.QuantitativeValue,"FeatureState","AuraEnabled", new QuantitativeValue(_PlayerFeedbackMan.AuraStatus() ) );
        XPXRManager.Recorder.AddInternalEvent(XPXR.Recorder.Models.SystemType.QuantitativeValue,"FeatureState","SeeingOtherPlayer", new QuantitativeValue(_PlayerFeedbackMan.InViewStatus() ) );
        XPXRManager.Recorder.AddInternalEvent(XPXR.Recorder.Models.SystemType.QuantitativeValue,"FeatureState","RhythmEnabled", new QuantitativeValue(_PlayerFeedbackMan.RhythmStatus() ) );
        XPXRManager.Recorder.AddInternalEvent(XPXR.Recorder.Models.SystemType.QuantitativeValue,"FeatureState","OneOftheLocksEnabled", new QuantitativeValue(_PlayerFeedbackMan.OneOfTheLocksIsOn ) );
        ///

    }
}
