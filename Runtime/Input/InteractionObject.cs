using UnityEngine;

namespace MMUCAVE
{
    /// <summary>
    ///     All classes that inherit from InteractionObject respond to being tapped through implementation of OnTouch.
    ///     (Requires a collider on the object)
    /// </summary>
    public abstract class InteractionObject : MonoBehaviour
    {
        /// <summary>
        ///     Override this to inform what happens when this object is tapped.
        /// </summary>
        /// <example>
        ///     <code>
        /// public override void OnTouch()
        /// {
        ///    CAVE.transform.position = transform.position;
        ///    CAVE.transform.rotation = Quaternion.Euler(rotationOffset);
        /// }
        /// </code>
        /// </example>
        public abstract void OnTouch();

        public abstract void OnSwipe(Vector3 direction);
        public abstract void OnHold();
    }
}