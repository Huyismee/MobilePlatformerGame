using UnityEngine;

public class PlayerAnimManager : MonoBehaviour
{
    public Animator animator;

    // Animation parameters
    private const string IS_RUNNING = "IsRunning";
    private const string JUMP = "Jump";
    private const string DIE_1 = "Die1";
    private const string DIE_2 = "Die2";

    private void Start()
    {
        if (animator == null)
        {
            Debug.LogError("Animator component not found on the player!");
        }
    }

    // Update is called once per frame

    // Set the running state
    public void SetRunning(bool isRunning)
    {
        animator.SetBool(IS_RUNNING, isRunning);
    }

    // Trigger jump animation
    public void TriggerJump()
    {
        animator.SetTrigger(JUMP);
    }

    // Trigger Die1 animation
    public void TriggerDie1()
    {
        animator.SetTrigger(DIE_1);
    }

    // Trigger Die2 animation
    public void TriggerDie2()
    {
        animator.SetTrigger(DIE_2);
    }

    // Reset all triggers (useful for transitioning between states)
    public void ResetTriggers()
    {
        animator.ResetTrigger(JUMP);
        animator.ResetTrigger(DIE_1);
        animator.ResetTrigger(DIE_2);
    }
}