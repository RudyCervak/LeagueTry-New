using UnityEngine;
using UnityEngine.SceneManagement;

namespace LeagueTry
{
    public class MainMenuController : MonoBehaviour
    {
        public string gameSceneName = "LeagueTry3D";

        private void Start()
        {
            // Keep normal time while the menu is open.// Mantenemos el tiempo normal mientras el menú está abierto.

            Time.timeScale = 1f;
        }

        public void PlayGame()
        {
            // Open the game scene and start a new run.// Abrimos la escena del juego y comenzamos una partida nueva.

            Time.timeScale = 1f;

            SceneManager.LoadScene(gameSceneName);
        }
    }
}