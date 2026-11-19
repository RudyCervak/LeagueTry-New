using UnityEngine;
using UnityEngine.InputSystem;

namespace LeagueTry
{
    
    public class PlayerJump : MonoBehaviour
    {
        public Transform visual;
        public int jumpsPerRound = 2;
        public float jumpSpeed = 6f;
        public float gravity = 18f;

        [HideInInspector] public int jumpsRemaining;
        [HideInInspector] public bool isJumping;

        private Vector3 restingPosition;
        private float height;
        private float verticalSpeed;

        private void Awake()
        {
            // EN: Visual must be a child of the player to separate jumping from field movement and keep the shadow on the ground.// ES: Visual debe ser un hijo del jugador para separar el salto  del movimiento por el campo y dejar la sombra en el suelo.

             

            if (visual == null || visual == transform ||
                !visual.IsChildOf(transform))
            {
                Debug.LogError(
                    "PlayerJump: assign the player's Visual child.",
                    this
                );

                enabled = false;
                return;
            }

            restingPosition = visual.localPosition;
            ResetJumps();
        }

        private void Update()
        {
            Keyboard keyboard = Keyboard.current;

            if (keyboard != null && Application.isFocused)
            {
                // Detect a single press, not a held key.// Detectamos una pulsación, no una tecla mantenida.
                
                if (keyboard.spaceKey.wasPressedThisFrame)
                {
                    TryJump();
                }
            }

            if (isJumping)
            {
                // Velocity raises the character and gravity slows it down.// La velocidad eleva al personaje y la gravedad lo frena.
                
                float time = Time.deltaTime;
                float upwardMovement = verticalSpeed * time;
                float gravityDrop = 0.5f * gravity * time * time;

                height += upwardMovement - gravityDrop;
                verticalSpeed -= gravity * time;

                // Landing allows another jump but does not restore uses.// Aterrizar permite otro salto, pero no recupera usos.
                
                if (height <= 0f)
                {
                    height = 0f;
                    verticalSpeed = 0f;
                    isJumping = false;
                }

                visual.localPosition =
                    restingPosition + Vector3.up * height;
            }
        }

        public void TryJump()
        {
            // The player must be grounded and have a jump available.// Hay que estar en el suelo y tener un salto disponible.
            
            if (!enabled || isJumping || jumpsRemaining <= 0)
            {
                return;
            }

            jumpsRemaining -= 1;
            isJumping = true;
            verticalSpeed = jumpSpeed;
            height = 0f;
        }

        public float GetJumpHeight()
        {
            // The defender can read the jump height without changing it.// El defensor puede consultar la altura sin modificarla.
            
            return height;
        }

        public void ResetJumps()
        {
            // EN: Avoid resetting if the Visual reference is invalid.// ES: Evitamos reiniciar si la referencia Visual es incorrecta.
            
            if (visual == null || visual == transform ||
                !visual.IsChildOf(transform))
            {
                return;
            }

            // Restore jumps and cancel any active jump.// Recuperamos los saltos y cancelamos cualquier salto activo.
            
            jumpsRemaining = jumpsPerRound;
            isJumping = false;
            height = 0f;
            verticalSpeed = 0f;

            visual.localPosition = restingPosition;
        }
    }
}