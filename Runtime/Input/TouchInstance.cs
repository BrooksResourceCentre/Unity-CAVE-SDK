using UnityEngine;

namespace MMUCAVE
{
    public class TouchInstance
    {
        public Coroutine coroutine;        // Holds references to the TouchUpdate coroutine
        public bool      didSwipe = false; // Used to note difference between hold and swipe input 
        public Vector2   direction;        // Stores the direction of the touch if it is a swipe
        public Vector2   lastPosition;     // Stores the position of the touch at last coroutine pass
        public float     startTime;        // Stores the time the touch was started

        public TouchInstance(Vector2 _direction, Vector2 _lastPosition, float _startTime)
        {
            direction    = _direction;
            lastPosition = _lastPosition;
            startTime    = _startTime;
        }
    }
}