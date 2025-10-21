using UnityEngine;
using UnityEngine.InputSystem;

namespace MMUCAVE
{
    /// <summary>
    ///     General movement controller for the CAVE system.
    /// </summary>
    public class KeyboardInputHandler : MonoBehaviour
    {
        [Tooltip("Reference to the CAVE game object")]
        [SerializeField]
        private GameObject cave; //Reference to the CAVE.

        [Header("Settings: Movement")]
        [Tooltip("Keyboard controlled movement speed")]
        [SerializeField]
        private float movementSpeed = 10f;

        [Tooltip("Keyboard controlled rotation speed")]
        [SerializeField]
        private float rotationSpeed = 100f;

        //Stores the directions passed from the input action callback for use with FixedUpdate.
        private Vector3 moveDirection   = Vector3.zero;
        private Vector2 rotateDirection = Vector2.zero;

        public void FixedUpdate()
        {
            //Perform transforms on the cave to apply the directions stored above.
            cave.transform.Translate(moveDirection * (movementSpeed * Time.fixedDeltaTime));
            cave.transform.Rotate(rotateDirection * (rotationSpeed * Time.fixedDeltaTime));
        }

        /// <summary>
        ///     Uses the Vector3 direction allocated by the input action to move the CAVE.
        /// </summary>
        /// <param name = "context"> </param>
        public void HandleMovementInput(InputAction.CallbackContext context)
        {
            moveDirection = context.ReadValue<Vector3>(); //Stores the direction passed from the input system bindings
        }

        /// <summary>
        ///     Uses the Vector2 direction allocated by the input action to rotate the CAVE.
        /// </summary>
        /// <param name = "context"> </param>
        public void HandleRotationInput(InputAction.CallbackContext context)
        {
            rotateDirection = context.ReadValue<Vector2>(); //Stores the direction passed from the input system bindings
        }
    }
}