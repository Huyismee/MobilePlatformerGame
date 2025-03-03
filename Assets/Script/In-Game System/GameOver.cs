using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI; // Add this namespace for UI elements

public class GameOver : MonoBehaviour
{
    private Animator animator;
    private int deathCount; // Track the number of times the player has died
    private int currentLevel; // Track the current level from PlayerPrefs

    [Header("UI References")]
    public TextMeshProUGUI deathCountText; // Assign the TMP text element for death count
    public TextMeshProUGUI currentLevelText; // Assign the TMP text element for current level
    public Button retryButton; // Assign the button to show after a delay

    [Header("Button Delay Settings")]
    public float buttonDelay = 3f; // Delay in seconds before showing the button

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        // Play the death animation
        animator.SetTrigger("Die1");

        // Load the death count from PlayerPrefs
        deathCount = PlayerPrefs.GetInt("DeathCount", 0);

        // Increment the death count
        deathCount++;
        PlayerPrefs.SetInt("DeathCount", deathCount); // Save the updated death count
        PlayerPrefs.Save(); // Ensure the data is saved immediately

        // Load the current level from PlayerPrefs
        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1); // Default to level 1 if not found

        // Update the TMP text to display the death count and current level
        if (deathCountText != null)
        {
            deathCountText.text = $"-{deathCount} IQ";
        }
        else
        {
            Debug.LogWarning("Death count TMP text is not assigned!");
        }

        if (currentLevelText != null)
        {
            currentLevelText.text = $"Level {currentLevel}";
        }
        else
        {
            Debug.LogWarning("Current level TMP text is not assigned!");
        }

        // Hide the button initially
        if (retryButton != null)
        {
            retryButton.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Retry button is not assigned!");
        }

        // Start the coroutine to show the button after a delay
        StartCoroutine(ShowButtonAfterDelay());
    }

    // Coroutine to show the button after a delay
    private IEnumerator ShowButtonAfterDelay()
    {
        yield return new WaitForSeconds(buttonDelay); // Wait for the specified delay

        // Show the button
        if (retryButton != null)
        {
            retryButton.gameObject.SetActive(true);
        }
    }
}