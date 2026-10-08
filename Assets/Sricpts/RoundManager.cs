using UnityEngine;

namespace LeagueTry
{
    public class RoundManager : MonoBehaviour
    {
        public PitchMovement player;
        public PlayerJump playerJump;
        public Transform spawnPoint;
        [HideInInspector] public int triesScored;
        [HideInInspector] public int currentRound = 1;
        [HideInInspector] public float lastTryTime = -10f;
        private bool tryRequested;

        private void Start()
        {
            // This is for check references so an incomplete Inspector setup is detected.// Comprobamos las conexiones para detectar un Inspector incompleto.
            
            if (player == null || playerJump == null || spawnPoint == null)
            {
                Debug.LogError("Assign Player, Player Jump and Spawn Point in RoundManager.", this);
                enabled = false;
                return;
            }
            triesScored = 0;
            currentRound = 1;
            ResetPlayer();
        }

        public void RequestTry( PitchMovement enteringPlayer)
        {
            //Only the assigned player can score. A bool combines duplicate requests.// Solo el jugador asignado puede anotar. Bool evita sumar dos avisos juntos.
            
            if (enabled && enteringPlayer == player)
            {
                tryRequested = true;
            }
        }

        private void LateUpdate()
        {
            //this process the try after physics so the reset happens outside its callbacks.// Procesamos el try después de la física para reiniciar con seguridad.
            
            if (tryRequested)
            {
                tryRequested = false;
                triesScored += 1;
                currentRound += 1;
                lastTryTime = Time.time;
                ResetPlayer();
            }
        }

        private void ResetPlayer()
        {
            //Each script handles its own part: position and jumps.// Cada script hace su parte: posición y saltos.
            
            player.ResetPosition(spawnPoint.position);
            playerJump.ResetJumps();
        }
    }
}