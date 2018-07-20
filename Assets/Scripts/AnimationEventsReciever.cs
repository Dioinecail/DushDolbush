using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventsReciever : MonoBehaviour
{
    public void PlayFadeAudio(AudioClip clip)
    {
        AudioSource source = GetComponent<AudioSource>();
        if(source)
        {
            source.PlayOneShot(clip);
        }
    }

    public void FinishedShowingCurtain()
    {
        GameManager.Instance.LoadLevel();
    }

    public void HideCurtainObject()
    {
        gameObject.SetActive(false);
    }
}