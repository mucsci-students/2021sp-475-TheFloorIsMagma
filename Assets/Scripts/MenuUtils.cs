using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUtils : MonoBehaviour
{
    private GameObject cam;
    private float moveCamTimer = -1;
    private bool movingUp = false;

    public string sceneToLoad;

    void Start()
    {
        cam = GameObject.Find("Main Camera");
    }

    void Update()
    {
        if (moveCamTimer >= 0)
        {
            Vector3 movement = movingUp ? Vector3.up : Vector3.down;
            moveCamTimer += Time.deltaTime * 2;
            if (moveCamTimer < 1)
            {
                cam.transform.position += movement * moveCamTimer * 800 * Time.deltaTime;
            }
            else if (moveCamTimer <= 2)
            {
                cam.transform.position += movement * (2.0f - moveCamTimer) * 800 * Time.deltaTime;
            }
            else
            {
                moveCamTimer = -1;
            }
        }
    }

    public void LoadScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
    } 

    public void LoadScene() {
        SceneManager.LoadScene(sceneToLoad);
    }

    public void MoveCameraUp()
    {
        if (moveCamTimer == -1)
        {
            moveCamTimer = 0;
            movingUp = true;
        }
    }

    public void MoveCameraDown()
    {
        if (moveCamTimer == -1)
        {
            moveCamTimer = 0;
            movingUp = false;
        }
    }

    public void QuitGame() {
        Application.Quit();
    }
}
