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

        [Header("Input Options")]
        [Tooltip("How the CAVE responds to tap inputs when not touching an Interaction Object")] [SerializeField]
        private CAVEUtilities.TapTypes tapType =  CAVEUtilities.TapTypes.None;
        [Tooltip("How the CAVE responds to hold inputs when not touching an Interaction Object")][SerializeField]
        private CAVEUtilities.HoldTypes holdType =   CAVEUtilities.HoldTypes.None;
        [Tooltip("How the CAVE responds to swipe inputs when not touching an Interaction Object")][SerializeField]
        private CAVEUtilities.SwipeTypes swipeType =    CAVEUtilities.SwipeTypes.None;
        [Tooltip("Whether Interaction Objects will respond to input")][SerializeField]
        private bool handleInteractionObjects = true;

        #region Swipe Input

        /// <summary>
        /// If object is swiped and enabled, calls its OnSwipe function, otherwise handles as specified SwipeType
        /// </summary>
        /// <param name="position"></param>
        /// <param name="direction"></param
        public void HandleSwipeActions(Vector2 position, Vector3 direction)
        {
            RaycastHit raycastHit = CAVEUtilities.RaycastFromScreenPosition(position, cameras);
            InteractionObject touchable = (raycastHit.collider) ? raycastHit.collider.GetComponent<InteractionObject>() :  null;
            // Get a reference to the raycast hit object if it inherits from the InteractionObject class
            if (touchable)
            {
                touchable.OnSwipe(direction); // Call the OnSwipe method on the object
            }
            else
            {
                // If there is no object, or it is not an Interaction Object, act on SwipeType
                switch (swipeType)
                {
                    case CAVEUtilities.SwipeTypes.None: // Do nothing
                        return; 
                    case CAVEUtilities.SwipeTypes.Look:
                        cave.transform.Rotate(direction, rotationSpeed); // Rotates the CAVE view in the direction specified
                        break;
                    default:
                        Debug.LogWarning("No action selected or action not implemented.");
                        break; //Logs an error if a correct touch type is not given
                }
            }
        }

        #endregion
        
        
        #region Touch Actions
        /// <summary>
        /// If object is tapped and enabled, calls its OnTap function, otherwise handles as specified TapType
        /// </summary>
        /// <param name="position"></param>
        public void HandleTapActions(Vector2 position)
        {
            
            RaycastHit raycastHit = CAVEUtilities.RaycastFromScreenPosition(position, cameras);
            InteractionObject touchable = (raycastHit.collider) ? raycastHit.collider.GetComponent<InteractionObject>() :  null;
            // Get a reference to the raycast hit object if it inherits from the InteractionObject class
            if (touchable)
            {
                touchable.OnTouch(); // Call the OnTouch method on the object
            }
            else
            {
                // If there is no object, or it is not an Interaction Object, act on TapType
                switch (tapType)
                {
                    case CAVEUtilities.TapTypes.None:
                        return; // Do nothing
                    
                    case CAVEUtilities.TapTypes.Teleport:
                        MoveCaveToHitPosition(raycastHit);
                        break; //Moves CAVE to touch coordinates.

                    case CAVEUtilities.TapTypes.SpawnObject:
                        InstantiateRandomPrimitiveAtHitPosition(raycastHit);
                        break; //Spawn random object at touch coordinates, only for demonstration.

                    case CAVEUtilities.TapTypes.ShootProjectile:
                        InstantiateProjectile(raycastHit);
                        break; //Shoot random object towards touch coordinates, only for demonstration.

                    default:
                        Debug.LogWarning("No action selected or action not implemented.");
                        break; //Logs an error if a correct touch type is not given
                }
            }
        }
        
        /// <summary>
        /// If object is held and enabled, calls its OnHold function, otherwise handles as specified HoldType
        /// </summary>
        /// <param name="position"></param>
        public void HandleHoldActions(Vector2 position)
        {
            RaycastHit raycastHit = CAVEUtilities.RaycastFromScreenPosition(position, cameras);
            InteractionObject touchable = (raycastHit.collider) ? raycastHit.collider.GetComponent<InteractionObject>() :  null;
            // Get a reference to the raycast hit object if it inherits from the InteractionObject class
            if (touchable)
            {
                touchable.OnHold(); // Call the OnHold method on the object
            }
            else
            {
                // If there is no object, or it is not an Interaction Object, act on HoldType
                switch (holdType)
                {
                    case CAVEUtilities.HoldTypes.None:
                        return; // Do nothing
                    
                    case CAVEUtilities.HoldTypes.Teleport:
                        MoveCaveToHitPosition(raycastHit);
                        break; //Moves CAVE to touch coordinates.

                    case CAVEUtilities.HoldTypes.SpawnObject:
                        InstantiateRandomPrimitiveAtHitPosition(raycastHit);
                        break; //Spawn random object at touch coordinates, only for demonstration.

                    case CAVEUtilities.HoldTypes.ShootProjectile:
                        InstantiateProjectile(raycastHit);
                        break; //Shoot random object towards touch coordinates, only for demonstration.

                    default:
                        Debug.LogWarning("No action selected or action not implemented.");
                        break; //Logs an error if a correct touch type is not given
                }
            }
        }

        private void MoveCaveToHitPosition(RaycastHit hit)
        {
            cave.transform.position = hit.point; // Move the CAVE to the hit point
        }

		#region Demo behaviour

        // Spawns a random primitive at the hit point
        private void InstantiateRandomPrimitiveAtHitPosition(RaycastHit hit)
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

