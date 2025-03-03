using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum InteractTarget
{
    Player, Arrow, Enemy
}

public class TriggerBox : MonoBehaviour
{
    // List of targets to check against
    public List<InteractTarget> validTargets = new List<InteractTarget>();
    public UnityEvent ToggleOn, ToggleOff;
    public float DelayActivateTime, DelayLeftTime;
    private bool pressed = false;

    // Function to check if the tag matches any in the validTargets list
    private bool IsValidTarget(Collider other)
    {
        // If no specific target is provided, we assume all tags are valid
        if (validTargets.Count == 0) return true;

        // Check if the tag matches any of the valid targets
        foreach (InteractTarget target in validTargets)
        {
            if (other.CompareTag(target.ToString())) return true;
        }

        return false; // No matching tag found
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("entered");
        if (!IsValidTarget(other) || pressed) return;
        StartCoroutine(TriggerToggle(true));
        pressed = true;
    }

    private IEnumerator TriggerToggle(bool toggle)
    {
        if (toggle)
        {
            yield return new WaitForSeconds(DelayActivateTime); // Wait for the delay
            ToggleOn.Invoke(); // Invoke the ToggleOn event
        }
        else if (!toggle)
        {
            yield return new WaitForSeconds(DelayLeftTime); // Wait for the delay
            ToggleOff.Invoke(); // Invoke the ToggleOff event
        }
    }
}