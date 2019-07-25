using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtonController : MonoBehaviour {

    public GameObject helpMenu;

    private void Awake()
    {
        helpMenu.SetActive(false);
    }
    public void PlayButtonPressed()
    {
        SceneManager.LoadScene(1);
    }
    public void QuitButtonPressed()
    {
        Application.Quit();
    }
    public void HelpButtonPressed()
    {
        helpMenu.SetActive(true);
    }
    public void HelpQuitButtonPressed()
    {
        helpMenu.SetActive(false);
    }
}
