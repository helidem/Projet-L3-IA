using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] public Slider slider;
    public static int difficulty;
    [SerializeField] public Toggle aivsai;
    public static bool aivsaiBool;

    [SerializeField] public Slider sliderJoueur1;
    public static int difficultyJoueur1;

    public void Update()
    {
        difficulty = (int)slider.value;
        aivsaiBool = aivsai.isOn;
        if (aivsaiBool)
        {
            sliderJoueur1.gameObject.SetActive(true);
            difficultyJoueur1 = (int)sliderJoueur1.value;
        }
        else
        {
            sliderJoueur1.gameObject.SetActive(false);
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}