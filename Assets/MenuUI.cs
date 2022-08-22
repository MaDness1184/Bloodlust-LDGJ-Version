using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
    [SerializeField] TMP_Text anyKeyText;
    [SerializeField] TMP_Text[] allText;
    [SerializeField] float fadeRateAll = 0.1f;
    [SerializeField] float fadeRateAny = 0.5f;

    float direction = 1;
    bool fadedIn = false;


    private void Start()
    {
        foreach(TMP_Text text in allText)
        {
            text.alpha = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!fadedIn)
        {
            foreach (TMP_Text text in allText)
            {
                text.alpha += fadeRateAll * Time.deltaTime;
            }

            if (anyKeyText.alpha >= 1)
                fadedIn = true;
        }
        else
        {
            if (anyKeyText.alpha >= 1 || anyKeyText.alpha <= 0)
                direction *= -1;

            anyKeyText.alpha += direction * fadeRateAny * Time.deltaTime;
        }
    }

    public void OnAnyKey(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            StartGame();
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
}
