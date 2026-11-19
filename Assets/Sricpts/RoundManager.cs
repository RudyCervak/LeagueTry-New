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

            triesScored = 0;
            currentRound = 1;
            lastTryTime = -10f;
            lastTackleTime = -10f;

            ResetPlayer();
        }

        public void RequestTry(PitchMovement enteringPlayer)
        {
            // Only the assigned player can score.// Solo el jugador asignado puede anotar.
           
            if (enabled && enteringPlayer == player)
            {
                tryRequested = true;
            }
        }

        public void RequestTackle(PitchMovement enteringPlayer)
        {
            // Ignore invalid or duplicate requests.// Ignoramos solicitudes incorrectas o repetidas.
            
            if (!enabled ||
                enteringPlayer != player ||
                tryRequested ||
                tackleRequested)
            {
                return;
            }

            // A shield can block the tackle.// El escudo puede bloquear el tackle.
            
            PlayerShield shield = player.GetComponent<PlayerShield>();

            if (shield != null && shield.TryBlockTackle())
            {
                return;
            }

            tackleRequested = true;
        }

        private void LateUpdate()
        {
            // Process results outside the physics callbacks.// Procesamos los resultados fuera de los eventos de física.
           
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

                // Record the tackle time for the HUD. // Guardamos el momento del tackle para el HUD.
                
                lastTackleTime = Time.time;

                ResetPlayer();
            }
        }

        private void ResetPlayer()
        {
            // EN: Restore the position and jumps.// Restauramos la posición y los saltos.

            // ResetPosition also clears the speed boost.// ResetPosition también elimina el boost de velocidad.
            
            player.ResetPosition(spawnPoint.position);
            playerJump.ResetJumps();

            PlayerShield shield = player.GetComponent<PlayerShield>();

            if (shield != null)
            {
                shield.ResetShield();
            }

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