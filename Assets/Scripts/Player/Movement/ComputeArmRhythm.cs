using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class ComputeArmRhythm : MonoBehaviour
{
    private PlayerController m_playercontroller;
    private PlayerFeedbackManager m_playerfeedback;
    private GameObject m_lefthand, m_righthand;
    [SerializeField] float ResetAfterTime=4f;
    [SerializeField] float MinimumCycleDuration = 0.4f;
    [SerializeField] float MaxCycleDuration = 3f;
    [SerializeField] int BufferSize = 30;
    [SerializeField] private float BroadcastTimeInvertal = 1f;
    [SerializeField] private float _DefaultCycleValue = 1f;
    public float MinimumSwingChange = -0.1f;
    private float GlobalTimer = 0f;
    private float totaldurationright, totaldurationleft;
    private int totalcountright, totalcountleft;
    private float _rightsum, _leftsum;
    private float _prevrightsum ,_prevleftsum;
    private float averagecycleduration;
    private float ResetTimer = 0;
    private Queue<float> Right_Swing_Elevation_Buffer;
    private Queue<float> Left_Swing_Elevation_Buffer;
    private bool ActivateCycleRight, ActivateCycleLeft;
    private float temprightcycle, templeftcycle;
    void Awake()
    {
        Right_Swing_Elevation_Buffer = new Queue<float>(BufferSize);
        Left_Swing_Elevation_Buffer = new Queue<float>(BufferSize);
        totalcountleft = 0;
        totalcountright = 0;
        totaldurationleft = 0;
        totaldurationright = 0;
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
        ComputeRhythm();
    }
    private void ComputeRhythm()
    {
        if (m_playercontroller.PlayerAverageSpeed > 0f)
        {
            ResetTimer = 0;
            ComputeRightRhythm();
            ComputeLeftRhythm();
            UpdateTimers();
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
        if (Right_Swing_Elevation_Buffer.Count == BufferSize)
        {
            _rightsum -= Right_Swing_Elevation_Buffer.Dequeue();
        }
        Right_Swing_Elevation_Buffer.Enqueue(m_righthand.transform.localPosition.y);
        _rightsum += m_righthand.transform.localPosition.y;
        if  (_rightsum - _prevrightsum < MinimumSwingChange) 
        {
            RightCycleChanged();
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
        if (_leftsum - _prevleftsum < -MinimumSwingChange)
        {
            LeftCycleChanged();
        }
    }
    private void RightCycleChanged()
    {
        ActivateCycleRight = !ActivateCycleRight;
        //Cycle Ended, so we add cycle duration to list and play Audio of local footstep
        if (!ActivateCycleRight)
        {
            if (temprightcycle > MinimumCycleDuration)
            {
                // RightCycleDuration.Add(temprightcycle);
                totaldurationright += temprightcycle;
                totalcountright++;
                temprightcycle = 0;
            }
            if (temprightcycle > MaxCycleDuration)
            {
                temprightcycle = 0;
            }
        }
    }

    private void LeftCycleChanged()
        {
        ActivateCycleLeft = !ActivateCycleLeft;
        if (!ActivateCycleLeft)
        {
            if (templeftcycle > MinimumCycleDuration)
            {
                // LeftCycleDuration.Add(templeftcycle);
                totaldurationleft += templeftcycle;
                totalcountleft++;
                templeftcycle = 0;
            }
            if (templeftcycle > MaxCycleDuration)
            {
                templeftcycle = 0;
            }
        }
    }

    private void UpdateTimers()
    {
        if (ActivateCycleRight)
        { 
            temprightcycle += Time.deltaTime;
        }
        if (ActivateCycleLeft) 
        {
            templeftcycle += Time.deltaTime;
        }

        GlobalTimer += Time.deltaTime;
        if (GlobalTimer > BroadcastTimeInvertal)
        {
            CalculateAverageCycleDuration();
            GlobalTimer = 0;
        }
    }

    private void CalculateAverageCycleDuration()
    {
        float sum = 0;

        //add all cycles together
        if (totalcountright > 0)
        {
            sum += totaldurationright;
        }
        if (totalcountleft > 0)
        {
            sum += totaldurationleft;
        }

        //Divide by total count to get average

        if (totalcountleft + totalcountright == 0)
        {
            averagecycleduration = _DefaultCycleValue;
        }
        else
        {
            averagecycleduration = sum / (totalcountright + totalcountleft);
        }

        Debug.Log("Average Cycle Duration: " + averagecycleduration);
        //Broadcast this average
        m_playercontroller.PlayerCycleDuration = averagecycleduration;

        //Reset the parameters and buffers
        ResetAfterBroadcast();
    }

    private void ResetAfterBroadcast()
    {
        totalcountleft = 0;
        totalcountright = 0;
        totaldurationleft = 0;
        totaldurationright = 0;
    }
    private void ResetParametersAndBuffers()
    {
        _rightsum = 0;
        _leftsum = 0;
        _prevrightsum = 0;
        _prevleftsum = 0;
        temprightcycle = 0;
        templeftcycle = 0;
        averagecycleduration = 0;
        Right_Swing_Elevation_Buffer.Clear();
        Left_Swing_Elevation_Buffer.Clear();
        ResetAfterBroadcast();
    }
}
