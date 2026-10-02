using UnityEngine;
using UnityEngine.InputSystem;

namespace LeagueTry
{
    public class PitchMovement : MonoBehaviour
    {
      
        public float moveSpeed = 5f;
        public Vector2 minimumPosition = new Vector2(-8.35f, -3.15f);
        public Vector2 maximumPosition = new Vector2(8.35f, 2.15f);
        [HideInInspector] public bool isMoving;

        // The Private stores internal data that other scripts do not need to change.//Private guarda datos internos que otros scripts no necesitan cambiar.
        
        private Rigidbody2D body;
        private Vector2 movementInput;

        private void Awake()
        {
            // We store the component once so we can move it later.// Guardamos el componente una vez para poder moverlo después.
           
            body = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            // The reset the direction so movement stops when the keys are released.// Reiniciamos la dirección para detenernos al soltar las teclas.
            
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

            // the limit the length to 1 so diagonal movement is not faster.// Limitamos la longitud a 1 para que la diagonal no sea más rápida.
            
            movementInput = Vector2.ClampMagnitude(movementInput, 1f);
            isMoving = movementInput != Vector2.zero;
        }

        private void FixedUpdate()
        {
            // The physics moves in FixedUpdate, using the time between physics steps.// La física se mueve en FixedUpdate, usando el tiempo entre pasos.
            
            Vector2 nextPosition = body.position;
            nextPosition += movementInput * moveSpeed * Time.fixedDeltaTime;

            // the clamp keeps the position between the field minimum and maximum.//Clamp mantiene la posición entre el mínimo y el máximo del campo.
            
            nextPosition.x = Mathf.Clamp(nextPosition.x, minimumPosition.x, maximumPosition.x);
            nextPosition.y = Mathf.Clamp(nextPosition.y, minimumPosition.y, maximumPosition.y);
            body.MovePosition(nextPosition);
        }

        public void ResetPosition(Vector2 position)
        {
            // The manager calls this method when another round begins.//  El manager llama este método al comenzar otra ronda.
            
            movementInput = Vector2.zero;
            isMoving = false;
            position.x = Mathf.Clamp(position.x, minimumPosition.x, maximumPosition.x);
            position.y = Mathf.Clamp(position.y, minimumPosition.y, maximumPosition.y);
            body.position = position;
        }

        private void OnDisable()
        {
            //  Evitamos conservar una dirección al desactivar el componente.  // This is for avoid keeping a movement direction when the component is disabled.
          
            movementInput = Vector2.zero;
            isMoving = false;
        }
    }
}