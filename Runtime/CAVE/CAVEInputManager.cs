using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;//Enables necessary touch input functions
using Random = UnityEngine.Random;

namespace MMUCAVE
{
    public class CAVEInputManager : MonoBehaviour
    {
		/// <summary>
		/// Manages the in-world reactions to any input given by the input handler class.
		/// </summary>

        [Header("References")] [Tooltip("Reference to the cameras in the CAVE")] [SerializeField]
        private Camera[] cameras; //All cameras

        [Tooltip("The touch actions available to perform")]
        private int keybindsPressedCounter = 0;


        [SerializeField] private float rotationSpeed = 100f; // Speed of rotation

        [Tooltip("Tooltip panel for keybind popups.")] [SerializeField]
        private GameObject keybindPanel;

        [Tooltip("The speed at which the CAVE will rotate when swiping")][SerializeField] private float rotationSpeed = 100f; // Speed of rotation
     
        [Tooltip("Reference to the CAVE game object")] [SerializeField]
        private GameObject cave; // Reference to the CAVE


        #region Swipe Inputs
		/// Could be combined into a generic rotate direction function? ///
        public void RotateCAVERight()
        {
            cave.transform.Rotate(Vector3.down, rotationSpeed); // Rotate right
        }
        public void RotateCAVELeft()
        {
            cave.transform.Rotate(Vector3.up, rotationSpeed); // Rotate left
        }
        

        #endregion
        
        
        #region Touch Actions

        public void HandleTouchActions(Vector2 position, CAVEUtilities.TouchTypes type)
        {

		/// <summary>
		/// Generic input handler, takes in a position and touch type, outputs relevant function call
		/// </summary>

            RaycastHit raycastHit = CAVEUtilities.RaycastFromScreenPosition(position, cameras);
			// Gets reference to the object at the touched position
            if (!raycastHit.collider)
            {
                return; // If there is no object, return nothing
            }
            
            // Switch selected TouchType Input
            switch (type)
            {
                case CAVEUtilities.TouchTypes.Teleport:
                    MoveCaveToClickPosition(raycastHit);
                    break; //Moves CAVE to touch coordinates.

                case CAVEUtilities.TouchTypes.SpawnObject:
                    InstantiateRandomPrimitiveAtClickPosition(raycastHit);
                    break; //Spawn random object at touch coordinates, temporary for demonstration.

                case CAVEUtilities.TouchTypes.Touchables:
                    InteractWithTouchables(raycastHit);
                    break; //Prompt a function on any object that contains the touchable class.

                case CAVEUtilities.TouchTypes.ShootProjectile:
                    InstantiateProjectile(raycastHit);
                    break; //Shoot random object towards touch coordinates, temporary for demonstration.

                default:
                    Debug.LogWarning("No action selected or action not implemented.");
                    break; //Logs an error if a correct touch type is not given
            }
        }

        private void MoveCaveToClickPosition(RaycastHit hit)
        {
            cave.transform.position = hit.point; // Move the CAVE to the hit point
        }

        private void InteractWithTouchables(RaycastHit hit)
        {
            // Check if the object has a Touchable component.
            InteractionObject touchable = hit.collider.GetComponent<InteractionObject>();
            if (touchable)
            {
                touchable.OnTouch(); // Call the OnTouch method on the Touchable component
            }
        }

		#region Demo behaviour

        // Spawns a random primitive at the hit point
        private void InstantiateRandomPrimitiveAtClickPosition(RaycastHit hit)
        {
            // List of available primitive types
            PrimitiveType[] primitiveTypes = new PrimitiveType[]
            {
                PrimitiveType.Cube,
                PrimitiveType.Sphere,
                PrimitiveType.Capsule,
                PrimitiveType.Cylinder,
            };

            // Randomly select a primitive type
            PrimitiveType randomType = primitiveTypes[Random.Range(0, primitiveTypes.Length)];

            // Create the primitive and offset
            GameObject primitive = GameObject.CreatePrimitive(randomType);
            float scale = Random.Range(0.2f, 0.5f); // Random scale for the primitive
            primitive.transform.localScale = Vector3.one * scale; // Scale the primitive
            primitive.transform.position = hit.point + hit.normal * scale; // Offset from the surface

            // Add a Rigidbody for physics interactions
            primitive.AddComponent<Rigidbody>();
        }

        // Create a projectile and fire it from the CAVE towards the hit point
        private void InstantiateProjectile(RaycastHit hit)
        {
            //List of available primitive types for the projectile
            PrimitiveType[] primitiveTypes = new PrimitiveType[]
            {
                PrimitiveType.Cube,
                PrimitiveType.Sphere,
                PrimitiveType.Capsule,
                PrimitiveType.Cylinder,
            };

            // Randomly select a primitive type
            PrimitiveType randomType = primitiveTypes[Random.Range(0, primitiveTypes.Length)];

            // Create the projectile GameObject
            GameObject projectile = GameObject.CreatePrimitive(randomType);
            projectile.transform.localScale = Vector3.one * 0.1f; // Scale down the projectile for better visibility
            projectile.transform.position = cave.transform.position; // Start position is the CAVE's position
            Rigidbody rb = projectile.AddComponent<Rigidbody>(); // Add a Rigidbody to the projectile
            rb.collisionDetectionMode =
                CollisionDetectionMode.ContinuousDynamic; // Set collision detection mode for better accuracy
            
            Vector3 direction =
                (hit.point - cave.transform.position).normalized; // Calculate direction to the hit point
            rb.linearVelocity = direction * 20f; // Set linear velocity of the projectile
      
     	  }
		#endregion
       #endregion
    }
}

