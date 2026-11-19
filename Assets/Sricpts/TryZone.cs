using UnityEngine;

namespace LeagueTry
{
    
    public class TryZone : MonoBehaviour
    {
        public RoundManager rounds;

        private void OnTriggerEnter(Collider other)
        {
            // Find movement on this object or its parent.// ES: Buscamos el movimiento en el objeto o en su padre.
            
            PitchMovement player =
                other.GetComponentInParent<PitchMovement>();

            if (player != null && rounds != null)
            {
                // The manager handles scoring and resetting the round.// El manager se encarga de anotar y reiniciar la ronda.
                
                rounds.RequestTry(player);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // EN: Keep detection for the original 2D scene.// Conservamos la detección para la escena original 2D.
            
            PitchMovement player =
                other.GetComponentInParent<PitchMovement>();

            if (player != null && rounds != null)
            {
                rounds.RequestTry(player);
            }
        }
    }
}