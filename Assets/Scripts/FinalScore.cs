using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FinalScore : MonoBehaviour
{
    public Text scoretext;
    public Text highScoreText;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        scoretext.text = "Final Score:  " + PlayerPrefs.GetInt("LastScore").ToString();
        highScoreText.text = "High Score:  " + PlayerPrefs.GetInt("LastHighScore").ToString();
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
