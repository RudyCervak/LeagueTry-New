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

            // ES: El script controla el movimiento sin gravedad.
            // EN: The script controls movement without gravity.
            body.useGravity = false;
            body.isKinematic = true;
        }

        private void FixedUpdate()
        {
            if (rounds == null)
            {
                return;
            }

            // ES: Aumentamos la velocidad por ronda hasta un máximo.
            // EN: Increase speed each round up to a maximum.
            float speed = startingSpeed
                + (rounds.currentRound - 1) * speedPerRound;

            speed = Mathf.Clamp(speed, 0f, maximumSpeed);

            Vector3 nextPosition = body.position;

            // ES: Recorremos el campo lateralmente sin cambiar Y ni Z.
            // EN: Move across the field without changing Y or Z.
            if (movingRight)
            {
                nextPosition.x += speed * Time.fixedDeltaTime;
            }
            else
            {
                nextPosition.x -= speed * Time.fixedDeltaTime;
            }

            // ES: Cambiamos de dirección al alcanzar los límites.
            // EN: Reverse direction when reaching the limits.
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
            // ES: También comprobamos si aterriza junto al defensor.
            // EN: Also check whether the player lands against the defender.
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

            // ES: Saltar con suficiente altura evita el tackle.
            // EN: Jumping high enough avoids the tackle.
            if (jump != null && jump.GetJumpHeight() >= safeJumpHeight)
            {
                return;
            }

            // ES: El manager comprueba el escudo antes de reiniciar.
            // EN: The manager checks the shield before resetting.
            rounds.RequestTackle(player);
        }
    }
}