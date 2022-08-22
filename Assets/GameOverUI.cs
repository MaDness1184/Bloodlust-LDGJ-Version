using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] TMP_Text scoreText;

    [SerializeField] float fadeRate = 1;
    [SerializeField] float waitDur = 3f;

    float nextTransition;

    private void Start()
    {
        scoreText.text = $"Days Survived: {GameManager.dayScore}" +
            $"\n Total Kills: {GameManager.killScore} ";
        nextTransition = Time.time + waitDur;
    }

    private void Update()
    {
        if (Time.time < nextTransition) return;

        text.alpha -= Time.deltaTime * fadeRate;
        scoreText.alpha -= Time.deltaTime * fadeRate;
        if (text.alpha <= 0)
            SceneManager.LoadScene(0);
    }
}
