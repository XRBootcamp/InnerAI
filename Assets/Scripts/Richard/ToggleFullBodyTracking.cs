using System;
using UnityEngine;
using UnityEngine.UI;

public class ToggleFullBodyTracking : MonoBehaviour
{
    public Image cameraBackground;
    public RawImage cameraRender;
    
    private void Update()
    {
        // if(Input.GetKeyDown(KeyCode.Space))
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            cameraBackground.enabled = !cameraBackground.enabled;
            cameraRender.enabled = !cameraRender.enabled;
        }
    }
}
