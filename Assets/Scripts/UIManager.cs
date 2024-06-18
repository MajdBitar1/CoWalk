using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] GroupManager _groupman;
    //[SerializeField] Armswing _armswing;



    [Header("UI Elements")]
    //[SerializeField] TextMeshProUGUI ASstateValue;
    [SerializeField] TextMeshProUGUI PlayerOneSpeed;
    [SerializeField] TextMeshProUGUI PlayerOneCycle;
    [SerializeField] TextMeshProUGUI SeparationDistance;
    [SerializeField] TextMeshProUGUI PlayerTwoSpeed;    
    [SerializeField] TextMeshProUGUI PlayerTwoCycle;
    [SerializeField] Slider _SpeedAmp_Slider;
    [SerializeField] Slider _Brightness_Slider;
    private UIData _data;
    private bool StateAura = false;
    private bool StateRhythm = false;


    void LateUpdate()
    {
        _data = _groupman.PassUIData();
       // _data.ASstate = _armswing.isActiveAndEnabled ? 1 : 0;
        UpdateUIValues();
    }
    private void SetPlayerValues()
    {
        //ASstateValue.text = _data.ASstate.ToString("F2");
        PlayerOneCycle.text = _data.PlayerOneCycle.ToString("F2");
        PlayerTwoCycle.text = _data.PlayerTwoCycle.ToString("F2");
        PlayerOneSpeed.text = _data.PlayerOneSpeed.ToString("F2");
        PlayerTwoSpeed.text = _data.PlayerTwoSpeed.ToString("F2");
        SeparationDistance.text = _data.SeparationDistance.ToString("F2");
    }
    private void UpdateUIValues()
    {
        SetPlayerValues();
    }
    public void ButtonUpdatedAura()
    {
        StateAura = !StateAura;
        UpdateButtonValues();
    }
    public void ButtonUpdatedRhythm()
    {
        StateRhythm = !StateRhythm;
        UpdateButtonValues();
    }
    public void UpdateSliderValue()
    {
        UpdateButtonValues();
    }

    private void UpdateButtonValues()
    {
        _groupman.ButtonUpdatedValues(StateAura, StateRhythm, _SpeedAmp_Slider.value, _Brightness_Slider.value);
        
    }
    // public void UpdateSliderValues()
    // {
    //     _groupman.SliderUpdatedValues();
    // }
}
