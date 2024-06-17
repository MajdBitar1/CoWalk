using Fusion;
using Oculus.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

public class Armswing : MonoBehaviour
{
    /// <summary>
    /// Use the Character Controller to Implement Armswinging utilizing the XR Interaction toolkit
    /// </summary>
    [Header("Player Obj Ref")]
    [SerializeField] GameObject lefthand;
    [SerializeField] GameObject righthand;
    [SerializeField] GameObject head;
    [SerializeField] GameObject hips;
    [SerializeField] PlayerController _playercontroller;
    private Vector3 prevPosLeft, prevPosRight, prevHipDirection;
    private Vector3 direction, HipDirection,HeadDirection;
    private Vector3 PlayerCurrentPosition, PlayerPreviousFramePosition;
    public PlayerMovementData _playermovementdata;

    [Header("Track These")]
    [SerializeField] float playerspeed;
    [SerializeField] float playerprevspeed;
    [SerializeField] float LeftDistanceMoved = 0;
    [SerializeField] float RightDistanceMoved = 0;


    [Header("Constants")]
    [SerializeField] float SpeedAmplifier = 50f;
    [SerializeField] float friction = 1f;
    [SerializeField] float RequiredMovementThreshold = 0.002f;
    [SerializeField] float MinimumPlayerSpeedThreshold = 0.02f;
    [SerializeField] float MaximumPlayerSpeedThreshold = 20f;
    [SerializeField] float RotationDetectionThreshold = 0.98f;
    [SerializeField] float DifferenceHeadandDirection = 0.6f;
    [SerializeField] float ReduceDistanceMoved = 1;
    //[SerializeField] float maxheight = 0.7f;


    // Start is called before the first frame update
    void Start()
    {
        InitializeSwinger();
    }

    public void InitializeSwinger()
    {
        HipDirection = hips.gameObject.transform.forward.normalized;
        HeadDirection = head.gameObject.transform.forward.normalized;
        PlayerPreviousFramePosition = transform.localPosition;
        prevPosLeft = lefthand.transform.localPosition;
        prevPosRight = righthand.transform.localPosition;
        prevHipDirection = HipDirection;
        playerspeed = 0;
    }

    public PlayerMovementData ComputeMovement(bool LeftLocked, bool RightLocked)
    {
        //Update Position
        PlayerCurrentPosition = transform.position;
        float headheight = head.transform.localPosition.y;

        //Define Direction
        HeadDirection = head.gameObject.transform.forward.normalized;
        HipDirection = hips.gameObject.transform.forward.normalized;
        direction = HipDirection;

        Vector3 NormalVec = Vector3.Cross(direction, Vector3.up).normalized;
        if (!LeftLocked)
        {
            LeftDistanceMoved = ComputeLeftHandMovement(headheight,NormalVec);
        }

        if  (!RightLocked)
        {
            RightDistanceMoved = ComputeRightHandMovement(headheight,NormalVec);
        }

        float playerDistanceMoved = Vector3.Distance(PlayerCurrentPosition, PlayerPreviousFramePosition);

        float totalmovement = LeftDistanceMoved + RightDistanceMoved - 2 * playerDistanceMoved;

        playerspeed = playerprevspeed * (0.9f - friction * playerprevspeed) + totalmovement; // * 1 / (1 + Mathf.Exp(-MovingTime));
        //Reduce Speed For these conditions
        if (playerspeed < MinimumPlayerSpeedThreshold)
        {
            playerspeed = 0;
        }

        if ( playerspeed > MaximumPlayerSpeedThreshold)
        {
            playerspeed = MaximumPlayerSpeedThreshold;
        }

        if (LeftDistanceMoved != 0 && RightDistanceMoved != 0) playerspeed = playerspeed * 1.5f;
        playerspeed = playerspeed * SpeedAmplifier;

        if (Vector3.Dot(HipDirection.normalized, HeadDirection.normalized) < DifferenceHeadandDirection)
        {
            playerspeed = playerspeed * 0.8f;
        }

        if (Vector3.Dot(HipDirection.normalized, prevHipDirection.normalized) < RotationDetectionThreshold)
        {
            //Debug.Log("Rotation Detected");
            playerspeed = playerspeed * 0.01f;
        }

        
        _playermovementdata = new PlayerMovementData(transform.position, direction, playerspeed, 1);

        //Setup parameters for next frame
        prevPosLeft = lefthand.transform.position;
        prevPosRight = righthand.transform.position;
        prevHipDirection = HipDirection;
        PlayerPreviousFramePosition = PlayerCurrentPosition;
        playerprevspeed = playerspeed;

        return _playermovementdata;
    }

    //TO DO
    //MAKE THEM ONE FUCITON WITH LEFTHAND OR RIGHTHAND AS INPUT, 
    //BUT THIS MAY CAUSE SOME PROBLEMS WITH VARIABLES, U CAN PASS A BOOL TO SPECIFY WHICH VARIALBES TO ACCESS IN AN 2D ARRAY.
    private float ComputeLeftHandMovement(float headheight,Vector3 NormalVec)
    {
        Vector3 DeltaLeftHand = lefthand.transform.position - prevPosLeft;
        Vector3 VeloLeftHand = DeltaLeftHand; // (Time.deltaTime * 100);
        //if (lefthand.transform.position.y > headheight * maxheight) VeloLeftHand = Vector3.zero;
        float LeftDistanceMoved = Mathf.Abs( Vector3.Dot(VeloLeftHand, direction));
        if ( Vector3.Dot(NormalVec, VeloLeftHand.normalized) >= 0.5) LeftDistanceMoved = 0;
        LeftDistanceMoved = LeftDistanceMoved / (Time.deltaTime * ReduceDistanceMoved);	
        if (LeftDistanceMoved < RequiredMovementThreshold) LeftDistanceMoved = 0;
        return LeftDistanceMoved;
    }

    private float ComputeRightHandMovement(float headheight,Vector3 NormalVec)
    {
        Vector3 DeltaRightHand = righthand.transform.position - prevPosRight;
        Vector3 VeloRightHand = DeltaRightHand; // (Time.deltaTime * 100);
        //if (righthand.transform.position.y > headheight * maxheight) VeloRightHand = Vector3.zero;
        RightDistanceMoved = Mathf.Abs( Vector3.Dot(VeloRightHand, direction));
        if (Vector3.Dot(NormalVec, VeloRightHand.normalized) >= 0.5) RightDistanceMoved = 0;
        RightDistanceMoved = RightDistanceMoved / (Time.deltaTime * ReduceDistanceMoved);
        if (RightDistanceMoved < RequiredMovementThreshold) RightDistanceMoved = 0;
        return RightDistanceMoved;
    }

    public Vector3 GetDirection()
    {
        return direction;
    }

    public PlayerMovementData GetMovementData()
    {
        PlayerMovementData Data = new PlayerMovementData();
        Data.Position = transform.position;
        Data.Direction = direction;
        Data.Speed = playerspeed;
        Data.CycleDuration = 2;
        return Data;
    }

    public void UpdateAmplifier(float value)
    {
        SpeedAmplifier = 200 * value;
    }
}

