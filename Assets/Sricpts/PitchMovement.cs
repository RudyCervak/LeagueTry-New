using UnityEngine;
using UnityEngine.InputSystem;

namespace LeagueTry
{
    public class PitchMovement : MonoBehaviour
    {
        public float moveSpeed = 5f;

        // The Original field limits for the 2D scene: X and Y.// Límites originales de la escena 2D: X e Y.
        
        public Vector2 minimumPosition = new Vector2(-8.35f, -3.15f);
        public Vector2 maximumPosition = new Vector2(8.35f, 2.15f);

        // In 3D, the first value is X and the second is Z.// En 3D, el primer valor es X y el segundo es Z.
        
        public Vector2 minimum3D = new Vector2(-8.5f, 1f);
        public Vector2 maximum3D = new Vector2(8.5f, 58f);

        [HideInInspector] public bool isMoving;

        private Rigidbody body3D;
        private Rigidbody2D body;
        private Vector2 movementInput;

        private void Awake()
        {
            //This player's componet determines whether we use 3D or 2D physics.// El componente del jugador determina si usamos física 3D o 2D.

            body3D = GetComponent<Rigidbody>();
            body = GetComponent<Rigidbody2D>();

            if (body3D == null && body == null)
            {
                Debug.LogError("PitchMovement needs a Rigidbody or Rigidbody2D.", this);
                enabled = false;
            }
        }

        private void Update()
        {
            // Resetting to zero stops the player when the keys are released.// Volver a cero hace que el jugador se detenga al soltar las teclas.
            
            movementInput = Vector2.zero;
            Keyboard keyboard = Keyboard.current;

            if (keyboard != null && Application.isFocused)
            {
                if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
                    movementInput.x -= 1f;

                if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
                    movementInput.x += 1f;

                if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
                    movementInput.y -= 1f;

                if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
                    movementInput.y += 1f;
            }

            // W+D should not move the player faster than W alone.// W+D no debe mover al jugador más rápido que W.
            
            movementInput = Vector2.ClampMagnitude(movementInput, 1f);
            isMoving = movementInput != Vector2.zero;
        }

        private void FixedUpdate()
        {
            float step = moveSpeed * Time.fixedDeltaTime;

            if (body3D != null)
            {
                // W/S moves along Z. Y stays unchanged to keep running and jumping separate.// W/S mueve en Z. Dejamos Y intacta para separar carrera y salto.
                
                Vector3 direction = new Vector3(movementInput.x, 0f, movementInput.y);
                Vector3 next = body3D.position + direction * step;

                next.x = Mathf.Clamp(next.x, minimum3D.x, maximum3D.x);
                next.z = Mathf.Clamp(next.z, minimum3D.y, maximum3D.y);

                body3D.MovePosition(next);
            }
            else if (body != null)
            {
                
               
                Vector2 next = body.position + movementInput * step;

                next.x = Mathf.Clamp(next.x, minimumPosition.x, maximumPosition.x);
                next.y = Mathf.Clamp(next.y, minimumPosition.y, maximumPosition.y);

                body.MovePosition(next);
            }
        }

        public void ResetPosition(Vector3 position)
        {
            // The RoundManager calls this method when resetting the round.// RoundManager llama este método al reiniciar la ronda.
            
            movementInput = Vector2.zero;
            isMoving = false;

            if (body3D != null)
            {
                position.x = Mathf.Clamp(position.x, minimum3D.x, maximum3D.x);
                position.z = Mathf.Clamp(position.z, minimum3D.y, maximum3D.y);
                body3D.position = position;
            }
            else if (body != null)
            {
                position.x = Mathf.Clamp(position.x, minimumPosition.x, maximumPosition.x);
                position.y = Mathf.Clamp(position.y, minimumPosition.y, maximumPosition.y);
                body.position = new Vector2(position.x, position.y);
            }
        }

        private void OnDisable()
        {
            movementInput = Vector2.zero;
            isMoving = false;
        }
    }
}