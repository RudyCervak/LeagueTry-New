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
            // This reference is needed to move the character's visual.// Esta referencia es necesaria para mover el dibujo del personaje.
           
            if (visual == null)
            {
                Debug.LogError("Assign Visual / Asigna Visual en PlayerJump.", this);
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
                // this is for detect one press, not a key being held down.// Detectamos una pulsación, no una tecla mantenida.
                
                if (keyboard.spaceKey.wasPressedThisFrame)
                {
                    TryJump();
                }
            }

            if (isJumping)
            {
                // This is fot calculate the upward movement and how gravity reduces its height.//Calculamos cuánto sube y cuánto reduce la gravedad su altura.
                
                float time = Time.deltaTime;
                float upwardMovement = verticalSpeed * time;
                float gravityDrop = 0.5f * gravity * time * time;
                height += upwardMovement - gravityDrop;
                verticalSpeed -= gravity * time;

                //  When the player reaching the ground ends the jump without restoring uses.// Al tocar el suelo terminamos el salto, sin recuperar usos.
                
                if (height <= 0f)
                {
                    height = 0f;
                    verticalSpeed = 0f;
                    isJumping = false;
                }

                visual.localPosition = restingPosition + Vector3.up * height;
            }
        }

        public void TryJump()
        {
            //The player must land and have a use available before jumping.// Hay que aterrizar y tener un uso disponible antes de saltar.
            
            if (isJumping || jumpsRemaining <= 0)
            {
                return;
            }
            jumpsRemaining -= 1;
            isJumping = true;
            verticalSpeed = jumpSpeed;
            height = 0f;
        }

        public void ResetJumps()
        {
            // A new round restores jumps and cancels any jump in progress.//  Una nueva ronda recupera los saltos y cancela un salto en curso.
            
            jumpsRemaining = jumpsPerRound;
            isJumping = false;
            height = 0f;
            verticalSpeed = 0f;
            if (visual != null)
            {
                visual.localPosition = restingPosition;
            }
        }
    }
}