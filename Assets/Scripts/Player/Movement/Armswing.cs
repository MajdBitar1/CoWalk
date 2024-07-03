using UnityEngine;
[RequireComponent(typeof(PlayerController))]
public class Armswing : MonoBehaviour
{
    /// <summary>
    /// 
    /// </summary>
    [Header("Player Obj Ref")]
    private GameObject _Lefthand;
    private GameObject _RightHand;
    [SerializeField] GameObject _Head;
    [SerializeField] GameObject _Hips;
    [SerializeField] PlayerController _playercontroller;
    private Vector3 prevPosLeft, prevPosRight, prevHipDirection;
    private Vector3 direction, HipDirection,HeadDirection;
    private Vector3 PlayerCurrentPosition, PlayerPreviousFramePosition;
    public PlayerMovementData _playermovementdata;

    [Header("Track These")]
    private float playerspeed;
    private float playerprevspeed;
    private float LeftDistanceMoved = 0;
    private float RightDistanceMoved = 0;


    [Header("Constants")]
    [SerializeField] float SpeedAmplifier = 50f;
    [SerializeField] float friction = 1f;
    [SerializeField] float RequiredMovementThreshold = 0.002f;
    [SerializeField] float MinimumPlayerSpeedThreshold = 0.02f;
    [SerializeField] float MaximumPlayerSpeedThreshold = 20f;
    [SerializeField] float RotationDetectionThreshold = 0.98f;
    [SerializeField] float DifferenceHeadandDirection = 0.6f;

    // Start is called before the first frame update
    void Start()
    {
        InitializeSwinger();
    }

    public void InitializeSwinger()
    {
        _Lefthand = _playercontroller.GetLeftHand();
        _RightHand = _playercontroller.GetRightHand();
        HipDirection = _Hips.gameObject.transform.forward.normalized;
        HeadDirection = _Head.gameObject.transform.forward.normalized;
        PlayerPreviousFramePosition = transform.localPosition;
        prevPosLeft = _Lefthand.transform.localPosition;
        prevPosRight = _RightHand.transform.localPosition;
        prevHipDirection = HipDirection;
        playerspeed = 0;
    }

    public PlayerMovementData ComputeMovement(bool LeftLocked, bool RightLocked)
    {
        //Update Position
        PlayerCurrentPosition = transform.position;
        //Define Direction
        HeadDirection = _Head.gameObject.transform.forward.normalized;
        HipDirection = _Hips.gameObject.transform.forward.normalized;
        direction = HipDirection;

        Vector3 NormalVec = Vector3.Cross(direction, Vector3.up).normalized;
        if (!LeftLocked)
        {
            LeftDistanceMoved = ComputeLeftHandMovement(NormalVec);
        }

        if  (!RightLocked)
        {
            RightDistanceMoved = ComputeRightHandMovement(NormalVec);
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

        if (RightLocked || LeftLocked) playerspeed = playerspeed * 1.75f;
        
        playerspeed = playerspeed * SpeedAmplifier;

        if (Vector3.Dot(HipDirection.normalized, HeadDirection.normalized) < DifferenceHeadandDirection)
        {
            playerspeed = playerspeed * 0.8f;
        }

        if (Vector3.Dot(HipDirection.normalized, prevHipDirection.normalized) < RotationDetectionThreshold)
        {
            //Debug.Log("Rotation Detected");
            playerspeed = playerspeed * 0.001f;
        }

        
        _playermovementdata = new PlayerMovementData(transform.position, direction, playerspeed, 1);

        //Setup parameters for next frame
        prevPosLeft = _Lefthand.transform.position;
        prevPosRight = _RightHand.transform.position;
        prevHipDirection = HipDirection;
        PlayerPreviousFramePosition = PlayerCurrentPosition;
        playerprevspeed = playerspeed;

        return _playermovementdata;
    }

    //TO DO
    //MAKE THEM ONE FUCITON WITH LEFTHAND OR RIGHTHAND AS INPUT, 
    //BUT THIS MAY CAUSE SOME PROBLEMS WITH VARIABLES, U CAN PASS A BOOL TO SPECIFY WHICH VARIALBES TO ACCESS IN AN 2D ARRAY.
    private float ComputeLeftHandMovement(Vector3 NormalVec)
    {
        Vector3 DeltaLeftHand = _Lefthand.transform.position - prevPosLeft;
        Vector3 VeloLeftHand = DeltaLeftHand;
        float LeftDistanceMoved = Mathf.Abs( Vector3.Dot(VeloLeftHand, direction));
        if ( Vector3.Dot(NormalVec, VeloLeftHand.normalized) >= 0.5) LeftDistanceMoved = 0;
        LeftDistanceMoved = LeftDistanceMoved / Time.deltaTime;	
        if (LeftDistanceMoved < RequiredMovementThreshold) LeftDistanceMoved = 0;
        return LeftDistanceMoved;
    }

    private float ComputeRightHandMovement(Vector3 NormalVec)
    {
        Vector3 DeltaRightHand = _RightHand.transform.position - prevPosRight;
        Vector3 VeloRightHand = DeltaRightHand;
        RightDistanceMoved = Mathf.Abs( Vector3.Dot(VeloRightHand, direction));
        if (Vector3.Dot(NormalVec, VeloRightHand.normalized) >= 0.5) RightDistanceMoved = 0;
        RightDistanceMoved = RightDistanceMoved / Time.deltaTime;
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

