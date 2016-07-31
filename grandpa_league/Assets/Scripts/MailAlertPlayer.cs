using UnityEngine;
using System.Collections;

public class MailAlertPlayer : MonoBehaviour
{

    public AudioSource mailSource;
    public AudioClip mail;

	void Awake ()
    {
        mailSource = GetComponent<AudioSource>();
	}

    public void PlayNotification()
    {
        mailSource.clip = mail;
        mailSource.PlayOneShot(mail, 1.0F);
    }
	
}
