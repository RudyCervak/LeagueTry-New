using UnityEngine;

namespace LeagueTry
{
    
    [RequireComponent(typeof(Rigidbody))]
    public class RugbyDefender : MonoBehaviour
    {
        public RoundManager rounds;
        public float safeJumpHeight = 0.45f;

        public float minimumX = -10f;
        public float maximumX = 10f;

        public float startingSpeed = 2f;
        public float speedPerRound = 0.3f;
        public float maximumSpeed = 5f;

        private Rigidbody body;
        private bool movingRight = true;

        private void Awake()
        {
            body = GetComponent<Rigidbody>();

            // The script controls movement without gravity.// El script controla el movimiento sin gravedad.
            
            body.useGravity = false;
            body.isKinematic = true;
        }

        private void FixedUpdate()
        {
            if (rounds == null)
            {
                return;
            }

            // Increase speed each round up to a maximum.// Aumentamos la velocidad por ronda hasta un máximo.
           
            float speed = startingSpeed
                + (rounds.currentRound - 1) * speedPerRound;

            speed = Mathf.Clamp(speed, 0f, maximumSpeed);

            Vector3 nextPosition = body.position;

            // Move across the field without changing Y or Z.// Recorremos el campo lateralmente sin cambiar Y ni Z.
            
            if (movingRight)
            {
                nextPosition.x += speed * Time.fixedDeltaTime;
            }
            else
            {
                nextPosition.x -= speed * Time.fixedDeltaTime;
            }

            // Reverse direction when reaching the limits.// Cambiamos de dirección al alcanzar los límites.
            
            if (nextPosition.x >= maximumX)
            {
                nextPosition.x = maximumX;
                movingRight = false;
            }
            else if (nextPosition.x <= minimumX)
            {
                nextPosition.x = minimumX;
                movingRight = true;
            }

            body.MovePosition(nextPosition);
        }

        private void OnTriggerEnter(Collider other)
        {
            CheckTackle(other);
        }

        private void OnTriggerStay(Collider other)
        {
            // Also check whether the player lands against the defender.// También comprobamos si aterriza junto al defensor.
            
            CheckTackle(other);
        }

        private void CheckTackle(Collider other)
        {
            PitchMovement player =
                other.GetComponentInParent<PitchMovement>();

            if (rounds == null || player == null || player != rounds.player)
            {
                return;
            }

            PlayerJump jump = player.GetComponent<PlayerJump>();

            // Jumping high enough avoids the tackle.// ES: Saltar con suficiente altura evita el tackle.
            
            if (jump != null && jump.GetJumpHeight() >= safeJumpHeight)
            {
                return;
            }

            //The manager checks the shield before resetting.// El manager comprueba el escudo antes de reiniciar.
            
            rounds.RequestTackle(player);
        }
    }
}