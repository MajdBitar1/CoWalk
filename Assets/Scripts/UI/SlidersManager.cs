using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlidersManager : MonoBehaviour
{
    [SerializeField] GroupManager groupman;
    [SerializeField] Slider AdditionalDistanceSlider,CSTDistanceSlider,MAXDistanceSlider,SAFEDistanceSlider;
    [SerializeField] TextMeshProUGUI AdditionalDistanceText,CSTDistanceText,MAXDistanceText,SAFEDistanceText;

    // Update is called once per frame
    void Start()
    {
        UpdateText();
    }
    private void UpdateText()
    {
        AdditionalDistanceText.text = AdditionalDistanceSlider.value.ToString();
        CSTDistanceText.text = CSTDistanceSlider.value.ToString();
        MAXDistanceText.text = MAXDistanceSlider.value.ToString();
        SAFEDistanceText.text = SAFEDistanceSlider.value.ToString();
    }
    private void PassValues()
    {
        groupman.UpdateDistanceSliders(SAFEDistanceSlider.value, MAXDistanceSlider.value, CSTDistanceSlider.value, AdditionalDistanceSlider.value);
    }

    public void SliderUpdate()
    {
        UpdateText();
        PassValues();
    }
}
