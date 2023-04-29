using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator)), RequireComponent(typeof(AudioSource))]
public class Menu : MonoBehaviour
{
    public string TitleState = "Master-P_menuloop";
    public AudioClip[] TitleSounds;

    private Animator Animator;
    private AudioSource AudioSource;
    private string PreviousState;

    void Start()
    {
        Animator = GetComponent<Animator>();
        AudioSource = GetComponent<AudioSource>();
        PreviousState = Animator.GetCurrentAnimatorStateInfo(0).ToString();
    }
    void Update()
    {
        var currentState = Animator.GetCurrentAnimatorStateInfo(0).ToString();

        // When we get past the title cards, play the "SUPER SPECIAL DELIVERY" sound effect.
        if (currentState != PreviousState && currentState == TitleState)
        {
            AudioSource.clip = TitleSounds[Random.Range(0, TitleSounds.Length)];
            AudioSource.Play();
        }

        PreviousState = currentState;
        
    }
}
