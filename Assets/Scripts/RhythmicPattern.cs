using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ExitGames.Client.Photon.StructWrapping;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class RhythmicPattern : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject m_UpperChestPosition;
    private PlayerController m_playercontroller;   
    private GameObject m_righthand, m_lefthand; 
    private Vector3 prevRightPos, prevLeftPos;
    private Vector3 m_PrevUpperChestpos;
    private Vector3 m_direction;
    private Plane m_Plane;

    public float RightCycleDuration = 0;
    public float LeftCycleDuration = 0;
    public List<float> RightSavedCycle = new List<float>();
    public List<float> LeftSavedCycle = new List<float>();

    // private Mesh mesh;
    // private MeshFilter meshfilter;
    // private MeshRenderer meshrenderer;  

    private void Start()
    {
        m_playercontroller = GetComponent<PlayerController>();
        m_righthand = m_playercontroller.GetRightHand();
        m_lefthand = m_playercontroller.GetLeftHand();
        prevRightPos = Vector3.zero;
        prevLeftPos = Vector3.zero;
        ComputePlane();
    }

    private void Update()
    {
        CheckPattern(m_righthand.transform.position, prevRightPos, true);
        //CheckPattern(m_lefthand.transform.position, prevLeftPos, false);
    }
    private void ComputePlane()
    {
        m_direction = m_playercontroller.GetDirection();
        m_PrevUpperChestpos = m_UpperChestPosition.transform.position;
        //m_PlaneConstant = -1 * Vector3.Dot(m_direction, m_rightshoulderpos);
        m_Plane = new Plane(m_direction, m_PrevUpperChestpos);
    }

    private void CheckPattern(Vector3 point1, Vector3 point2, bool isRightHand)
    {
        if (isRightHand)
        {
            if ( m_Plane.SameSide(point1,point2) )
            {
                RightCycleDuration += Time.deltaTime;
            }
            else 
            {
                if (RightCycleDuration > 0.001)
                {
                    Debug.Log("Right Cycle Ended, Duration: " + RightCycleDuration);
                    RightSavedCycle.Add(RightCycleDuration);
                }
                    //RightSavedCycle.Add(RightCycleDuration);
                
                RightCycleDuration = 0;
                ComputePlane();
            }
        }
        else
        {
            if ( m_Plane.SameSide(point1,point2) )
            {
                LeftCycleDuration += Time.deltaTime;
            }
            else 
            {
                LeftSavedCycle.Add(LeftCycleDuration);
                LeftCycleDuration = 0;
            }
        }
        
    }


}
