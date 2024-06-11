using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Oculus.Interaction.Locomotion;
using UnityEngine.InputSystem.XR.Haptics;
using Oculus.Haptics;
using UnityEngine.Video;

public class JukeBox : NetworkBehaviour
{
    private float mytime = 0f;
    [SerializeField] float StepCycle = 2f;
    [SerializeField] List<AudioClip> clips;
    [SerializeField] AudioSource audioSource;
    private bool vibration = false;
    private int clipcounter = 0;

    // Start is called before the first frame update
    void Start()
    {
    }

    public override void FixedUpdateNetwork()
    {
        // mytime += Runner.DeltaTime;
        // // if (mytime > (2 * Runner.DeltaTime) && vibration == true) vibrateOFF();
        // if (mytime > StepCycle)
        // {
        //     //vibrateON();
        //     clipcounter++;
        //     AudioClip clip = clips[ clipcounter%2 ];
        //     audioSource.clip = clip;
        //     audioSource.Play();
        //     mytime = 0;
        //     //Debug.Log("Beat");
        // }
    }
    public void setcycle(float newcycle)
    {
        if(newcycle>0.5f && newcycle<4f)
        StepCycle = newcycle;
    }

}
