using UnityEngine;

namespace MMUCAVE
{
    public abstract class InteractionObject : MonoBehaviour
    {
        /// <summary>
        /// All Interaction Objects must implement OnTouch as its primary function
        /// Requires a collider on the object to call OnTouch
        /// </summary>
        public abstract void OnTouch();
    }
}
