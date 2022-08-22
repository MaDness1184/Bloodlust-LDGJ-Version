using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer main;
    AudioSource audioSource;

    [SerializeField] float startingPitch = 0.5f;
    [SerializeField] float gameOverPitch = -0.3f;

    private void Awake()
    {
        if (main == null)
            main = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.pitch = startingPitch;
    }

    private void Update()
    {
        Scene scene = SceneManager.GetActiveScene();

        if (scene.name == "SampleScene" && !audioSource.isPlaying)
            audioSource.Play();
        else if (scene.name == "Main Menu")
            audioSource.Stop();
        else if (scene.name == "Game Over")
            audioSource.pitch = gameOverPitch;
        else
            audioSource.pitch = startingPitch;
    }

}
