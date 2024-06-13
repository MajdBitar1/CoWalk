using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class Rhythm : MonoBehaviour
{
    [SerializeField] PlayerController m_playercontroller;
    [SerializeField] PlayerFeedbackManager m_playerfeedback;
    [SerializeField] GameObject m_lefthand, m_righthand;
    [SerializeField] float ResetAfterTime=4f;
    [SerializeField] float MinimumCycleDuration = 0.4f;
    [SerializeField] float MaxCycleDuration = 3f;
    [SerializeField] int BufferSize = 20;
    
    public List<float> RightCycleDuration = new List<float>();
    public List<float> LeftCycleDuration = new List<float>();

    public List<float> RightCycle = new List<float>();
    public List<float> LeftCycle = new List<float>();
    private float _rightsum, _leftsum;
    private bool _isMoving = false;
    private float averagecycleduration;
    private float ResetTimer = 0;
    private Queue<float> Right_Swing_Elevation_Buffer;
    private Queue<float> Left_Swing_Elevation_Buffer;
    private float _prevrightsum ,_prevleftsum;
    private bool RightCycleState, LeftCycleState;
    private float temprightcycle, templeftcycle;
    private int _ResetBuffersCounter = 0;
    // Start is called before the first frame update

    void Awake()
    {
        Right_Swing_Elevation_Buffer = new Queue<float>(BufferSize);
        Left_Swing_Elevation_Buffer = new Queue<float>(BufferSize);
    }
    void Start()
    {        
        m_playercontroller = GetComponent<PlayerController>();
        m_lefthand = m_playercontroller.GetLeftHand();
        m_righthand = m_playercontroller.GetRightHand();
        m_playerfeedback = m_playercontroller.GetPlayerFeedbackManager();
        ResetParametersAndBuffers();
    }

    void OnDisable()
    {
        ResetParametersAndBuffers();
    }

    // Update is called once per frame
    public void Update()
    {
        _isMoving = m_playercontroller.isMoving;
        ComputeRhythm();
    }
    private void ComputeRhythm()
    {
        if (_isMoving)
        {
            ResetTimer = 0;
            ComputeRightRhythm();
            ComputeLeftRhythm();
            _prevrightsum = _rightsum;
            _prevleftsum = _leftsum;
        }

        //Reset the parameters and buffers if the player is not moving for a certain Time
        else
        {
            ResetTimer += Time.deltaTime;
            if (ResetTimer > ResetAfterTime)
            {
                ResetParametersAndBuffers();
            }
        }
    }

    void ComputeRightRhythm()
    {
        //if buffer is full, remove the first element and add the new element
        //removing first element means subtracting it from the Sum
        //adding new element means adding it to the sum
        _rightsum += m_righthand.transform.localPosition.y;
        if (RightCycle.Count == BufferSize)
        {
            float value = _rightsum - _prevrightsum;
            if (!RightCycleState && (_rightsum - _prevrightsum > 0) )
            {
                    RightCycleChanged();
            }
            else if ( RightCycleState && (_rightsum - _prevrightsum < 0) )
            {
                RightCycleChanged();
            }
            _prevrightsum = _rightsum;
            RightCycle.Clear();
        }
    }

    void ComputeLeftRhythm()
    {
        if (Left_Swing_Elevation_Buffer.Count == BufferSize)
        {
            _leftsum -= Left_Swing_Elevation_Buffer.Dequeue();
        }
        Left_Swing_Elevation_Buffer.Enqueue(m_lefthand.transform.localPosition.y);
        _leftsum += m_lefthand.transform.localPosition.y;

        if (LeftCycleState && (_leftsum - _prevleftsum < 0))
        {
            LeftCycleChanged();
        }
        else if (!LeftCycleState && (_leftsum - _prevleftsum > 0))
        {
            LeftCycleChanged();
        }
    }
    private void RightCycleChanged()
    {
        RightCycleState = !RightCycleState;
        // Cycle Started, so we add time to it
        if (RightCycleState)
        { 
            temprightcycle += Time.deltaTime;
        }
        //Cycle Ended, so we add cycle duration to list and play Audio of local footstep
        else
        {
            Debug.Log("[RhythmDetection] Right Cycle Duration: " + temprightcycle);
            if (temprightcycle > MinimumCycleDuration && temprightcycle < MaxCycleDuration)
            {
                RightCycleDuration.Add(temprightcycle);
                m_playerfeedback.PlayFootstepSound();   
            }
            //BROADCAST DATA TO OTHER PLAYERS
            BroadcastChange();
            temprightcycle = 0;
        }
    }

    private void LeftCycleChanged()
    {
        LeftCycleState = !LeftCycleState;
        if (LeftCycleState) 
        {
            templeftcycle += Time.deltaTime;
        }
        else
        {
            Debug.Log("[RhythmDetection] Left Cycle Duration: " + templeftcycle);
            if (templeftcycle > MinimumCycleDuration && templeftcycle < MaxCycleDuration)
            {
                LeftCycleDuration.Add(templeftcycle);
                m_playerfeedback.PlayFootstepSound();
            }
            BroadcastChange();
            templeftcycle = 0;
        }
    }

    private void BroadcastChange()
    {
        if (RightCycleDuration.Count != 0)
        {
            foreach (float item in RightCycleDuration)
            {
                averagecycleduration += item;
            }
        }
        if (LeftCycleDuration.Count != 0)
        {           
            foreach (float item in LeftCycleDuration)
            {
                averagecycleduration += item;
            }
        }
        averagecycleduration /= RightCycleDuration.Count + LeftCycleDuration.Count;
        m_playercontroller.PlayerCycleDuration = averagecycleduration;
        _ResetBuffersCounter++;
        if (_ResetBuffersCounter >= 20)
        {
            RightCycleDuration.Clear();
            LeftCycleDuration.Clear();
        }
    }

    private void ResetParametersAndBuffers()
    {
        RightCycleState = false;
        LeftCycleState = false;
        _rightsum = 0;
        _leftsum = 0;
        _prevrightsum = 0;
        _prevleftsum = 0;
        temprightcycle = 0;
        templeftcycle = 0;
        averagecycleduration = 0;
        _ResetBuffersCounter = 0;
        Right_Swing_Elevation_Buffer.Clear();
        Left_Swing_Elevation_Buffer.Clear();
        RightCycleDuration.Clear();
        LeftCycleDuration.Clear();
    }
    public float GetAverageCycleDuration()
    {
        return averagecycleduration;
    }
}
