using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Unity.VisualScripting;

[RequireComponent(typeof(PlayerController))]
public class ComputeArmRhythm : MonoBehaviour
{
    private PlayerController m_playercontroller;
    private GameObject m_lefthand, m_righthand;
    [SerializeField] float ResetAfterTime=4f;
    [SerializeField] float MinimumCycleDuration = 0.4f;
    [SerializeField] float MaxCycleDuration = 3f;

    public bool _isMoving = false;
    public float averagecycleduration;

    private bool Reset = true;
    private float ResetTimer = 0;
    private Queue<float> Right_Swing_Elevation_Buffer = new Queue<float>(60);
    private Queue<float> Left_Swing_Elevation_Buffer = new Queue<float>(60);
    public List<float> RightCycleDuration = new List<float>();
    public List<float> LeftCycleDuration = new List<float>();

    public float _rightsum, _leftsum;
    private float _prevrightsum ,_prevleftsum;
    private bool RightCycleState, LeftCycleState;

    private float temprightcycle;
    private float templeftcycle;
    // Start is called before the first frame update
    void Start()
    {
        m_playercontroller = GetComponent<PlayerController>();
        m_lefthand = m_playercontroller.GetLeftHand();
        m_righthand = m_playercontroller.GetRightHand();
        ResetParametersAndBuffers();
    }

    // Update is called once per frame
    public void Update()
    {
        _isMoving = m_playercontroller.isMoving;
        ComputeRhythm();
        UpdateCycle();
    }

    private void OnEnable()
    {
        TimeEventManager.OnTimeEvent += CheckTimer;
    }
    private void OnDisable()
    {
        TimeEventManager.OnTimeEvent -= CheckTimer;
    }
    private void CheckTimer()
    {
        if (RightCycleDuration.Count != 0 || LeftCycleDuration.Count != 0)
        {
            foreach (float item in RightCycleDuration)
            {
                averagecycleduration += item;
            }
            foreach (float item in LeftCycleDuration)
            {
                averagecycleduration += item;
            }
            averagecycleduration /= (RightCycleDuration.Count + LeftCycleDuration.Count);
            RightCycleDuration.Clear();
            LeftCycleDuration.Clear();
        }
    }
    private void ComputeRhythm()
    {
        if (_isMoving)
        {
            Reset = false;
            ResetTimer = 0;
            if (Right_Swing_Elevation_Buffer.Count == 60)
            {
                _rightsum -= Right_Swing_Elevation_Buffer.Dequeue();
            }
            Right_Swing_Elevation_Buffer.Enqueue(m_righthand.transform.localPosition.y);
            _rightsum += m_righthand.transform.localPosition.y;

            if (!RightCycleState && (_rightsum - _prevrightsum < 0))
            {
                RightCycleState = !RightCycleState;
            }
            else if (RightCycleState && (_rightsum - _prevrightsum > 0))
            {
                RightCycleState = !RightCycleState;
            }

            if (Left_Swing_Elevation_Buffer.Count == 60)
            {
                _leftsum -= Left_Swing_Elevation_Buffer.Dequeue();
            }
            Left_Swing_Elevation_Buffer.Enqueue(m_lefthand.transform.localPosition.y);
            _leftsum += m_lefthand.transform.localPosition.y;

            if (!LeftCycleState && (_leftsum - _prevleftsum < 0))
            {
                LeftCycleState = !LeftCycleState;
            }
            else if (LeftCycleState && (_leftsum - _prevleftsum > 0))
            {
                LeftCycleState = !LeftCycleState;
            }
            _prevrightsum = _rightsum;
            _prevleftsum = _leftsum;
            //CheckTimer();
        }
        else if(!Reset)
        {
            ResetTimer += Time.deltaTime;
            if (ResetTimer > ResetAfterTime)
            {
                ResetParametersAndBuffers();
            }
        }
    }

    private void UpdateCycle()
    {
        if (RightCycleState) temprightcycle += Time.deltaTime;

        if(!RightCycleState)
        {
            if (temprightcycle > MinimumCycleDuration && temprightcycle < MaxCycleDuration)
            {
                RightCycleDuration.Add(temprightcycle);
            }
            temprightcycle = 0;
        }

        if (LeftCycleState) templeftcycle += Time.deltaTime;
        if (!LeftCycleState)
        {
            if (templeftcycle > MinimumCycleDuration && templeftcycle < MaxCycleDuration)
            {
                LeftCycleDuration.Add(templeftcycle);
            }
            templeftcycle = 0;
        }
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
        RightCycleDuration.Clear();
        LeftCycleDuration.Clear();
        Reset = true;
    }

    public float GetAverageCycleDuration()
    {
        return averagecycleduration;
    }
}
