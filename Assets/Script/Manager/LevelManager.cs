using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : SerializedMonoBehaviour
{
    // Singleton instance
    public static LevelManager Instance { get; private set; }

    [Header("Level Settings")]
    [SerializeField]
    public Dictionary<int, GameObject> levels = new Dictionary<int, GameObject>(); // Dictionary to hold level prefabs
    public int currentLevel = 1; // Current level the player is on
    public int maxLevelReached = 1; // Highest level the player has unlocked

    private const string CURRENT_LEVEL_KEY = "CurrentLevel";
    private const string MAX_LEVEL_REACHED_KEY = "MaxLevelReached";

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scene changes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }

        // Load player progression from PlayerPrefs
        LoadPlayerProgression();
        SavePlayerProgression();
    }

    private void OnLevelWasLoaded(int level)
    {
        if (!SceneManager.GetActiveScene().name.Equals("Gameplay")) return;
        StartCurrentLevel();
    }
    // Load player progression from PlayerPrefs
    private void LoadPlayerProgression()
    {
        currentLevel = PlayerPrefs.GetInt(CURRENT_LEVEL_KEY, 1); // Default to level 1
        maxLevelReached = PlayerPrefs.GetInt(MAX_LEVEL_REACHED_KEY, 1); // Default to level 1
        Debug.Log("Current Level " + currentLevel + " - Max Level "+ maxLevelReached);
    }

    // Save player progression to PlayerPrefs
    private void SavePlayerProgression()
    {
        PlayerPrefs.SetInt(CURRENT_LEVEL_KEY, currentLevel);
        PlayerPrefs.SetInt(MAX_LEVEL_REACHED_KEY, maxLevelReached);
        PlayerPrefs.Save();
    }

    // Start the current level (used when the player dies and presses "Play Again")
    public void StartCurrentLevel()
    {
        if (levels.ContainsKey(currentLevel))
        {
            StartLevel(currentLevel); // Instantiate the current level prefab
            Debug.Log($"Starting Level {currentLevel}");
        }
        else
        {
            Debug.LogWarning($"Level {currentLevel} not found!");
            GameManager.Instance.LoadLevelSelect();
        }
    }

    // Start a specific level (used for level selection)
    public void StartLevel(int levelNumber)
    {
        if (levels.ContainsKey(levelNumber))
        {
            if (levelNumber <= maxLevelReached)
            {
                currentLevel = levelNumber; // Update the current level
                SavePlayerProgression(); // Save the updated progression
                Instantiate(levels[levelNumber]); // Instantiate the level prefab
                Debug.Log($"Starting Level {levelNumber}");
            }
            else
            {
                Debug.LogWarning($"Level {levelNumber} is locked!");
            }
        }
        else
        {
            Debug.LogWarning($"Level {levelNumber} not found!");
            GameManager.Instance.LoadLevelSelect();
        }
    }

    // Complete the current level and unlock the next level
    public void CompleteCurrentLevel()
    {
        if (currentLevel == maxLevelReached)
        {
            maxLevelReached++; // Unlock the next level
            Debug.Log($"Level {currentLevel} completed! Level {maxLevelReached} unlocked.");
        }
        else
        {
            Debug.Log($"Level {currentLevel} completed!");
        }

        currentLevel++; // Move to the next level
        SavePlayerProgression(); // Save the updated progression
        if (!levels.ContainsKey(currentLevel))
        {
            GameManager.Instance.LoadLevelSelect();
        }
        else
        {
            GameManager.Instance.ReLoadGameplay();
        }
    }

    // Reset player progression
    public void ResetProgression()
    {
        currentLevel = 1;
        maxLevelReached = 1;
        SavePlayerProgression();
        Debug.Log("Player progression reset.");
    }

    // Add a level to the dictionary
    public void AddLevel(int levelNumber, GameObject levelPrefab)
    {
        if (!levels.ContainsKey(levelNumber))
        {
            levels.Add(levelNumber, levelPrefab);
            Debug.Log($"Level {levelNumber} added.");
        }
        else
        {
            Debug.LogWarning($"Level {levelNumber} already exists!");
        }
    }
}