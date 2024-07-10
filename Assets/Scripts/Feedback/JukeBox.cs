using System.Collections.Generic;
using UnityEngine;

public class JukeBox : MonoBehaviour
{

    [SerializeField] List<AudioClip> clips;
    [SerializeField] AudioSource audioSource;
    private int clipcounter = 0;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayFootstepSound()
    {
        AudioClip clip = clips[clipcounter];
        audioSource.clip = clip;
        audioSource.Play();
        clipcounter++;
        if (clipcounter >= clips.Count)
        {
            clipcounter = 0;
        }
    }
}
