using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesController : MonoBehaviour
{
    [SerializeField] private GameObject MenuPanel;
    [SerializeField] private GameObject OptionsPanel;
    [SerializeField] private GameObject AudioAndVideoPanel;
    [SerializeField] private GameObject ControllsPanel;

    public void Play()
    {
        SceneManager.LoadScene(1);
    }

    public void Options()
    {
        OptionsPanel.SetActive(true);
        MenuPanel.SetActive(false);
    }

    public void AudioAndVideo()
    {
        AudioAndVideoPanel.SetActive(true);
        OptionsPanel.SetActive(false);
    }

    public void SetMasterVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    public void SetFullscreen(bool fullscreen)
    {
        Screen.fullScreen = fullscreen;
    }
    public void AudioAndVideoBackOption()
    {
        AudioAndVideoPanel.SetActive(false);
        OptionsPanel.SetActive(true);
    }

    public void controls()
    {
        ControllsPanel.SetActive(true);
        OptionsPanel.SetActive(false);
    }

    public void ControlsBackOption()
    {
        ControllsPanel.SetActive(false);
        OptionsPanel.SetActive(true);
    }


    public void backOptions()
    {

        MenuPanel.SetActive(true);
        OptionsPanel.SetActive(false);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
