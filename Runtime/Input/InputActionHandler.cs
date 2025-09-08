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
    public class InputActionHandler : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Reference to the CAVE's Input Manager")]
        [SerializeField]
        private CAVEInputManager caveInputManager;

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

        private readonly Coroutine[]
            _coroutines = new Coroutine[maxInputCount]; // Holds references to the TouchUpdate coroutines

        private readonly Vector2[]
            _directions = new Vector2[maxInputCount]; // Stores the direction of each touch if it is a swipe

        private readonly Vector2[]
            _lastPositions = new Vector2[maxInputCount]; // Stores the position of the touch at last coroutine pass

        private readonly float[] _startTimes = new float[maxInputCount]; // Stores the time each touch was started

        private readonly bool[]
            didSwipe = new bool[maxInputCount]; // Used to note difference between hold and swipe inputs

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
            // Stores all the needed data for the new touch in the relevant arrays
            _lastPositions[finger.index] = finger.currentTouch.screenPosition;
            _startTimes[finger.index] = Time.time;
            _directions[finger.index] = Vector2.zero;
            _coroutines[finger.index] = StartCoroutine(TouchUpdate(finger)); // Starts checking for changes to the input
        }

        private void FingerUp(Finger finger)
        {
            // Clears all data for the unneeded touch
            StopCoroutine(_coroutines[finger.index]); // Stops checking for input changes

            if (!didSwipe[finger.index]) // Checks if a swipe was already performed.
            {
                if (Mathf.Abs(_startTimes[finger.index] - Time.time) <
                    tapTimeThreshold) // Compare start and current time
                {
                    // If the time passed is less than what is required for a hold then respond to a tap
                    caveInputManager.HandleTapActions(finger.currentTouch.screenPosition);
                }
                else
                {
                    caveInputManager.HandleHoldActions(finger.currentTouch.screenPosition);
                }
            }

            didSwipe[finger.index] = false; // Reset value for next touch
        }

    #region Swipe Detection

        private IEnumerator TouchUpdate(Finger finger) // Checks for changes to the touch input
        {
            while (true)
            {
                _directions[finger.index] = Vector2.zero; // Reset stored direction from last pass
                DetectSwipe(finger);                      // Check for a swipe

                _lastPositions[finger.index] =
                    finger.currentTouch.screenPosition; // Update stored position for next pass

                yield return new WaitForSeconds(inputUpdateWait); // Wait for the required time before next pass
            }
        }

        private void DetectSwipe(Finger finger)
        {
            // Touch is a swipe if it has moved at least the minimum distance since the last pass
            if (Vector2.Distance(_lastPositions[finger.index], finger.currentTouch.screenPosition) >= minimumDistance)
            {
                didSwipe[finger.index] = true;

                _directions[finger.index] =
                    (finger.currentTouch.screenPosition - _lastPositions[finger.index]).normalized;

                // vv THE INTERACTION A SWIPE CORRESPONDS TO vv
                SwipeDirection(_directions[finger.index], finger.currentTouch.screenPosition);
                // In this example it rotates the view
            }
            else if (Mathf.Abs(_startTimes[finger.index] - Time.time) >
                     tapTimeThreshold) // Compare start and current time
            {
                // vv THE INTERACTION A NON-SWIPE CORRESPONDS TO vv
                // If the time passed is more than what is required for a hold then respond to a hold
                caveInputManager.HandleHoldActions(finger.currentTouch.screenPosition);
            }
        }

        private void SwipeDirection(Vector2 direction, Vector2 position)
        {
            // Uses the dot product to determine how similar the touch direction is to each cardinal direction.
            if (Vector2.Dot(Vector2.right, direction) > directionThreshold)
            {
                caveInputManager.HandleSwipeActions(position, Vector3.down); // If swiped right, rotate right
            }

            if (Vector2.Dot(Vector2.left, direction) > directionThreshold)
            {
                caveInputManager.HandleSwipeActions(position, Vector3.up); // If swiped left, rotate left
            }
        }

    #endregion
    }
}