using UnityEngine;

namespace LeagueTry
{
    
    public class TryZone : MonoBehaviour
    {
        public RoundManager rounds;

        private void OnTriggerEnter(Collider other)
        {
            // ES: Buscamos el movimiento en el objeto o en su padre.
            // Esto permite detectar colliders que estén en hijos del jugador.
            // EN: Find movement on this object or its parent.
            // This also supports colliders on the player's children.
            PitchMovement player =
                other.GetComponentInParent<PitchMovement>();

            if (player != null && rounds != null)
            {
                // ES: El manager se encarga de anotar y reiniciar la ronda.
                // EN: The manager handles scoring and resetting the round.
                rounds.RequestTry(player);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // ES: Conservamos la detección para la escena original 2D.
            // EN: Keep detection for the original 2D scene.
            PitchMovement player =
                other.GetComponentInParent<PitchMovement>();

            if (player != null && rounds != null)
            {
                rounds.RequestTry(player);
            }
        }
    }
}