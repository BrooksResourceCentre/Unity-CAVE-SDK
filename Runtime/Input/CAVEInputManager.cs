using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace MMUCAVE
{
    /// <summary> 
    /// Manages the in-world reactions to any input given by the input handler class.
 	/// </summary>
    public class CAVEInputManager : MonoBehaviour
    {
        [Tooltip("Reference to the cameras in the CAVE")] [SerializeField]
        private Camera[] cameras;

        [Tooltip("The speed at which the CAVE will rotate when swiping")][SerializeField]
        private float rotationSpeed = 100f;
     
        [Tooltip("Reference to the CAVE game object")] [SerializeField]
        private GameObject cave;


        #region Swipe Input

        /// <summary>
        /// Rotates the CAVE view in the direction specified.<br><ul> (Up = Left, Down = Right) </ul></br>
        /// </summary>
        /// <param name="position"></param>
        /// <param name="direction"></param
        public void HandleSwipeActions(Vector2 position, Vector3 direction)
        {
            RaycastHit raycastHit = CAVEUtilities.RaycastFromScreenPosition(position, cameras);
            if (!raycastHit.collider)
            {
                return; // If there is no object, return nothing
            }
            // Get a reference to the raycast hit object if it inherits from the InteractionObject class
            InteractionObject touchable = raycastHit.collider.GetComponent<InteractionObject>();
            if (touchable)
            {
                touchable.OnSwipe(direction); // Call the OnSwipe method on the Touchable component
            }
            else
            {
                cave.transform.Rotate(direction, rotationSpeed); // if no touchable object was hit, Rotate view
            }
        }

        #endregion
        
        
        #region Touch Actions
        /// <summary>
        /// Calls relevant function associated with passed Input Type
        /// </summary>
        /// <param name="position"></param>
        /// <param name="type"></param>
        public void HandleTouchActions(Vector2 position, CAVEUtilities.TouchTypes type)
        {
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
                    InteractionObject touchable = raycastHit.collider.GetComponent<InteractionObject>();
                    if (touchable)
                    {
                        touchable.OnTouch(); // Call the OnTouch method on the Touchable component
                    }
                    break; //Prompt a function on any object that contains the touchable class.

                case CAVEUtilities.TouchTypes.ShootProjectile:
                    InstantiateProjectile(raycastHit);
                    break; //Shoot random object towards touch coordinates, temporary for demonstration.

                default:
                    Debug.LogWarning("No action selected or action not implemented.");
                    break; //Logs an error if a correct touch type is not given
            }
        }
        
        public void HandleHoldActions(Vector2 position)
        {
            RaycastHit raycastHit = CAVEUtilities.RaycastFromScreenPosition(position, cameras);
            if (!raycastHit.collider)
            {
                return; // If there is no object, return nothing
            }
            // Get a reference to the raycast hit object if it inherits from the InteractionObject class
            InteractionObject touchable = raycastHit.collider.GetComponent<InteractionObject>();
            if (touchable)
            {
                touchable.OnHold(); // Call the OnSwipe method on the touchable component
            }
            else
            {
                return; // if no touchable object was hit, return
            }
        }

        private void MoveCaveToClickPosition(RaycastHit hit)
        {
            cave.transform.position = hit.point; // Move the CAVE to the hit point
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

