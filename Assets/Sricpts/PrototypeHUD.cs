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
            // EN: Check the Inspector references before updating the HUD.
            // ES: Comprobamos las referencias antes de actualizar el HUD.
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
            // EN: Display the current score, round and available jumps.
            // ES: Mostramos los tries, la ronda y los saltos disponibles.
            triesText.text = "TRIES  " + rounds.triesScored;
            roundText.text = "ROUND  " + rounds.currentRound;

            jumpsText.text =
                "JUMPS  " + playerJump.jumpsRemaining +
                " / " + playerJump.jumpsPerRound;

            // EN: Display one movement state at a time.
            // ES: Mostramos un estado de movimiento a la vez.
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

            // EN: Show whether a shield is available to block a tackle.
            // ES: Indicamos si queda un escudo para bloquear un tackle.
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

            // EN: Read the boost timer without changing movement.
            // ES: Consultamos el tiempo del boost sin cambiar el movimiento.
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

            // EN: Show a short scoring message or the current objective.
            // ES: Mostramos un aviso de try o el objetivo actual.
            if (Time.time - rounds.lastTryTime < 1.5f)
            {
                messageText.text = "TRY! Jumps restored";
            }
            else if (playerJump.jumpsRemaining == 0 &&
                     !playerJump.isJumping)
            {
                messageText.text =
                    "No jumps left - run to the try zone";
            }
            else
            {
                messageText.text =
                    "Reach the try zone to start the next round";
            }
        }
    }
}