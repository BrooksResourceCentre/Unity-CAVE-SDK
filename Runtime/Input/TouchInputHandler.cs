using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace MMUCAVE
{
    /// <summary>
    ///     Handles all touch inputs to the CAVE and reacts dependent on type of input received.
    ///     <br> Provides adjustment variables to hone to your CAVE setup. </br>
    /// </summary>
    public class TouchInputHandler : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Reference to the CAVE's Input Manager")]
        [SerializeField]
        private CAVEInteractionManager caveInteractionManager;

        [Header("Threshold Adjustments")]
        [Tooltip("The maximum inputs your display can handle")]
        private static readonly int maxInputCount = 10;
        
        [Tooltip("The minimum distance a touch must move to register as a swipe")]
        [SerializeField]
        private float minimumDistance = 100f;

        [Tooltip("How frequently to check for changes in the input, lower = more frequent")]
        [SerializeField]
        private float inputUpdateWait = 0.1f;

        [Tooltip("The percentage of similarity a swipe must have to register a cardinal direction")]
        [SerializeField]
        private float directionThreshold = .9f;

        [Tooltip("The minimum time that must pass before a tap becomes a hold")]
        [SerializeField]
        private float tapTimeThreshold = 0.6f;

        private readonly TouchInstance[] touches = new TouchInstance[maxInputCount]; // Holds references to the TouchInstances

        private void OnEnable()
        {
            EnhancedTouchSupport.Enable();    // Enables the enhanced touch system for our use
            Touch.onFingerDown += FingerDown; // vv
            Touch.onFingerUp   += FingerUp;   // Subscribes our functions to the touch system events
        }

        private void OnDisable()
        {
            EnhancedTouchSupport.Disable();   // Disables the enhanced touch system when unused
            Touch.onFingerDown -= FingerDown; // vv
            Touch.onFingerUp   -= FingerUp;   // Unsubscribes from the touch system events
        }


        private void FingerDown(Finger finger)
        {
            touches[finger.index] = new TouchInstance(Vector2.zero,
                finger.currentTouch.screenPosition, Time.time);
            // Stores all the needed data for the new touch in a new TouchInstance
            touches[finger.index].coroutine = StartCoroutine(TouchUpdate(finger));
            //coroutine is assigned afterward to avoid starting a coroutine before other values are assigned
        }

        private void FingerUp(Finger finger)
        {
            // Clears all data for the unneeded touch
            StopCoroutine(touches[finger.index].coroutine); // Stops checking for input changes

            if (!touches[finger.index].didSwipe) // Checks if a swipe was already performed.
            {
                if (Mathf.Abs(touches[finger.index].startTime - Time.time) <
                    tapTimeThreshold) // Compare start and current time
                {
                    // If the time passed is less than what is required for a hold then respond to a tap
                    caveInteractionManager.HandleTapActions(finger.currentTouch.screenPosition);
                }
                else
                {
                    caveInteractionManager.HandleHoldActions(finger.currentTouch.screenPosition);
                }
            }
        }

    #region Swipe Detection

        private IEnumerator TouchUpdate(Finger finger) // Checks for changes to the touch input
        {
            while (true)
            {
                touches[finger.index].direction = Vector2.zero; // Reset stored direction from last pass
                DetectSwipe(finger);                      // Check for a swipe

                touches[finger.index].lastPosition =
                    finger.currentTouch.screenPosition; // Update stored position for next pass

                yield return new WaitForSeconds(inputUpdateWait); // Wait for the required time before next pass
            }
        }

        private void DetectSwipe(Finger finger)
        {
            // Touch is a swipe if it has moved at least the minimum distance since the last pass
            if (Vector2.Distance(touches[finger.index].lastPosition, finger.currentTouch.screenPosition) >= minimumDistance)
            {
                touches[finger.index].didSwipe = true;

                touches[finger.index].direction = (finger.currentTouch.screenPosition - touches[finger.index].lastPosition).normalized;

                // vv THE INTERACTION A SWIPE CORRESPONDS TO vv
                SwipeDirection(touches[finger.index].direction, finger.currentTouch.screenPosition);
                // In this example it rotates the view
            }
            else if (Mathf.Abs(touches[finger.index].startTime - Time.time) >
                     tapTimeThreshold) // Compare start and current time
            {
                // vv THE INTERACTION A NON-SWIPE CORRESPONDS TO vv
                // If the time passed is more than what is required for a hold then respond to a hold
                caveInteractionManager.HandleHoldActions(finger.currentTouch.screenPosition);
            }
        }

        private void SwipeDirection(Vector2 direction, Vector2 position)
        {
            // Uses the dot product to determine how similar the touch direction is to each cardinal direction.
            if (Vector2.Dot(Vector2.right, direction) > directionThreshold)
            {
                caveInteractionManager.HandleSwipeActions(position, Vector2.down); // If swiped right, rotate right
            }

            if (Vector2.Dot(Vector2.left, direction) > directionThreshold)
            {
                caveInteractionManager.HandleSwipeActions(position, Vector2.up); // If swiped left, rotate left
            }
        }

    #endregion
    }
}