using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public static  AudioSource audioSource;
    public AudioClip die;
    public AudioClip jump;
    public AudioClip split;
    public AudioClip absorb;
    public AudioClip victory;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update

    public void PlayDie()
    {
        audioSource.PlayOneShot(die);
    }
    public void PlayJump()
    {
        audioSource.PlayOneShot(jump);
    }
    public void PlaySplit()
    {
        audioSource.PlayOneShot(split);
    }
    public void PlayAbsorb()
    {
        audioSource.PlayOneShot(absorb);
    }
    public void PlayVictory()
    {
        audioSource.PlayOneShot(victory);
    }

}
