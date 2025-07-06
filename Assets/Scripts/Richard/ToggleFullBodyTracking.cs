using System;
using UnityEngine;
using UnityEngine.UI;

public class ToggleFullBodyTracking : MonoBehaviour
{
    public Image cameraBackground;
    public RawImage cameraRender;
    public RawImage cameraRender2;
    
    private void Update()
    {
        // if(Input.GetKeyDown(KeyCode.Space))
        if (OVRInput.GetDown(OVRInput.Button.Three))
        {
            cameraBackground.enabled = !cameraBackground.enabled;
            cameraRender.enabled = !cameraRender.enabled;
        }
        else if (OVRInput.GetDown(OVRInput.Button.Four))
        {
            cameraRender2.enabled = !cameraRender2.enabled;
        }
    }
}
