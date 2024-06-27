using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("Input Data")]
    [SerializeField] GameObject Player;
    private Armswing _playerarmswinger;
    private ComputeArmRhythm _playerarmrhythm;
    public PlayerMovementData movementdata;

    // private GestureManager _playergesturemanager;
    // private GestureData _gesturedata;

    [Header("Output Data")]
    [SerializeField] float GroupSpeedModifier;

    private void Start()
    {
        _playerarmswinger = Player.GetComponent<Armswing>();
        _playerarmrhythm = Player.GetComponent<ComputeArmRhythm>();

        // _playergesturemanager = Player.GetComponent<GestureManager>();
    }

    void Update()
    {
        //movementdata = _playerarmswinger.GetMovementData();
        //movementdata.CycleDuration = _playerarmrhythm.GetAverageCycleDuration();

        
        //under some condition call _playergesturemanager.SpeedTuning(GroupSpeedModifier);
    }
}
