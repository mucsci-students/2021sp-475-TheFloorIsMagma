using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuUnlocks : MonoBehaviour
{
    public Text tutorialLabel;
    public Text lvl1Label;
    public Text lvl2Label;
    public Button lvl1Button;
    public Button lvl2Button;

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        tutorialLabel.text = "High Score: " + PlayerPrefs.GetInt("TutorialScore");
        if (!PlayerPrefs.HasKey("TutorialScore") || PlayerPrefs.GetInt("TutorialScore") == 0)
        {
            lvl1Label.text = "Complete the Tutorial";
            lvl1Button.interactable = false;
        }
        else
        {
            lvl1Label.text = "High Score: " + PlayerPrefs.GetInt("Lvl1Score");
            lvl1Button.interactable = true;
        }
        if (!PlayerPrefs.HasKey("Lvl1Score") || PlayerPrefs.GetInt("Lvl1Score") < 20000)
        {
            lvl2Label.text = "Score 20000 on Level 1";
            lvl2Button.interactable = false;
        }
        else
        {
            lvl2Label.text = "High Score: " + PlayerPrefs.GetInt("Lvl2Score");
            lvl2Button.interactable = true;
        }
    }
}
