using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    private Animator animator;

    private const string JUMPING = "Jumping";
    private const string ISRUNNING = "isRunning";
    private const string FLAIR = "Flair";
    private const string MINING = "Mining";

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void Running()
    {
        animator.SetBool(ISRUNNING, true);
    }

    public void Idling()
    {
        animator.SetBool(ISRUNNING, false);
    }

    public void Jumping()
    {
        animator.Play(JUMPING);
    }

    public void Flair()
    {
        animator.Play(FLAIR);
    }

    public void Mining()
    {
        //animator.Play(MINING);
        animator.SetBool(MINING, true);
    }

    public void Fallen()
    {
        Debug.Log("fallen");
    }

    public void NotMining()
    {
        //animator.Play(MINING);
        animator.SetBool(MINING, false);
    }
}
