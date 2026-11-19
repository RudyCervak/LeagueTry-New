using UnityEngine;
using UnityEngine.UI;

namespace LeagueTry
{
    public class PrototypeHUD : MonoBehaviour
    {
        public RoundManager rounds;
        public PlayerJump playerJump;
        public PitchMovement player;
        public PlayerShield playerShield;

        public Text triesText;
        public Text roundText;
        public Text jumpsText;
        public Text stateText;
        public Text shieldText;
        public Text speedText;
        public Text messageText;

        private void Start()
        {
            // Check all Inspector references. // Comprobamos las referencias del Inspector.
            
            if (rounds == null ||
                playerJump == null ||
                player == null ||
                playerShield == null ||
                triesText == null ||
                roundText == null ||
                jumpsText == null ||
                stateText == null ||
                shieldText == null ||
                speedText == null ||
                messageText == null)
            {
                Debug.LogError(
                    "PrototypeHUD: assign all references in the Inspector.",
                    this
                );

                enabled = false;
                return;
            }
        }

        private void Update()
        {
            // Display score, round and remaining jumps.// Mostramos tries, ronda y saltos restantes.
           
            triesText.text = "TRIES  " + rounds.triesScored;
            roundText.text = "ROUND  " + rounds.currentRound;

            jumpsText.text =
                "JUMPS  " + playerJump.jumpsRemaining +
                " / " + playerJump.jumpsPerRound;

            // Display the movement state.// Mostramos el estado del movimiento.
            
            if (playerJump.isJumping)
            {
                stateText.text = "AIRBORNE";
            }
            else if (player.isMoving)
            {
                stateText.text = "RUNNING";
            }
            else
            {
                stateText.text = "READY";
            }

            //  Show whether a shield charge is available.// Indicamos si hay una carga de escudo disponible.
            
            if (playerShield.hasShield)
            {
                shieldText.text = "SHIELD  READY";
                shieldText.color = Color.cyan;
            }
            else
            {
                shieldText.text = "SHIELD  OFF";
                shieldText.color = Color.white;
            }

            // Display the remaining speed boost time.// Mostramos el tiempo restante del boost.
            
            float boostTime = player.GetSpeedBoostTimeRemaining();

            if (boostTime > 0f)
            {
                speedText.text =
                    "SPEED BOOST  " + boostTime.ToString("F1") + "s";

                speedText.color = Color.yellow;
            }
            else
            {
                speedText.text = "SPEED  NORMAL";
                speedText.color = Color.white;
            }

            // Show the latest result for 1.5 seconds.// Mostramos el último resultado durante 1.5 segundos.
            
            if (Time.time - rounds.lastTackleTime < 1.5f &&
                rounds.lastTackleTime >= rounds.lastTryTime)
            {
                messageText.text = "TACKLED! Try again";
                messageText.color = Color.red;
            }
            else if (Time.time - rounds.lastTryTime < 1.5f)
            {
                messageText.text = "TRY! Jumps restored";
                messageText.color = Color.green;
            }
            else if (playerJump.jumpsRemaining == 0 &&
                     !playerJump.isJumping)
            {
                messageText.text =
                    "No jumps left - run to the try zone";

                messageText.color = Color.white;
            }
            else
            {
                messageText.text =
                    "Reach the try zone to start the next round";

                messageText.color = Color.white;
            }
        }
    }
}