using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public GameObject OptionScreen;
    public void Play()
    {
        SceneManager.LoadScene("Finale Level V2");
    }
    public void OptionButton() // Retour/Charge la sc�ne de menu principale
    {
        OptionScreen.SetActive(true);
    }
    public void BackButton() // Retour/Charge la sc�ne de menu principale
    {
        OptionScreen.SetActive(false);
    }
    public void QuitGame()
    {
        // Quitte l'application
        Application.Quit();
        Debug.Log("Quit Game");
    }
}
