using UnityEngine;

namespace LeagueTry
{
    public class RoundManager : MonoBehaviour
    {
        public PitchMovement player;
        public PlayerJump playerJump;
        public Transform spawnPoint;

        public GameObject shieldPickup;
        public GameObject speedPickup;

        [HideInInspector] public int triesScored;
        [HideInInspector] public int currentRound = 1;
        [HideInInspector] public float lastTryTime = -10f;
        [HideInInspector] public float lastTackleTime = -10f;

        private bool tryRequested;
        private bool tackleRequested;

        private void Start()
        {
            // EN: Check the required references.
            // ES: Comprobamos las referencias necesarias.
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

            triesScored = 0;
            currentRound = 1;
            lastTryTime = -10f;
            lastTackleTime = -10f;

            ResetPlayer();
        }

        public void RequestTry(PitchMovement enteringPlayer)
        {
            // EN: Only the assigned player can score.
            // ES: Solo el jugador asignado puede anotar.
            if (enabled && enteringPlayer == player)
            {
                tryRequested = true;
            }
        }

        public void RequestTackle(PitchMovement enteringPlayer)
        {
            // EN: Ignore invalid or duplicate requests.
            // ES: Ignoramos solicitudes incorrectas o repetidas.
            if (!enabled ||
                enteringPlayer != player ||
                tryRequested ||
                tackleRequested)
            {
                return;
            }

            // EN: A shield can block the tackle.
            // ES: El escudo puede bloquear el tackle.
            PlayerShield shield = player.GetComponent<PlayerShield>();

            if (shield != null && shield.TryBlockTackle())
            {
                return;
            }

            tackleRequested = true;
        }

        private void LateUpdate()
        {
            // EN: Process results outside the physics callbacks.
            // ES: Procesamos los resultados fuera de los eventos de física.
            if (tryRequested)
            {
                tryRequested = false;
                tackleRequested = false;

                triesScored += 1;
                currentRound += 1;
                lastTryTime = Time.time;

                ResetPlayer();
            }
            else if (tackleRequested)
            {
                tackleRequested = false;

                // EN: Record the tackle time for the HUD.
                // ES: Guardamos el momento del tackle para el HUD.
                lastTackleTime = Time.time;

                ResetPlayer();
            }
        }

        private void ResetPlayer()
        {
            // EN: Restore the position and jumps.
            // ES: Restauramos la posición y los saltos.
            // ResetPosition also clears the speed boost.
            // ResetPosition también elimina el boost de velocidad.
            player.ResetPosition(spawnPoint.position);
            playerJump.ResetJumps();

            PlayerShield shield = player.GetComponent<PlayerShield>();

            if (shield != null)
            {
                shield.ResetShield();
            }

            // EN: Make the pickups available again.
            // ES: Hacemos que los pickups vuelvan a estar disponibles.
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