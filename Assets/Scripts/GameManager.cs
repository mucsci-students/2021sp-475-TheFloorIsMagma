using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Level Settings")]
    public int levelTimer;
    public int timeAddValue = 10;
    public int scoreAddValue = 1000;

    [Header("Linked Objects")]
    public LavaScript lava;
    public PlayerControl player;

    [Header("UI")]
    public Text timeText;
    public Text timeAddText;
    public Text scoreText;
    public Text scoreAddText;
    public Text superJumpText;
    public Text lavaCoolText;
    public Text glideText;
    public Text checkpointText;
    public Text winText;
    public Image blackCover;

    private int timeRemaining;
    private int score = 0;

    private float timeTimer = 0;
    private float addTimeTimer = -1;
    private float addScoreTimer = -1;
    private float lavaCoolTimer = -1;
    private float glideTimer = -1;
    private float checkpointTimer = -1;
    private float fadeOutTimer = -1;
    private float fadeOutSpeed = 1;
    private float fadeInTimer = -1;
    private float fadeInSpeed = 1;
    private float winTimer = -1;

    AudioSource[] audio;
    GameObject[] pauseObjects;
    GameObject[] loseObjects;

    void Start()
    {
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        timeRemaining = levelTimer;
        timeText.text = "Time: " + levelTimer;
        timeAddText.text = "        +" + timeAddValue;
        timeAddText.enabled = false;
        scoreAddText.text = "+" + scoreAddValue;
        scoreAddText.enabled = false;
        checkpointText.enabled = false;
        winText.enabled = false;

        audio = GetComponents<AudioSource>();
        pauseObjects = GameObject.FindGameObjectsWithTag("PauseObject");
        loseObjects = GameObject.FindGameObjectsWithTag("LoseObject");

        HidePaused();
        HideLoseMenu();
    }

    void Update()
    {
        timeTimer += Time.deltaTime;
        if (timeTimer > 1)
        {
            timeTimer -= 1;
            timeRemaining -= 1;
            if (timeRemaining == 0)
            {
                audio[5].Stop();
                audio[6].Play();
                Time.timeScale = 0;
                ShowLoseMenu();
            }
        }

        if (winTimer >= 0)
        {
            winTimer += Time.deltaTime;
            if (winTimer > 6 && timeRemaining == 0)
            {
                if (SceneManager.GetActiveScene().name == "NewTutorial")
                {
                    if (!PlayerPrefs.HasKey("TutorialScore") || PlayerPrefs.GetInt("TutorialScore") < score)
                    {
                        PlayerPrefs.SetInt("TutorialScore", score);
                    }
                    PlayerPrefs.SetInt("LastHighScore", PlayerPrefs.GetInt("TutorialScore"));
                }
                else if (SceneManager.GetActiveScene().name == "Lvl1")
                {
                    if (!PlayerPrefs.HasKey("Lvl1Score") || PlayerPrefs.GetInt("Lvl1Score") < score)
                    {
                        PlayerPrefs.SetInt("Lvl1Score", score);
                    }
                    PlayerPrefs.SetInt("LastHighScore", PlayerPrefs.GetInt("Lvl1Score"));
                }
                else if (SceneManager.GetActiveScene().name == "Lvl2")
                {
                    if (!PlayerPrefs.HasKey("Lvl2Score") || PlayerPrefs.GetInt("Lvl2Score") < score)
                    {
                        PlayerPrefs.SetInt("Lvl2Score", score);
                    }
                    PlayerPrefs.SetInt("LastHighScore", PlayerPrefs.GetInt("Lvl2Score"));
                }
                PlayerPrefs.SetInt("LastScore", score);
                SceneManager.LoadScene("Credits");
            }
            if (timeRemaining > 0)
            {
                --timeRemaining;
                score += 10;
                audio[4].Play();
            }
        }

        timeText.text = "Time: " + timeRemaining;
        scoreText.text = "Score: " + score;

        // Flashing text when time/score is added
        if (addTimeTimer >= 0)
        {
            addTimeTimer += Time.deltaTime;
            if (addTimeTimer > 1)
            {
                addTimeTimer = -1;
                timeAddText.enabled = false;
            }
            else
            {
                timeAddText.enabled = addTimeTimer % 0.2f < 0.1f;
            }
        }
        if (addScoreTimer >= 0)
        {
            addScoreTimer += Time.deltaTime;
            if (addScoreTimer > 1)
            {
                addScoreTimer = -1;
                scoreAddText.enabled = false;
            }
            else
            {
                scoreAddText.enabled = addScoreTimer % 0.2f < 0.1f;
            }
        }
        if (checkpointTimer >= 0)
        {
            checkpointTimer += Time.deltaTime;
            if (checkpointTimer > 1)
            {
                checkpointTimer = -1;
                checkpointText.enabled = false;
            }
            else
            {
                checkpointText.enabled = checkpointTimer % 0.2f < 0.1f;
            }
        }

        if (lavaCoolTimer >= 0)
        {
            lavaCoolTimer += Time.deltaTime;
            if (lavaCoolTimer >= 10)
            {
                EndLavaCool();
            }
            else
            {
                lavaCoolText.text = "Magma Cooled! Time: " + (10 - (int)lavaCoolTimer);   
            }
        }

        if (glideTimer >= 0)
        {
            glideTimer += Time.deltaTime;
            if (glideTimer >= 10)
            {
                EndGlide();
            }
            else
            {
                glideText.text = "Hold Jump to Glide! Time: " + (10 - (int)glideTimer);   
            }
        }

        if (fadeOutTimer >= 0)
        {
            fadeOutTimer += Time.deltaTime * fadeOutSpeed;
            if (fadeOutTimer >= 1)
            {
                fadeOutTimer = -1;
                blackCover.color = Color.black;
            }
            else
            {
                blackCover.color = new Color(0, 0, 0, fadeOutTimer);
            }
        }

        if (fadeInTimer >= 0)
        {
            fadeInTimer += Time.deltaTime * fadeInSpeed;
            if (fadeInTimer >= 1)
            {
                fadeInTimer = -1;
                blackCover.color = new Color(0, 0, 0, 0);
            }
            else
            {
                blackCover.color = new Color(0, 0, 0, 1 - fadeInTimer);
            }
        }

        if(Input.GetKeyDown(KeyCode.Escape)) {
            TogglePause();
        }
    }

    public void IncreaseTimer()
    {
        timeRemaining += timeAddValue;
        addTimeTimer = 0;
        audio[1].Play();
    }

    public void IncreaseScore()
    {
        score += scoreAddValue;
        addScoreTimer = 0;
        audio[0].Play();
    }

    public void StartSJDisplay()
    {
        // Move text to first open position
        if (lavaCoolText.rectTransform.anchoredPosition.y != 0 && glideText.rectTransform.anchoredPosition.y != 0)
        {
            superJumpText.rectTransform.anchoredPosition = new Vector3(0, 0, 0);
        }
        else if (lavaCoolText.rectTransform.anchoredPosition.y != -30 && glideText.rectTransform.anchoredPosition.y != -30)
        {
            superJumpText.rectTransform.anchoredPosition = new Vector3(0, -30, 0);
        }
        else
        {
            superJumpText.rectTransform.anchoredPosition = new Vector3(0, -60, 0);
        }
        audio[3].Play();
    }

    public void StopSJDisplay()
    {
        superJumpText.rectTransform.anchoredPosition = new Vector3(0, 30, 0);
    }

    public void StartLavaCool()
    {
        lava.Freeze();
        lavaCoolTimer = 0;

        // Move text to first open position
        if (superJumpText.rectTransform.anchoredPosition.y != 0 && glideText.rectTransform.anchoredPosition.y != 0)
        {
            lavaCoolText.rectTransform.anchoredPosition = new Vector3(0, 0, 0);
        }
        else if (superJumpText.rectTransform.anchoredPosition.y != -30 && glideText.rectTransform.anchoredPosition.y != -30)
        {
            lavaCoolText.rectTransform.anchoredPosition = new Vector3(0, -30, 0);
        }
        else
        {
            lavaCoolText.rectTransform.anchoredPosition = new Vector3(0, -60, 0);
        }
        audio[3].Play();
    }

    public void EndLavaCool()
    {
        lava.Melt();
        lavaCoolTimer = -1;
        
        lavaCoolText.rectTransform.anchoredPosition = new Vector3(0, 30, 0);
    }

    public void StartGlide()
    {
        player.SetCanGlide(true);
        glideTimer = 0;

        // Move text to first open position
        if (superJumpText.rectTransform.anchoredPosition.y != 0 && lavaCoolText.rectTransform.anchoredPosition.y != 0)
        {
            glideText.rectTransform.anchoredPosition = new Vector3(0, 0, 0);
        }
        else if (superJumpText.rectTransform.anchoredPosition.y != -30 && lavaCoolText.rectTransform.anchoredPosition.y != -30)
        {
            glideText.rectTransform.anchoredPosition = new Vector3(0, -30, 0);
        }
        else
        {
            glideText.rectTransform.anchoredPosition = new Vector3(0, -60, 0);
        }
        audio[3].Play();
    }

    public void EndGlide()
    {
        player.SetCanGlide(false);
        glideTimer = -1;

        glideText.rectTransform.anchoredPosition = new Vector3(0, 30, 0);
    }

    public void DisplayCheckpoint()
    {
        checkpointTimer = 0;
        audio[2].Play();
    }

    public void Win()
    {
        FadeOut(0.25f);
        checkpointText.enabled = false;
        checkpointTimer = -1;
        winText.enabled = true;
        timeTimer = -100;
        winTimer = 0;
        audio[5].Stop();
        audio[7].Play();
    }

    public void FadeOut(float speed)
    {
        fadeOutTimer = 0;
        fadeOutSpeed = speed;
    }

    public void FadeIn(float speed)
    {
        fadeInTimer = 0;
        fadeInSpeed = speed;
    }

    public void TogglePause() {
        if(Time.timeScale == 1) {
            Time.timeScale = 0;
            ShowPaused();
        }
        else if(Time.timeScale == 0) {
            Time.timeScale = 1;
            HidePaused();
        }
    }

    public void Reload() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ShowPaused() {
        foreach(GameObject g in pauseObjects) {
            g.SetActive(true);
        }
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void HidePaused() {
        foreach(GameObject g in pauseObjects) {
            g.SetActive(false);
        }
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ShowLoseMenu() {
        foreach(GameObject g in loseObjects) {
            g.SetActive(true);
        }
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void HideLoseMenu() {
        foreach(GameObject g in loseObjects) {
            g.SetActive(false);
        }
    }

    public void LoadLevel(string sceneName) {
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneName);
    }
}
