using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
[RequireComponent(typeof(VisualEffect))]
public class AuraManager : MonoBehaviour
{
    
    [Header("Constants To Tune")]
    [SerializeField] float SafeSeparationZone = 5;
    [SerializeField] float MaxSeparationZone = 10;
    [SerializeField] float StartingValue = 9;
    [SerializeField] float AdditionalValue = 1;
    private VisualEffect m_AuraEffect;
    private bool AuraBroken = false;
    private bool inView = false;

    private void Start()
    {
        m_AuraEffect = GetComponent<VisualEffect>();
    }
    private void OnApplicationQuit()
    {
        gameObject.transform.localScale = new Vector3(40,40,40);
        m_AuraEffect.SetFloat("Lifetime", 1.5f);
        m_AuraEffect.SetVector4("Color", Color.white );
    }
    
    private bool ObjectInCameraView(GameObject obj)
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(obj.transform.position);
        bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
        return onScreen;
    }
    public int Aura(bool AuraState,float distance)
    {
        if (!AuraState)
        {
            return 0;
        }
        //Normalize Distance relative to Min and Max Separation Zones, to Get a value which is Negative While in Safe zone, between safe and max the value will be between 0 and 1
        float value = Mathf.Min( 1, (distance - SafeSeparationZone) / MaxSeparationZone ) ;
        Debug.Log("[GroupMan] Value: " + value);
        //Return Bool that checks if other player is in view or not
        if (!GameManager.IsCameraMan)
        {
            inView = ObjectInCameraView(GameManager.RemotePlayerObject);
        }
        if (!AuraBroken && value > 0)
        {
            // Value > 0 means the separation distance is > SAFE ZONE
            //Check if you can see the other player
            if (inView) 
            {
                //if you can see other player, ripple effect will stop
                m_AuraEffect.Stop();
                return 1;
            }
            //You can't see other player, then we have to play the ripple effect
            else
            {
                // if Value exceeds 1 then the separatino distance > MAX distance and thus the aura breaks!
                if (value >= 1f)
                {
                    AuraBroken = true;
                    m_AuraEffect.Stop();
                    return -1;
                }   
                // First Scale up based on separation distance, this will ensure that the other player will see ripple effect even at high separation
                gameObject.transform.localScale = new Vector3( StartingValue + AdditionalValue + (value * 1.25f * MaxSeparationZone) , 0f, StartingValue + AdditionalValue + (value * 1.25f * MaxSeparationZone) ); 
                m_AuraEffect.SetFloat("Lifetime", 1.5f + value);
                //Second is to change the color of the ripples, this will be a gradient from Oranage to Red
                Color ColorOnGrad = Color.Lerp( new Color(1,0.74f,0,1) , Color.red , value); // Color(1,0.647f,0,1) is Orange (255,165,0)
                m_AuraEffect.SetVector4("Color", ColorOnGrad);

                //Finally Play the effect
                m_AuraEffect.Play();
                return 2;
            }
        }
        //Value < 0 Means the separation is in SAFE ZONE, this means the ripples will be smaller and will have a white/transperant color
        //Moreover if aura was broken and players enter safe zone then the AURA will be re-activated again
        else
        {
            if (value <= 0)
            {
                AuraBroken = false;
                if (inView) 
                {
                    //if you can see other player, ripple effect will stop
                    m_AuraEffect.Stop();
                    return 1;
                }
                m_AuraEffect.SetFloat("Lifetime", 1.5f);
                gameObject.transform.localScale = new Vector3( StartingValue + 2 + value * MaxSeparationZone , 0f, StartingValue + 2 + value * MaxSeparationZone);
                m_AuraEffect.SetVector4("Color", Color.white );
                m_AuraEffect.Play();
                return 2;
            }
            return -1;
        }
    }
    public void UpdateZoneValues(float Safe, float Max, float Cst, float Add)
    {
        SafeSeparationZone = Safe;
        MaxSeparationZone = Max;
        StartingValue = Cst;
        AdditionalValue = Add;
    }

}
