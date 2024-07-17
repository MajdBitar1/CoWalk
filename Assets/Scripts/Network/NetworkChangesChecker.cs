using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkChangesChecker : MonoBehaviour
{
    [SerializeField] Armswing m_armswing;
    [SerializeField] UIMenuManager m_PlayerFeedbackManager;
    [SerializeField] FootstepsManager m_FootstepsManager;
    [SerializeField] DataCollection _DataCollector;
    private NetworkPlayerInfo m_NetworkPlayerInfo;
    public bool _Experimenter = false;
    private bool _PlayersReady = false;
    // Start is called before the first frame update
    private void OnEnable()
    {
        GameManager.OnPlayerListUpdated += SetupNetworkInfo;
    }
    private void OnDisable()
    {
        GameManager.OnPlayerListUpdated -= SetupNetworkInfo;
    }
    void SetupNetworkInfo()
    {
        m_NetworkPlayerInfo = GameManager.LocalPlayerObject.GetComponent<NetworkPlayerInfo>();
        _PlayersReady = true;
    }

    private void FixedUpdate()
    {
        if (!_PlayersReady) return;
        CheckUpdatesfromUI();
    }


    public void ShowMenu()
    {
        if (_Experimenter)
        {
            m_PlayerFeedbackManager.ShowMenu();
        }
    }

    private void CheckUpdatesfromUI()
    {
        m_armswing.UpdateAmplifier(m_NetworkPlayerInfo.SpeedAmplifier);
        m_FootstepsManager.UpdateDistanceSliders(m_NetworkPlayerInfo.SAFEDIS, m_NetworkPlayerInfo.MAXDIST, m_NetworkPlayerInfo.CSTDIST, m_NetworkPlayerInfo.ADDDIST);
        _DataCollector.ChangeTracingState(m_NetworkPlayerInfo.TracingState);
    }

    public void SetExperimenter(bool state)
    {
        _Experimenter = state;
    }
}
