using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public Button startButton;
    public Button settingsButton;
    public Button closeSettingsButton;
    public Slider volumeSlider;
    public GameObject settingsPanel;

    private void Awake()
    {
        startButton.onClick.AddListener(() => { StartGame(); });
        settingsButton.onClick.AddListener(() => { OpenSettings(); });
        closeSettingsButton.onClick.AddListener(() => { CloseSettings(); });
    }
    
    public void StartGame()
    {
        SceneManager.LoadScene(Random.Range(1, 5));
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }
}
