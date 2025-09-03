using UnityEngine;

namespace MMUCAVE
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Camera camera;

        [SerializeField] private float cameraSensitivity = 2.0f;

        void Start()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        void Update()
        {
            float pitch = cameraSensitivity * -Input.GetAxis("Mouse Y");
            float yaw = cameraSensitivity * Input.GetAxis("Mouse X");
            
            Quaternion rotation = camera.transform.localRotation;
            rotation.eulerAngles += new Vector3(pitch, yaw, 0.0f);
            camera.transform.localRotation = rotation;
        }
    }
}
