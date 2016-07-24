using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class SoundEffectPlayer : MonoBehaviour {

    public AudioClip[] clicks;
    public AudioSource clickSource;


    //choses a random click noise to play then plays it
    public void PlayClick()
    {
        int randClip = Random.Range(0, clicks.Length);
        clickSource.clip = clicks[randClip];
        clickSource.Play();
    }
}
