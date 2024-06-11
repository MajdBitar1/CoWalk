using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioClip clip;
    [SerializeField] AudioSource audioSource;

    // Update is called once per frame
    void Start()
    {
        audioSource.clip = clip;
        audioSource.Play();
    }
}
