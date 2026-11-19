using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace LeagueTry
{
    public class RoundManager : MonoBehaviour
    {
        public PitchMovement player;
        public PlayerJump playerJump;
        public Transform spawnPoint;

        public GameObject shieldPickup;
        public GameObject speedPickup;

        public PickupRandomizer pickupRandomizer;
        public DefenderSpawner defenderSpawner;
        public ConversionKick conversionKick;

        [Min(1)] public int triesPerConversion = 5;

        public int startingLives = 5;
        public string mainMenuScene = "MainMenu";

        public Text livesText;
        public Text recordText;
        public GameObject gameOverPanel;
        public Text gameOverText;

        [HideInInspector] public int triesScored;
        [HideInInspector] public int currentRound = 1;
        [HideInInspector] public int livesRemaining;
        [HideInInspector] public int personalBest;

        [HideInInspector] public bool isGameOver;
        [HideInInspector] public bool isConversionActive;

        [HideInInspector] public float lastTryTime = -10f;
        [HideInInspector] public float lastTackleTime = -10f;

        private bool tryRequested;
        private bool tackleRequested;
        private bool newRecordAchieved;

        private bool conversionResultRequested;
        private bool conversionWasSuccessful;

        private const string recordKey =
            "LeagueTry_PersonalBest";

        private void Start()
        {
            // Check the required references.// Comprobamos las referencias necesarias.

            if (player == null ||
                playerJump == null ||
                spawnPoint == null)
            {
                Debug.LogError(
                    "Assign Player, Player Jump and Spawn Point in RoundManager.",
                    this
                );

                enabled = false;
                return;
            }

            // A new game starts with normal time and full lives.// Una partida nueva comienza con tiempo normal y todas las vidas.

            Time.timeScale = 1f;

            startingLives = Mathf.Max(1, startingLives);
            livesRemaining = startingLives;

            triesScored = 0;
            currentRound = 1;

            lastTryTime = -10f;
            lastTackleTime = -10f;

            isGameOver = false;
            isConversionActive = false;

            tryRequested = false;
            tackleRequested = false;
            newRecordAchieved = false;

            conversionResultRequested = false;
            conversionWasSuccessful = false;

            // Load the record saved on this device.// Cargamos el récord guardado en este dispositivo.

            personalBest = PlayerPrefs.GetInt(recordKey, 0);

            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(false);
            }

            ResetPlayer();
            UpdateLivesAndRecord();
        }

        public void RequestTry(PitchMovement enteringPlayer)
        {
            // Only the assigned player can score.// Solo el jugador asignado puede anotar.

            if (enabled &&
                !isGameOver &&
                !isConversionActive &&
                enteringPlayer == player)
            {
                tryRequested = true;
            }
        }

        public void RequestTackle(PitchMovement enteringPlayer)
        {
            // Ignore invalid or duplicate requests.// Ignoramos solicitudes incorrectas o repetidas.

            if (!enabled ||
                isGameOver ||
                isConversionActive ||
                enteringPlayer != player ||
                tryRequested ||
                tackleRequested)
            {
                return;
            }

            // A shield can block the tackle.// El escudo puede bloquear el tackle.

            PlayerShield shield =
                player.GetComponent<PlayerShield>();

            if (shield != null && shield.TryBlockTackle())
            {
                return;
            }

            tackleRequested = true;
        }

        private void LateUpdate()
        {
            // Stop processing results after the game ends.// Dejamos de procesar resultados cuando termina la partida.

            if (isGameOver)
            {
                return;
            }

            // Finish the kick after input processing to avoid an accidental jump.// Terminamos la patada después de procesar el input para evitar un salto accidental.

            if (isConversionActive)
            {
                if (conversionResultRequested)
                {
                    FinishConversion(conversionWasSuccessful);
                }
                else if (conversionKick == null ||
                         !conversionKick.IsRunning)
                {
                    FinishConversion(false);
                }

                return;
            }

            // Process results outside the physics callbacks.// Procesamos los resultados fuera de los eventos de física.

            if (tryRequested)
            {
                tryRequested = false;
                tackleRequested = false;

                triesScored += 1;
                currentRound += 1;

                lastTryTime = Time.time;

                SavePersonalBest();
                UpdateLivesAndRecord();

                // Offer one conversion after every five tries by default.// Ofrecemos una conversión cada cinco tries de forma predeterminada.

                if (triesScored %
                    Mathf.Max(1, triesPerConversion) == 0 &&
                    TryBeginConversion())
                {
                    return;
                }

                ResetPlayer();
            }
            else if (tackleRequested)
            {
                tackleRequested = false;

                // Record the tackle time for the HUD. // Guardamos el momento del tackle para el HUD.

                lastTackleTime = Time.time;

                // An unblocked tackle removes one life.// Un tackle sin bloquear quita una vida.

                livesRemaining = Mathf.Max(
                    0,
                    livesRemaining - 1
                );

                UpdateLivesAndRecord();

                if (livesRemaining <= 0)
                {
                    EndGame();
                }
                else
                {
                    ResetPlayer();
                }
            }
        }

        private bool TryBeginConversion()
        {
            if (conversionKick == null)
            {
                Debug.LogWarning(
                    "Assign Conversion Kick in RoundManager.",
                    this
                );

                return false;
            }

            isConversionActive = true;

            conversionResultRequested = false;
            conversionWasSuccessful = false;

            // Disable movement and jumping during the conversion.// Desactivamos el movimiento y el salto durante la conversión.

            player.enabled = false;
            playerJump.enabled = false;

            if (conversionKick.BeginKick(this))
            {
                return true;
            }

            // Continue normal play if the conversion has not been configured.// Continuamos la partida normal si la conversión no está configurada.

            isConversionActive = false;

            player.enabled = true;
            playerJump.enabled = true;

            return false;
        }

        public void RequestConversionResult(bool success)
        {
            // Accept only one result for the active conversion.// Aceptamos un solo resultado para la conversión activa.

            if (!isConversionActive ||
                isGameOver ||
                conversionResultRequested)
            {
                return;
            }

            conversionWasSuccessful = success;
            conversionResultRequested = true;
        }

        private void FinishConversion(bool success)
        {
            if (conversionKick != null)
            {
                conversionKick.CancelKick();
            }

            isConversionActive = false;
            conversionResultRequested = false;

            tryRequested = false;
            tackleRequested = false;

            // A successful kick restores one life up to the starting maximum.// Una patada acertada recupera una vida hasta el máximo inicial.

            if (success)
            {
                livesRemaining = Mathf.Min(
                    startingLives,
                    livesRemaining + 1
                );
            }

            // Missing the kick does not remove a life.// Fallar la patada no quita una vida.

            Time.timeScale = 1f;

            ResetPlayer();

            player.enabled = true;
            playerJump.enabled = true;

            UpdateLivesAndRecord();
        }

        private void SavePersonalBest()
        {
            // Save only when the player beats the previous record.// Guardamos solo cuando el jugador supera el récord anterior.

            if (triesScored <= personalBest)
            {
                return;
            }

            personalBest = triesScored;
            newRecordAchieved = true;

            PlayerPrefs.SetInt(recordKey, personalBest);
            PlayerPrefs.Save();
        }

        private void UpdateLivesAndRecord()
        {
            // Show the remaining lives and the personal record.// Mostramos las vidas restantes y el récord personal.

            if (livesText != null)
            {
                livesText.text =
                    "LIVES: " +
                    livesRemaining +
                    "/" +
                    startingLives;
            }

            if (recordText != null)
            {
                recordText.text = "BEST: " + personalBest;
            }
        }

        private void EndGame()
        {
            // End the game once all lives are lost.// Terminamos la partida cuando se pierden todas las vidas.

            isGameOver = true;

            tryRequested = false;
            tackleRequested = false;

            // Disable player controls and pause the action.// Desactivamos los controles del jugador y pausamos la acción.

            player.enabled = false;
            playerJump.enabled = false;

            Time.timeScale = 0f;

            // Show the final score and whether a new record was achieved.// Mostramos la puntuación final y si se consiguió un nuevo récord.

            if (gameOverText != null)
            {
                gameOverText.text =
                    "GAME OVER\n\n" +
                    "TRIES: " + triesScored + "\n" +
                    "PERSONAL BEST: " + personalBest;

                if (newRecordAchieved)
                {
                    gameOverText.text +=
                        "\n\nNEW PERSONAL BEST!";
                }
            }

            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }
        }

        private void LeaveConversion()
        {
            isConversionActive = false;
            conversionResultRequested = false;

            if (conversionKick != null)
            {
                conversionKick.CancelKick();
            }
        }

        public void RestartGame()
        {
            // Reload the current scene to start a fresh game.// Recargamos la escena actual para comenzar una partida nueva.

            LeaveConversion();

            Time.timeScale = 1f;

            SceneManager.LoadScene(
                SceneManager.GetActiveScene().name
            );
        }

        public void ReturnToMenu()
        {
            // Restore normal time before opening the main menu.// Restauramos el tiempo normal antes de abrir el menú principal.

            LeaveConversion();

            Time.timeScale = 1f;

            SceneManager.LoadScene(mainMenuScene);
        }

        private void ResetPlayer()
        {
            // EN: Restore the position and jumps.// Restauramos la posición y los saltos.

            // ResetPosition also clears the speed boost.// ResetPosition también elimina el boost de velocidad.

            player.ResetPosition(spawnPoint.position);
            playerJump.ResetJumps();

            PlayerShield shield =
                player.GetComponent<PlayerShield>();

            if (shield != null)
            {
                shield.ResetShield();
            }

            // Prepare defenders before choosing pickup positions.// Preparamos los defensores antes de elegir las posiciones de los powerups.

            if (defenderSpawner != null)
            {
                defenderSpawner.PrepareRound(this);
            }

            // Choose new pickup positions for the next attempt.// Elegimos nuevas posiciones para los powerups del siguiente intento.

            if (pickupRandomizer != null)
            {
                pickupRandomizer.RespawnPickups(
                    shieldPickup,
                    speedPickup,
                    spawnPoint.position
                );
            }
            else
            {
                //  Make the pickups available again.// Hacemos que los pickups vuelvan a estar disponibles.

                if (shieldPickup != null)
                {
                    shieldPickup.SetActive(true);
                }

                if (speedPickup != null)
                {
                    speedPickup.SetActive(true);
                }
            }
        }
    }
}