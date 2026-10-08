using UnityEngine;


namespace LeagueTry
{
    public class TryZone : MonoBehaviour
    {
        public RoundManager rounds;

        private void OnTriggerEnter2D(Collider2D other)
        {
            PitchMovement player = other.GetComponent<PitchMovement>();

            if (player != null && rounds != null)
            {
                // EN: Notify the manager; this zone does not change movement.// Avisamos al manager; esta zona no cambia el movimiento.
                
                rounds.RequestTry(player);
            }
        }
    }
}