using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Método para cambiar a la siguiente escena
    public void StartGame()
    {
        // Asegúrate de que la siguiente escena esté añadida en la lista de escenas del build
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // Método para salir del juego
    public void ExitGame()
    {
        // Cierra el juego en el build final
        Application.Quit();
    }
}
