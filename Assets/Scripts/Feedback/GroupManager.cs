using UnityEngine;
// GroupManager is a class that manages the group of players, 
// it receives the data from the PlayerManagers of each player 
// computes parameters for the group and then sends back modifications 
// for each player so that they can adjust their movement accordingly and receive proper feedback based on other player's actions.
public class GroupManager : MonoBehaviour
{
    private NetworkPlayerInfo m_NetworkPlayerOneInfo, m_NetworkPlayerTwoInfo;
    private float _SeparationDistance, _DeltaSpeed, _AverageCycleForBoth;
    private bool twoplayersready = false;
    public int AuraCodedValue = 0;

    // Start is called before the first frame update
    private void OnEnable()
    {
        GameManager.OnPlayerListUpdated += PlayersUpdated;
    }
    private void OnDisable()
    {
        GameManager.OnPlayerListUpdated -= PlayersUpdated;
    }
    void Start()
    {
        _SeparationDistance = 0;
        _DeltaSpeed = 0;
        _AverageCycleForBoth = 1;
    }

    private void PlayersUpdated()
    {
        if (GameManager.IsCameraMan)
        {
            m_NetworkPlayerOneInfo = GameManager.PlayerOne.GetComponent<NetworkPlayerInfo>();
            m_NetworkPlayerTwoInfo = GameManager.PlayerTwo.GetComponent<NetworkPlayerInfo>();
        }
        else
        {
            m_NetworkPlayerOneInfo = GameManager.LocalPlayerObject.GetComponent<NetworkPlayerInfo>();
            m_NetworkPlayerTwoInfo = GameManager.RemotePlayerObject.GetComponent<NetworkPlayerInfo>();
        }
        twoplayersready = true;
    }

    void Update()
    {
        if (twoplayersready)
        {
            UpdateGroupParameters();
        }
    }
    // Update is called once per frame
    private void UpdateGroupParameters()
    {
        _SeparationDistance = SeparationDistance2D(m_NetworkPlayerOneInfo.transform.position,m_NetworkPlayerTwoInfo.transform.position);
        _DeltaSpeed = m_NetworkPlayerOneInfo.Speed - m_NetworkPlayerTwoInfo.Speed;
        _AverageCycleForBoth = ComputeAverageFrequency(m_NetworkPlayerOneInfo.CycleDuration, m_NetworkPlayerTwoInfo.CycleDuration);
    }

    private float ComputeAverageFrequency(float Value1, float Value2)
    {
        return (Value1 + Value2) / 2;
    }

    private float SeparationDistance2D(Vector3 FirstPosition, Vector3 SecondPosition)
    {
        FirstPosition = new Vector3(FirstPosition.x, 0, FirstPosition.z);
        SecondPosition = new Vector3(SecondPosition.x, 0, SecondPosition.z);
        return Vector3.Distance(FirstPosition, SecondPosition);
    }
    public float GetSeparationDistance()
    {
        return _SeparationDistance;
    }
    public float GetAverageCycleForBoth()
    {
        return _AverageCycleForBoth;
    }
    public float GetDeltaSpeed()
    {
        return _DeltaSpeed;
    }
}