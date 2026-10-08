using UnityEngine;
using UnityEngine.UI;

namespace LeagueTry
{
    public class PrototypeHUD : MonoBehaviour
    {
        public RoundManager rounds;
        public PlayerJump playerJump;
        public PitchMovement player;
        public Text triesText;
        public Text roundText;
        public Text jumpsText;
        public Text stateText;
        public Text messageText;

        private void Update()
        {
            //This is for Convert numbers to text. The interface does not change game rules.// Convertimos los números en texto. La interfaz no cambia las reglas.
            
            triesText.text = "TRIES  " + rounds.triesScored;
            roundText.text = "ROUND  " + rounds.currentRound;
            jumpsText.text = "JUMPS  " + playerJump.jumpsRemaining + " / " + playerJump.jumpsPerRound;

            // Use if / else to display one state at a time.// Usamos if / else para mostrar un estado a la vez.
            
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

            //The try message lasts 1.5 seconds; then the instruction returns.//El aviso de try dura 1.5 segundos; luego vuelve la instrucción.
            
            if (Time.time - rounds.lastTryTime < 1.5f)
            {
                messageText.text = "TRY! Two jumps restored";
            }
            else if (playerJump.jumpsRemaining == 0 && !playerJump.isJumping)
            {
                messageText.text = "No jumps left - run to the gold zone";
            }
            else
            {
                messageText.text = "Reach the gold zone to start the next round";
            }
        }
    }
}