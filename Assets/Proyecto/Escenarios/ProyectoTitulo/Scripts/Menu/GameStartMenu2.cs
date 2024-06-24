using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStartMenu2 : MonoBehaviour
{
    [Header("UI Pages")]
    public GameObject mainMenu;
    public GameObject seleccion;
    public GameObject escena1;
    public GameObject escena2;
    public GameObject escena3;
    public GameObject escena4;

    [Header("Main Menu Button")]
    public Button continuarButton;

    [Header("Seleccion Menu Buttons")]
    public Button escena1Button;
    public Button escena2Button;
    public Button escena3Button;
    public Button escena4Button;
    public Button quitButton;

    [Header("Escena Buttons")]
    public Button jugarEscena1Button;
    public Button volverEscena1Button;
    public Button jugarEscena2Button;
    public Button volverEscena2Button;
    public Button jugarEscena3Button;
    public Button volverEscena3Button;
    public Button jugarEscena4Button;
    public Button volverEscena4Button;

    // Start is called before the first frame update
    void Start()
    {
        EnableMainMenu();

        // Hook events
        continuarButton.onClick.AddListener(ShowSeleccion);

        escena1Button.onClick.AddListener(EnableEscena1);
        escena2Button.onClick.AddListener(EnableEscena2);
        escena3Button.onClick.AddListener(EnableEscena3);
        escena4Button.onClick.AddListener(EnableEscena4);
        quitButton.onClick.AddListener(QuitGame);

        jugarEscena1Button.onClick.AddListener(() => StartGame(1));
        volverEscena1Button.onClick.AddListener(ShowSeleccion);
        jugarEscena2Button.onClick.AddListener(() => StartGame(2));
        volverEscena2Button.onClick.AddListener(ShowSeleccion);
        jugarEscena3Button.onClick.AddListener(() => StartGame(3));
        volverEscena3Button.onClick.AddListener(ShowSeleccion);
        jugarEscena4Button.onClick.AddListener(() => StartGame(4));
        volverEscena4Button.onClick.AddListener(ShowSeleccion);
    }

    public void StartGame(int sceneIndex)
    {
        SceneTransitionManager.singleton.GoToSceneAsync(sceneIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void HideAll()
    {
        mainMenu.SetActive(false);
        seleccion.SetActive(false);
        escena1.SetActive(false);
        escena2.SetActive(false);
        escena3.SetActive(false);
        escena4.SetActive(false);
    }

    public void EnableMainMenu()
    {
        HideAll();
        mainMenu.SetActive(true);
    }

    public void ShowSeleccion()
    {
        HideAll();
        seleccion.SetActive(true);
    }

    public void EnableEscena1()
    {
        HideAll();
        escena1.SetActive(true);
    }

    public void EnableEscena2()
    {
        HideAll();
        escena2.SetActive(true);
    }

    public void EnableEscena3()
    {
        HideAll();
        escena3.SetActive(true);
    }

    public void EnableEscena4()
    {
        HideAll();
        escena4.SetActive(true);
    }
}
