using UnityEngine;
using UnityEngine.InputSystem;

namespace LeagueTry
{
    
    public class PitchMovement : MonoBehaviour
    {
        public float moveSpeed = 5f;

        // ES: Límites de la escena original 2D: X e Y.
        // EN: Original 2D scene limits: X and Y.
        public Vector2 minimumPosition = new Vector2(-8.35f, -3.15f);
        public Vector2 maximumPosition = new Vector2(8.35f, 2.15f);

        // ES: En 3D, estos valores representan X y Z.
        // EN: In 3D, these values represent X and Z.
        public Vector2 minimum3D = new Vector2(-8.5f, 1f);
        public Vector2 maximum3D = new Vector2(8.5f, 58f);

        [HideInInspector] public bool isMoving;

        private Rigidbody body3D;
        private Rigidbody2D body;
        private Vector2 movementInput;

        private float speedMultiplier = 1f;
        private float boostEndTime;

        private void Awake()
        {
            body3D = GetComponent<Rigidbody>();
            body = GetComponent<Rigidbody2D>();

            // ES: Necesitamos un cuerpo físico para mover al jugador.
            // EN: A physics body is required to move the player.
            if (body3D == null && body == null)
            {
                Debug.LogError(
                    "PitchMovement needs a Rigidbody or Rigidbody2D.",
                    this
                );

                enabled = false;
            }
        }

        private void Update()
        {
            // ES: Al soltar las teclas, el movimiento vuelve a cero.
            // EN: Movement returns to zero when the keys are released.
            movementInput = Vector2.zero;
            Keyboard keyboard = Keyboard.current;

            if (keyboard != null && Application.isFocused)
            {
                if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
                {
                    movementInput.x -= 1f;
                }

                if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
                {
                    movementInput.x += 1f;
                }

                if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
                {
                    movementInput.y -= 1f;
                }

                if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
                {
                    movementInput.y += 1f;
                }
            }

            // ES: Limitamos la diagonal para que no sea más rápida.
            // EN: Limit diagonal movement so it is not faster.
            movementInput = Vector2.ClampMagnitude(movementInput, 1f);
            isMoving = movementInput != Vector2.zero;
        }

        private void FixedUpdate()
        {
            // ES: Al terminar el tiempo, recuperamos la velocidad normal.
            // EN: Restore normal speed when the timer expires.
            if (Time.time >= boostEndTime)
            {
                ResetSpeedBoost();
            }

            float step =
                moveSpeed * speedMultiplier * Time.fixedDeltaTime;

            if (body3D != null)
            {
                // ES: Movemos en X/Z sin cambiar la altura del jugador.
                // EN: Move on X/Z without changing the player's height.
                Vector3 direction =
                    new Vector3(movementInput.x, 0f, movementInput.y);

                Vector3 nextPosition = body3D.position + direction * step;

                nextPosition.x = Mathf.Clamp(
                    nextPosition.x, minimum3D.x, maximum3D.x
                );

                nextPosition.z = Mathf.Clamp(
                    nextPosition.z, minimum3D.y, maximum3D.y
                );

                body3D.MovePosition(nextPosition);
            }
            else if (body != null)
            {
                // ES: Conservamos el movimiento de la versión 2D.
                // EN: Keep movement for the 2D version.
                Vector2 nextPosition = body.position + movementInput * step;

                nextPosition.x = Mathf.Clamp(
                    nextPosition.x, minimumPosition.x, maximumPosition.x
                );

                nextPosition.y = Mathf.Clamp(
                    nextPosition.y, minimumPosition.y, maximumPosition.y
                );

                body.MovePosition(nextPosition);
            }
        }

        public void ActivateSpeedBoost(float multiplier, float duration)
        {
            // ES: Renovamos el efecto sin acumular multiplicadores.
            // EN: Renew the effect without stacking multipliers.
            speedMultiplier = Mathf.Max(1f, multiplier);
            boostEndTime = Time.time + Mathf.Max(0f, duration);
        }

        public void ResetSpeedBoost()
        {
            // ES: El valor uno mantiene la velocidad base.
            // EN: A value of one preserves the base speed.
            speedMultiplier = 1f;
            boostEndTime = 0f;
        }

        public void ResetPosition(Vector3 position)
        {
            // ES: Reiniciar el intento también cancela la velocidad temporal.
            // EN: Restarting the attempt also cancels the temporary speed boost.
            movementInput = Vector2.zero;
            isMoving = false;
            ResetSpeedBoost();

            if (body3D != null)
            {
                position.x = Mathf.Clamp(
                    position.x, minimum3D.x, maximum3D.x
                );

                position.z = Mathf.Clamp(
                    position.z, minimum3D.y, maximum3D.y
                );

                body3D.position = position;
            }
            else if (body != null)
            {
                position.x = Mathf.Clamp(
                    position.x, minimumPosition.x, maximumPosition.x
                );

                position.y = Mathf.Clamp(
                    position.y, minimumPosition.y, maximumPosition.y
                );

                body.position = new Vector2(position.x, position.y);
            }
       
        }



        public float GetSpeedBoostTimeRemaining()
        {
            if (speedMultiplier <= 1f)
            {
                return 0f;
            }

            return Mathf.Max(0f, boostEndTime - Time.time);

        }
        private void OnDisable()
        {
            movementInput = Vector2.zero;
            isMoving = false;
            ResetSpeedBoost();
        }
    }
}