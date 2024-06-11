using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlaneCycleDetection : MonoBehaviour
{

    public float RightCycleDuration = 0;
    public float LeftCycleDuration = 0;
    public List<float> RightSavedCycle = new List<float>();
    public List<float> LeftSavedCycle = new List<float>();

    private bool RightCycleState = false;
    private bool LeftCycleState = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ComputeRightCycle(RightCycleState);
        ComputeLeftCycle(LeftCycleState);
    }


    void ComputeRightCycle(bool CycleOn)
    {
        if (CycleOn)
        {
            RightCycleDuration += Time.deltaTime;
        }
        else
        {
            if (RightCycleDuration > 0.01f && RightCycleDuration < 2f)
            {
                RightSavedCycle.Add(RightCycleDuration);
            }
            RightCycleDuration = 0;
        }
    }

     void ComputeLeftCycle(bool CycleOn)
    {
        if (CycleOn)
        {
            LeftCycleDuration += Time.deltaTime;
        }
        else
        {
            if (LeftCycleDuration > 0.01f && LeftCycleDuration < 2f)
            {
                LeftSavedCycle.Add(LeftCycleDuration);
            }
            LeftCycleDuration = 0;
        }
    }

    // void onCollisionEnter(Collision other)
    // {
    //     if (other.gameObject.tag == "hands")
    //     {
    //         Debug.Log("[PCD]Collision Detected WITH HANDS");
    //         if ( other.gameObject.tag == "LeftHand")
    //         {
    //             LeftCycleState = !LeftCycleState;
    //         }

    //         if ( other.gameObject.tag == "RightHand")
    //         {
    //             RightCycleState = !RightCycleState;
    //         }
    //     }
    // }

    void OnCollisionEnter(Collision other)
    {
        Debug.Log("[PCD]Collision Detected");
        if (other.gameObject.layer == 8)
        {
            Debug.Log("[PCD]Collision Detected WITH HANDS");
            if ( other.gameObject.tag == "LeftHand")
            {
                Debug.Log("[PCD]Collision Detected WITH LEFT HANDS");
                LeftCycleState = !LeftCycleState;
            }

            if ( other.gameObject.tag == "RightHand")
            {
                Debug.Log("[PCD]Collision Detected WITH RIGHT HANDS");
                RightCycleState = !RightCycleState;
            }
        }
    }
}
