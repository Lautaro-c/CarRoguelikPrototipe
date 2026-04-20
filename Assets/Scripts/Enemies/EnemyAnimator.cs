using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayDeathAnamiation()
    {
        animator.SetBool("IsDead", true);
    }

    public void PlayWalkingAnamiation()
    {
        animator.SetBool("IsWalking", true);
        animator.SetBool("IsShooting", false);
        animator.SetBool("IsRunning", false);
    }

    public void PlayRunningAnamiation()
    {
        animator.SetBool("IsWalking", false);
        animator.SetBool("IsShooting", false);
        animator.SetBool("IsRunning", true);
    }

    public void PlayAttackAnamiation()
    {
        animator.SetBool("IsWalking", false);
        animator.SetBool("IsShooting", true);
        animator.SetBool("IsRunning", false);
    }
}
