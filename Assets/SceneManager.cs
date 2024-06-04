using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // M�todo para cambiar a la siguiente escena
    public void StartGame()
    {
        // Aseg�rate de que la siguiente escena est� a�adida en la lista de escenas del build
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // M�todo para salir del juego
    public void ExitGame()
    {
        // Cierra el juego en el build final
        Application.Quit();
    }
}
