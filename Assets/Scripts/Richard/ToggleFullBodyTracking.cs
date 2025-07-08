using System;
using PassthroughCameraSamples.CameraToWorld;
using UnityEngine;
using UnityEngine.UI;

public class ToggleFullBodyTracking : MonoBehaviour
{
    public Image cameraBackground;
    public RawImage cameraRender;
    // public RawImage cameraRender2;
    public CameraToWorldCameraCanvas cameraToWorldCameraCanvas;
    public CameraToWorldManager2 cameraToWorldManager2;
    
    private void Update()
    {
        // if(Input.GetKeyDown(KeyCode.Space))
        if (OVRInput.GetDown(OVRInput.Button.Three))
        {
            cameraBackground.enabled = !cameraBackground.enabled;
            cameraRender.enabled = !cameraRender.enabled;
            cameraToWorldCameraCanvas?.ToggleDebugText();
            cameraToWorldManager2?.ToggleCameraMarkers();
        }
        else if (OVRInput.GetDown(OVRInput.Button.Four))
        {
            // cameraRender2.enabled = !cameraRender2.enabled;
        }
    }
}
