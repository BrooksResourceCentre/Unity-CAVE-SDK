using UnityEngine;

/// <summary>
///     Helper utility functions for Cave automatic virtual environment (CAVE) applications
/// </summary>
public static class CAVEUtilities
{
    public enum SwipeTypes
    {
        /// <summary> No action. </summary>
        None,

        /// <summary> Drag along CAVE walls to rotate the camera view. </summary>
        Look,
        
        /// <summary> Call a Unity Event. </summary>
        CallEvent
    }

    /// <summary>
    ///     Defined ways the CAVE can respond to touch interactions.
    /// </summary>
    public enum TouchTypes
    {
        /// <summary> No action. </summary>
        None,

        /// <summary> Teleport the CAVE to the touch point. </summary>
        Teleport,

        /// <summary> Spawn a random object at the touch point. </summary>
        SpawnObject,
        
        /// <summary> Call a Unity Event. </summary>
        CallEvent
    }

    /// <summary>
    ///     Get the camera that contains the mouse position,
    ///     used to determine which camera the mouse is over.
    /// </summary>
    /// <param name = "pos"> </param>
    /// <param name = "cameras"> </param>
    /// <returns> </returns>
    public static Camera GetCameraFromScreenPosition(Vector3 pos, Camera[] cameras)
    {
        float screenWidth = Screen.width;

        for (var i = 0; i < cameras.Length; i++)
        {
            // Check each camera on the CAVE
            var cam          = cameras[i];
            var viewportRect = cam.rect;

            // Calculate the screen rect for the camera
            var screenRect = new Rect(viewportRect.x * screenWidth, viewportRect.y * Screen.height,
                viewportRect.width * screenWidth, viewportRect.height * Screen.height);

            if (screenRect.Contains(pos))
            {
                return cam; // If the position passed in is within that camera, return a reference to it
            }
        }

        // If no camera contains the mouse position, return null
        return null;
    }


    /// <summary>
    ///     Raycasts from the given position to the corresponding world point,
    ///     uses the correct CAVE camera for accurate results and returns the raycast hit.
    /// </summary>
    public static RaycastHit RaycastFromScreenPosition(Vector2 pos, Camera[] cameras)
    {
        var camera = GetCameraFromScreenPosition(pos, cameras);

        if (!camera)
        {
            return new RaycastHit();
        }

        var ray = camera.ScreenPointToRay(pos);

        if (Physics.Raycast(ray, out var hit))
        {
            return hit;
        }

        return new RaycastHit();
    }
}