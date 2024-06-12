using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class JukeBox : MonoBehaviour
{
    private float mytime = 0f;
    [SerializeField] float StepCycle = 2f;
    [SerializeField] List<AudioClip> clips;
    [SerializeField] AudioSource audioSource;
    private int clipcounter = 0;

    public void PlayFootstepSound()
    {
        clipcounter++;
        AudioClip clip = clips[ clipcounter%2 ];
        audioSource.clip = clip;
        audioSource.Play();
    }

    public void setcycle(float cycle)
    {
        StepCycle = cycle;
    }

}
