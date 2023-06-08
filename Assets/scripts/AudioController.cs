using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public static AudioController Instance;
    //“˝»Î“Ù∆µ
    public AudioClip boom, fire, die, fade;
    private AudioSource audioSource;

    private void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayDie()
    {
        audioSource.clip = die;
        audioSource.Play();
    }
    public void PlayBoom()
    {
        audioSource.clip = boom;
        audioSource.Play();
    }

    public void PlayFire()
    {
        audioSource.clip = fire;
        audioSource.Play();
    }

    public void PlayFade()
    {
        audioSource.clip = fade;
        audioSource.Play();
    }
}

