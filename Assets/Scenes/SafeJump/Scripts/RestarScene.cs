using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartScene : MonoBehaviour
{
    // Método para reiniciar la escena actual
    public void RestartCurrentScene()
    {
        // Obtiene el nombre de la escena actual
        string currentSceneName = SceneManager.GetActiveScene().name;

        // Reinicia la escena actual cargándola de nuevo
        SceneManager.LoadScene(currentSceneName);
    }
}
