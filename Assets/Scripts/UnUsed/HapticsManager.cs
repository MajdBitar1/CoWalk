using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Haptics;
public class HapticsManager : MonoBehaviour
{
    [SerializeField] HapticClip SeparationHapticClip;
    private HapticClipPlayer m_HapticClipPlayer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void VirtualTouchHaptic()
    {
       // m_HapticClipPlayer = new HapticClipPlayer();
        //m_HapticClipPlayer.Play(SeparationHapticClip);
    }
}
