using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(PlayerController))]
public class PlayerInputHandler : MonoBehaviour
{
    private PlayerController m_playercontroller;
    [SerializeField] private InputActionReference m_LeftHandGrip;
    [SerializeField] private InputActionReference m_RightHandGrip;
    [SerializeField] private InputActionReference m_LeftHandTrigger;
    [SerializeField] private InputActionReference m_RightHandTrigger;
    public int PrevState = 0;
    public int CurrentState = 0;


    // Start is called before the first frame update
    void Start()
    {
        m_playercontroller = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        CurrentState = CheckState();
        CheckStates(CurrentState, PrevState);
        PrevState = CurrentState;
    }


    private int CheckState()
    {
        if ( OVRInput.GetUp(OVRInput.Button.Three) ) // Lock Left
        {
            return 1;
        }
        else if (m_LeftHandGrip.action.IsPressed() && !m_LeftHandTrigger.action.IsPressed()) // Left Pointing
        {
            return 2;
        }
        else if (m_RightHandGrip.action.IsPressed() && !m_RightHandTrigger.action.IsPressed()) // Right Pointing
        {
            return 3;
        }
        else if ( OVRInput.GetUp(OVRInput.Button.One) ) // Lock Right
        {
            return 4;
        }

        else if (OVRInput.GetUp(OVRInput.Button.Two) || OVRInput.GetUp(OVRInput.Button.Four) ) // Show Menu
        {
            return 5;
        }

        return 0;
    }

    
    private void CheckStates(int current, int prev)
    {
        switch (current)
        {
            case 1:
                //lockLEFT
                m_playercontroller.LeftLocked = !m_playercontroller.LeftLocked;
                m_playercontroller.UpdateLock();
                break;
            case 2:
                //DrawLineLEFT
                if (prev != 3)
                {
                    m_playercontroller.IsPointingLeft = true;
                }
                break;
            case 3:
                //DrawLineRIGHT
                if (prev != 2)
                {
                    m_playercontroller.IsPointingRight = true;
                }
                break;
            case 4:
                //lockRIGHT
                m_playercontroller.RightLocked = !m_playercontroller.RightLocked;
                m_playercontroller.UpdateLock();
                break;
            case 5:
                //ShowMenu
                m_playercontroller.ShowMenu();
                break;
            default:
                // if ( prev == 1 )
                // {
                //     //Unlock
                //     m_playercontroller.isMoving = true;
                // }
                if (prev == 2)
                {
                    //Disable Line
                    Debug.Log("[INPUT HANDLER] STOPPED LEFT POINTING");
                    m_playercontroller.ReleasedLeftPointing = true;
                }
                if (prev == 3)
                {
                    //Disable Line
                    Debug.Log("[INPUT HANDLER] STOPPED RIGHT POINTING");
                    m_playercontroller.ReleasedRightPointing = true;
                }
                break;
        }
    }
}
