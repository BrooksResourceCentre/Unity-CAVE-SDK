using UnityEngine;

namespace MMUCAVE
{
    public class CAVEEmulatorManager : MonoBehaviour
    {
        // TODO: add tooltips and document
        [SerializeField] private bool enableEmulator;
        [SerializeField] private GameObject cave;
        [SerializeField] private Camera[] caveCameras; 
        [SerializeField] private GameObject screenPrefab;
        [SerializeField] private GameObject controllerPrefab;
        void Start()
        {
            #if UNITY_EDITOR
            if (enableEmulator)
            {
                GenerateScreens();
            }
            #endif
        }

        void GenerateScreens()
        {
            foreach (Camera camera in caveCameras)
            {
                GameObject screen = Instantiate(screenPrefab, camera.transform.parent.gameObject.transform, false);
                screen.name = $"Output of {camera.name}";
                
                // correct viewport rect of camera
                camera.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
                
                // create render texture and set camera output to this render texture
                RenderTexture cameraOutput = new RenderTexture(Screen.width, Screen.height, 24); // TODO: see what gives best performance
                camera.targetTexture = cameraOutput;
                
                // assign texture to material
                screen.GetComponent<Renderer>().material.mainTexture = cameraOutput;
                
                // set size of screen
                screen.transform.localScale = camera.name == "Projection Camera Floor" ? new Vector3(5f, 5f, 1f) : new Vector3(5f, 2.8f, 1f);
                screen.transform.localPosition = new Vector3(0f, 0f, -0.01f); // just moving the plane ever so slightly back so it gets culled by the camera
            }
           
            // create our controller prefab
            Instantiate(controllerPrefab, cave.transform);
        }
    }
}
