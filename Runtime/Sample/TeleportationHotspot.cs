using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using MMUCAVE;

public class TeleportationHotspot : InteractionObject
{
    [Tooltip("The GameObject to teleport")]
    [SerializeField] private GameObject objectToTeleport;

    

    [Tooltip("The position to teleport to relative to this GameObject")]
    [SerializeField] private Vector3 positionOffset = new Vector3(0, 1.425f, 0); // Default CAVE height

    [Tooltip("The rotation to teleport to relative to this GameObject")]
    [SerializeField] private Vector3 rotationOffset;



    [Header("Canvas Cameras")]

    [Tooltip("reference to the cameras that will be used to render the hotspot canvas")]
    [SerializeField] private Camera[] cameras;

    [Tooltip("reference to the hotspot canvas element")]
    private Canvas canvas;

    private Vector2 lastTouchPosition;


    void Start()
    {
        canvas = GetComponentInChildren<Canvas>();
        if (canvas == null)
            Debug.LogError("Canvas not found");

        // Start intercepting touch input to decide which camera to use
        //_ = SwitchCanvasCameraTouchIntercept();
    }

    /*void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastTouchPosition = Input.mousePosition;
            StartCoroutine(SwitchCameraAndSimulateTouch(lastTouchPosition));
        }
    }*/



    public override void OnTouch()
    {
        if (objectToTeleport == null)
        {
            Debug.LogError("Object to teleport is null");
            return;
        }

        objectToTeleport.transform.position = transform.position + positionOffset;
        objectToTeleport.transform.rotation = Quaternion.Euler(rotationOffset);
    }

#region Switch Canvas Camera Touch Intercept

public void SwitchCanvasCameraTouchIntercept()
{
    // Existing code to switch the camera
    int cameraIndex = (int)(Input.mousePosition.x / (Screen.width / cameras.Length));
    cameraIndex = Mathf.Clamp(cameraIndex, 0, cameras.Length - 1);
    canvas.worldCamera = cameras[cameraIndex];
}

#endregion

IEnumerator SwitchCameraAndSimulateTouch(Vector2 position)
{
    SwitchCanvasCameraTouchIntercept();
    yield return new WaitForEndOfFrame(); // Wait for the end of the frame to ensure the camera switch is complete
    SimulateTouch(position);
}

void SimulateTouch(Vector2 position)
{
    PointerEventData pointerData = new PointerEventData(EventSystem.current)
    {
        position = position
    };

    List<RaycastResult> results = new List<RaycastResult>();
    EventSystem.current.RaycastAll(pointerData, results);

    foreach (RaycastResult result in results)
    {
        ExecuteEvents.Execute(result.gameObject, pointerData, ExecuteEvents.pointerDownHandler);
        ExecuteEvents.Execute(result.gameObject, pointerData, ExecuteEvents.pointerUpHandler);
    }
}

#region  Switch Canvas Camera Rapidly
/*
    private async Task SwitchCanvasCameraRapidly()
    {
        while (gameObject.activeSelf)
        {
            if (cameras.Length == 0)
            {
                Debug.LogError("No cameras assigned");
                break;
            }

            foreach (var camera in cameras)
            {
                canvas.worldCamera = camera;
                await Task.Yield(); // Switch to the next camera in the same frame
            }
        }
    }

    private void StopSwitchingCameras()
    {
        // Logic to stop the camera switching if needed
    }
*/
#endregion
}
