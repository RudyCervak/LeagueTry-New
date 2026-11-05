using UnityEngine;

namespace LeagueTry
{
    // ES: Creado con ayuda de ChatGPT para recoger velocidad temporal.
    // EN: Created with ChatGPT assistance to collect a temporary speed boost.
    public class SpeedPickup : MonoBehaviour
    {
        public float multiplier = 1.5f;
        public float duration = 4f;

        private void OnTriggerEnter(Collider other)
        {
            PitchMovement player =
                other.GetComponentInParent<PitchMovement>();

            if (player == null)
            {
                return;
            }

            // ES: El jugador controla la duración aunque el pickup se desactive.
            // EN: The player manages duration even when the pickup is disabled.
            player.ActivateSpeedBoost(multiplier, duration);
            gameObject.SetActive(false);
        }
    }
}