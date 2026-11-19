using UnityEngine;

namespace LeagueTry
{
    
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

            // The player manages duration even when the pickup is disabled.// El jugador controla la duración aunque el pickup se desactive.
            
            player.ActivateSpeedBoost(multiplier, duration);
            gameObject.SetActive(false);
        }
    }
}