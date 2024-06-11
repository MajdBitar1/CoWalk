using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerTwoTP : MonoBehaviour
{
    public delegate void PlayerTwoEnterPlatform();
    public delegate void PlayerTwoExitPlatform();
    public static event PlayerTwoEnterPlatform OnPlayerTwoEnterPlatform;
    public static event PlayerTwoExitPlatform OnPlayerTwoExitPlatform;
    private Collider m_collider;

    void Start()
    {
        m_collider = GetComponent<Collider>();
        m_collider.isTrigger = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            //RAISE EVENT THAT P1 IS READY TO TELEPORT
            OnPlayerTwoEnterPlatform();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            //RAISE EVENT THAT P1 IS NOT READY ANYMORE TO TELEPORT
            OnPlayerTwoExitPlatform();
        }
    }
}
