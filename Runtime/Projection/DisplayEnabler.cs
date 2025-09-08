using UnityEngine;

/// <summary>
///     Manually enable all required screens for accurate CAVE projection
/// </summary>
public class DisplayEnabler : MonoBehaviour
{
    [Tooltip("Number of displays connected to the system (-1 for indexing)")]
    [SerializeField]
    private int displayIndex;

    private void OnEnable()
    {
#if !UNITY_EDITOR
            if (displayIndex >= Display.displays.Length)
            {
                Debug.LogError("Display index out of range");
                return; // If more displays are given than are found, log this as a warning
            }
            
            Display.displays[displayIndex].Activate();// Activate all possible displays
#endif
    }
}