using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    [SerializeField] private float deathTime = 1f;
    private Animator animator;


    private void Start()
    {
        animator = GetComponent<Animator>();
        CancelInvoke("Destroy");
    }

    public void PlayDeathAnamiation()
    {
        animator.SetBool("IsDead", true);
        Invoke("Destroy", deathTime);
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

    private void Destroy()
    {
        Destroy(this.gameObject);
    }
}
