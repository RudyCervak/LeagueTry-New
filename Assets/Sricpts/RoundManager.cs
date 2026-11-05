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

        private bool tryRequested;
        private bool tackleRequested;

        private void Start()
        {
            // ES: Comprobamos las referencias necesarias antes de empezar.
            // EN: Check required references before starting.
            if (player == null || playerJump == null || spawnPoint == null)
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
            ResetPlayer();
        }

        public void RequestTry(PitchMovement enteringPlayer)
        {
            // ES: Solo el jugador asignado puede anotar.
            // EN: Only the assigned player can score.
            if (enabled && enteringPlayer == player)
            {
                tryRequested = true;
            }
        }

        public void RequestTackle(PitchMovement enteringPlayer)
        {
            // ES: Evitamos procesar varias peticiones del mismo contacto.
            // EN: Avoid processing multiple requests from the same contact.
            if (!enabled || enteringPlayer != player ||
                tryRequested || tackleRequested)
            {
                return;
            }

            PlayerShield shield = player.GetComponent<PlayerShield>();

            // ES: Si el escudo bloquea el golpe, no reiniciamos el intento.
            // EN: If the shield blocks the hit, do not restart the attempt.
            if (shield != null && shield.TryBlockTackle())
            {
                return;
            }

            tackleRequested = true;
        }

        private void LateUpdate()
        {
            // ES: Procesamos los reinicios después de los avisos de física.
            // EN: Process resets after physics notifications.
            if (tryRequested)
            {
                // ES: Si coinciden un try y un tackle, el try tiene prioridad.
                // EN: If a try and tackle coincide, the try takes priority.
                tryRequested = false;
                tackleRequested = false;

                triesScored += 1;
                currentRound += 1;
                lastTryTime = Time.time;

                ResetPlayer();
            }
            else if (tackleRequested)
            {
                // ES: El tackle no aumenta los tries ni la ronda.
                // EN: A tackle does not increase tries or the round.
                tackleRequested = false;
                ResetPlayer();
            }
        }

        private void ResetPlayer()
        {
            // ES: ResetPosition también cancela el aumento de velocidad.
            // EN: ResetPosition also cancels the speed boost.
            player.ResetPosition(spawnPoint.position);
            playerJump.ResetJumps();

            // ES: El jugador comienza el nuevo intento sin escudo.
            // EN: The player starts the new attempt without a shield.
            PlayerShield shield = player.GetComponent<PlayerShield>();

            if (shield != null)
            {
                shield.ResetShield();
            }

            // ES: Los dos objetos vuelven a estar disponibles.
            // EN: Both pickups become available again.
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