using UnityEngine;


public class PlayerFeedbackManager : MonoBehaviour
{
    [Header("Feedback Objects")]
    [SerializeField] GameObject m_StatsMenuObj,m_TuningMenuObj,m_InteractionObj,LeftGestureLock, RightGestureLock;

    [Header("Feedback States")]
    public bool _ShowMenu = false;
    public int AuraCodedState = 0; // -1 is BrokenAura, 0 is Aura NOT Broken but NOT enabled, 1 is Aura is Enabled BUT INVIEW, 2 is Aura is Enabled and Visualizing

    public void ShowMenu()
    {
        _ShowMenu = !_ShowMenu;
        if (_ShowMenu)
        {
            m_InteractionObj.SetActive(true);
            m_StatsMenuObj.gameObject.transform.forward = Camera.main.transform.forward.normalized;
            m_StatsMenuObj.gameObject.transform.position = GameManager.PlayerOne.gameObject.transform.position + Camera.main.transform.forward.normalized * 3  + new Vector3(0, 3f, 0);
            Vector3 InfrontOfPlayer = Vector3.Cross(Camera.main.transform.forward.normalized, Camera.main.transform.up.normalized).normalized;
            m_TuningMenuObj.gameObject.transform.forward = InfrontOfPlayer;
            m_TuningMenuObj.gameObject.transform.position = GameManager.PlayerOne.gameObject.transform.position + InfrontOfPlayer * 3 + new Vector3(0, 3f, 0);
            m_StatsMenuObj.SetActive(true);
            m_TuningMenuObj.SetActive(true);
        }
        else
        {
            m_StatsMenuObj.SetActive(false);
            m_TuningMenuObj.SetActive(false);
            m_InteractionObj.SetActive(false);
        }
    }
    public int AuraStatus()
    {
        return AuraCodedState;
    }
}
