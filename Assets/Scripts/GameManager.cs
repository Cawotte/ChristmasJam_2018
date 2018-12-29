using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;
    public Player player;

    public float timer = 120f;
    public float timeLeft;
    public bool gameHasStarted = false;

    private void Update()
    {
        if (!gameHasStarted) return;

        timeLeft -= Time.deltaTime;

        uiManager.SetSliderValue(Mathf.Max(0f, timeLeft));

        if (timeLeft <= 0f)
        {
            ResetGame();
        }
    }

    public void StartGame()
    {
        gameHasStarted = true;
        timeLeft = timer;
        player.gameObject.SetActive(true);
        uiManager.InitSlider(timer);
    }


    public void ResetGame()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

}
