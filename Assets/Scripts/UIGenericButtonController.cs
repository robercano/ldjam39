using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(AudioSource))]
public class UIGenericButtonController : EventTrigger
{

    private AudioSource audioSource;
    private EventTrigger eventTrigger;
    public AudioClip UIClickSound;
    public AudioClip UIOverSound;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayClickSound()
    {
        audioSource.PlayOneShot(UIClickSound);
    }

    public void PlayOverSound()
    {
        audioSource.PlayOneShot(UIOverSound);
    }

    public override void OnPointerEnter(PointerEventData data)
    {
        PlayOverSound();
    }

    public override void OnSelect(BaseEventData data)
    {
        PlayOverSound();
    }
    public override void OnSubmit(BaseEventData data)
    {
        PlayClickSound();
    }

}
