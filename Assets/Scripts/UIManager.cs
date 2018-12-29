using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject toHide;

    [SerializeField] private Slider timeSlider;

    public void InitSlider(float timer)
    {
        timeSlider.maxValue = timer;
        timeSlider.value = timer;
        timeSlider.gameObject.SetActive(true);
    }
    public void SetSliderValue(float value)
    {

        timeSlider.value = value;
    }
    public void StartGame()
    {
        gameManager.StartGame();
        toHide.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
