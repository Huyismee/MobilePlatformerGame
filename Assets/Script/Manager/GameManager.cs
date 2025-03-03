using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Scene Names")]
    public string mainMenuScene = "MainMenu";
    public string levelSelectScene = "LevelSelect";
    public string gameplayScene = "Gameplay";
    public string dieMenuScene = "DieScene";
    public TextMeshProUGUI Message;

    [Header("Transition Settings")]
    public float transitionDelay = 1f;
    public CanvasGroup fadeCanvasGroup; // Assign the CanvasGroup for fade effects

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    private void Start()
    {
        if (fadeCanvasGroup != null)
        {
            StartCoroutine(FadeCanvasGroup(fadeCanvasGroup, 1f, 0f, transitionDelay / 2));
        }
    }

    public void LoadMainMenu()
    {
        StartCoroutine(TransitionToScene(mainMenuScene));
    }

    public void LoadLevelSelect()
    {
        StartCoroutine(TransitionToScene(levelSelectScene));
    }

    public void LoadGameplay(int level)
    {
        if (level > PlayerPrefs.GetInt("MaxLevelReached")) 
        {
            if(!Message.enabled)
                StartCoroutine(ShowText("Current level not reached!"));
            return;
        }
        PlayerPrefs.SetInt("CurrentLevel", level);
        StartCoroutine(TransitionToScene(gameplayScene));
    }

    public void ReLoadGameplay()
    {
        if(PlayerPrefs.HasKey("CurrentLevel"))
        LoadGameplay(PlayerPrefs.GetInt("CurrentLevel"));
    }

    public void LoadDieMenu()
    {
        StartCoroutine(TransitionToScene(dieMenuScene));
    }

    public void RestartCurrentScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        StartCoroutine(TransitionToScene(currentScene));
    }

    private IEnumerator TransitionToScene(string sceneName)
    {
        // Fade out
        if (fadeCanvasGroup != null)
        {
            yield return StartCoroutine(FadeCanvasGroup(fadeCanvasGroup, 0f, 1f, transitionDelay / 2));
        }

        // Wait for the transition delay
        yield return new WaitForSeconds(transitionDelay);

        // Load the new scene
        SceneManager.LoadScene(sceneName);

        // Fade in
        if (fadeCanvasGroup != null)
        {
            yield return StartCoroutine(FadeCanvasGroup(fadeCanvasGroup, 1f, 0f, transitionDelay / 2));
        }
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = endAlpha;
    }

    private IEnumerator ShowText(string message)
    {
        float duration = 1.5f;
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            Message.enabled = true;
            Message.text = message;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Message.enabled = false;
      
    }
}