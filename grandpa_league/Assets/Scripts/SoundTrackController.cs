using UnityEngine;
using System.Collections;

public class SoundTrackController : MonoBehaviour
{

    public AudioSource musicSource;
    public AudioClip[] music;

    private float lengthOfClip;
    private float numOfRepeats = 8;
    private float timeOfRepeats;
    private float fadeOutTime = 15;
    private int trackNumber = 0;
    private float fadeInTime = 10;

	void Awake ()
    {
        musicSource.clip = music[trackNumber];
        musicSource.Play();

        StartCoroutine(CountSongLoops());
	}

    IEnumerator CountSongLoops()
    {
        while (true)
        {
            //finds out total length of time music will play before switchinging songs
            lengthOfClip = music[trackNumber].length;
            timeOfRepeats = lengthOfClip * numOfRepeats;

            //waits designated amouint of timebefore fading out
            yield return new WaitForSeconds(timeOfRepeats);
            StartCoroutine(FadeOutMusic());

            //switches track #
            if (trackNumber != (music.Length - 1))
                trackNumber++;
            else
                trackNumber = 0;

            //after fade out is done, loads new clip, plays and fades in
            yield return new WaitForSeconds(fadeOutTime);

            musicSource.Stop();
            musicSource.clip = music[trackNumber];
            musicSource.Play();

            StartCoroutine(FadeInMusic());
        }
    }

    IEnumerator FadeInMusic()
    {
        float t = fadeInTime;

        while (t>0)
        {
            yield return null;
            t -= Time.deltaTime;
            musicSource.volume = (1 - t / fadeInTime);
        }
        yield break;
    }


    IEnumerator FadeOutMusic()
    {
        float t = fadeOutTime;
        while (t>0)
        {
            yield return null;
            t -= Time.deltaTime;
            musicSource.volume = t / fadeOutTime;
        }
        yield break;
    }
	
}
