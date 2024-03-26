using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Manager : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;
    //public TextMeshProUGUI endTimerText;
    //public TextMeshProUGUI endScoreText;
    public GameObject playerUI;
    public GameObject endScreen;
    public GameObject player;
    private float elapsedTime;
    private int score = 0;

    private void Start() 
    {
        playerUI.SetActive(true);
    }
    private void Update()
    {
        timeUpdate();
    }

    private void timeUpdate() {
        elapsedTime += Time.deltaTime;
        int mins = Mathf.FloorToInt(elapsedTime / 60);
        int secs = Mathf.FloorToInt(elapsedTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", mins, secs);
    }
    public void scoreUp(int value) {
        score += value;
        scoreText.text = score.ToString();
    }
}
