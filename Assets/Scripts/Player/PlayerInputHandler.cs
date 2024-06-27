using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(PlayerController))]
public class PlayerInputHandler : MonoBehaviour
{
    private PlayerController m_playercontroller;
    // [SerializeField] private InputActionReference m_LeftHandGrip;
    // [SerializeField] private InputActionReference m_RightHandGrip;
    // [SerializeField] private InputActionReference m_LeftHandTrigger;
    // [SerializeField] private InputActionReference m_RightHandTrigger;
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

        else if ( OVRInput.GetUp(OVRInput.Button.One) ) // Lock Right
        {
            return 2;
        }

        else if (OVRInput.GetUp(OVRInput.Button.Two) || OVRInput.GetUp(OVRInput.Button.Four) ) // Show Menu
        {
            return 3;
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
                //lockRIGHT
                m_playercontroller.RightLocked = !m_playercontroller.RightLocked;
                m_playercontroller.UpdateLock();
                break;
            case 3:
                //ShowMenu
                m_playercontroller.ShowMenu();
                break;
            default:
                break;
        }
    }
}
