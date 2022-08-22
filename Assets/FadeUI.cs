using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeUI : MonoBehaviour
{
    public static FadeUI main;

    [SerializeField] Image fadeImage;
    [SerializeField] float fadeRate = 1;
    // Start is called before the first frame update

    bool fade = true;
    private void Awake()
    {
        main = this;
        Color temp;
        ColorUtility.TryParseHtmlString("#0F0F0F", out temp);
        fadeImage.color = temp;
    }

    // Update is called once per frame
    void Update()
    {
        if (fade && fadeImage.color.a > 0)
        {
            fadeImage.color = new Color(fadeImage.color.r,
            fadeImage.color.g,
            fadeImage.color.b,
            fadeImage.color.a -
            fadeRate * Time.deltaTime);
            
        }
        //Debug.Log("alpha = " + fadeImage.color.a);
    }

    public void StartFadeCoroutine()
    {
        Debug.Log("Start Coroutine");
        StartCoroutine(FadeInCoroutine());
    }

    IEnumerator FadeInCoroutine()
    {
        fade = false;
        if (fadeImage.color.a < 0)
            fadeImage.color = new Color(fadeImage.color.r,
            fadeImage.color.g,
            fadeImage.color.b,
            0);
        //Debug.Log("Coroutine Started");
        while (fadeImage.color.a < 1)
        {
            
            fadeImage.color = new Color(fadeImage.color.r,
                fadeImage.color.g,
                fadeImage.color.b,
                fadeImage.color.a +
                fadeRate * Time.deltaTime);

            yield return null;
        }

        SceneManager.LoadScene(2);
    }
}
