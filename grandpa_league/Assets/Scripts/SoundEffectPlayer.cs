using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class SoundEffectPlayer : MonoBehaviour {

    public AudioClip[] clicks;
    public AudioSource clickSource;
    public AudioClip mail;
    public AudioSource mailSource;

    public void Awake()
    {
        mailSource.Play();
    }

    public void PlayNotification()
    {
        mail.LoadAudioData();
        mailSource.clip = mail;
        mailSource.enabled = true;
        mailSource.ignoreListenerPause = true;

        if (mailSource.clip.loadState == AudioDataLoadState.Loaded)
        {
            if (!mailSource.isActiveAndEnabled)
                Debug.Log("not active or enabled sound fx");
            else
            {
                mailSource.Play();
                Debug.Log(string.Format("Just played audio clip {0} hz, {1} length, {2}", mailSource.clip.frequency, mailSource.clip.length, mailSource.clip.name));
            }
        }
        else
            Debug.LogError(string.Format("AUDIO PLAYING FAILED {0}", mailSource.clip.loadState.ToString()));
    }

    //choses a random click noise to play then plays it
    public void PlayClick()
    {
        int randClip = Random.Range(0, clicks.Length);
        clickSource.clip = clicks[randClip];
        clickSource.Play();
        mailSource.clip = mail;
        mailSource.Play();
    }
}
