using UnityEngine;
using UnityEngine.InputSystem;

namespace LeagueTry
{
    
    public class PitchMovement : MonoBehaviour
    {
        public float moveSpeed = 5f;

        // Original 2D scene limits: X and Y.// Límites de la escena original 2D: X e Y.
        
        public Vector2 minimumPosition = new Vector2(-8.35f, -3.15f);
        public Vector2 maximumPosition = new Vector2(8.35f, 2.15f);

        // In 3D, these values represent X and Z.// En 3D, estos valores representan X y Z.
        
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

            // A physics body is required to move the player.// Necesitamos un cuerpo físico para mover al jugador.
            
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
            // Movement returns to zero when the keys are released. // Al soltar las teclas, el movimiento vuelve a cero.
            
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

            // Limit diagonal movement so it is not faster.// Limitamos la diagonal para que no sea más rápida.
            
            movementInput = Vector2.ClampMagnitude(movementInput, 1f);
            isMoving = movementInput != Vector2.zero;
        }

        private void FixedUpdate()
        {
            // Restore normal speed when the timer expires.// Al terminar el tiempo, recuperamos la velocidad normal.
            
            if (Time.time >= boostEndTime)
            {
                ResetSpeedBoost();
            }

            float step =
                moveSpeed * speedMultiplier * Time.fixedDeltaTime;

            if (body3D != null)
            {
                // Move on X/Z without changing the player's height.// Movemos en X/Z sin cambiar la altura del jugador.
                
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
                // Conservamos el movimiento de la versión 2D.// Keep movement for the 2D version.
                
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
            // Renew the effect without stacking multipliers.// Renovamos el efecto sin acumular multiplicadores.
            
            speedMultiplier = Mathf.Max(1f, multiplier);
            boostEndTime = Time.time + Mathf.Max(0f, duration);
        }

        public void ResetSpeedBoost()
        {
            // A value of one preserves the base speed.// El valor uno mantiene la velocidad base.
            
            speedMultiplier = 1f;
            boostEndTime = 0f;
        }

        public void ResetPosition(Vector3 position)
        {
            // Restarting the attempt also cancels the temporary speed boost.// Reiniciar el intento también cancela la velocidad temporal.
            
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