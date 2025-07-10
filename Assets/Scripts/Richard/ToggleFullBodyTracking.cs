using System;
using Mediapipe;
using PassthroughCameraSamples.CameraToWorld;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class ToggleFullBodyTracking : MonoBehaviour
{
    public Image cameraBackground;
    public RawImage cameraRender;
    // public RawImage cameraRender2;
    public CameraToWorldCameraCanvas cameraToWorldCameraCanvas;
    public CameraToWorldManager2 cameraToWorldManager2;
    public GameObject mediaPipePoints;
    
    public CameraModes currentCameraMode = CameraModes.CameraAndFrustum;
    public ControllerModes currentControllerMode = ControllerModes.Quest3InHand;

    public GameObject Quest3Controller;
    public DelayedObjectFollowWithOffset QuestProHandle;

    private float lagTimeStart = 0.05f;
    private float lagTimeIncSpeed = 0.125f;
    
    public enum CameraModes
    {
        CameraAndFrustum = 0,
        JustCamera = 1,
        CameraAndFrustumOff = 2,
    }

    public enum ControllerModes
    {
        Quest3InHand = 0,
        QuestProHandle = 1,
        QuestProHandleIncSpeed = 2
    }
    
    private void Update()
    {
        // if(Input.GetKeyDown(KeyCode.Space))
        if (OVRInput.GetDown(OVRInput.Button.Three))
        {
            if (currentCameraMode == CameraModes.CameraAndFrustum)
            {
                currentCameraMode = CameraModes.JustCamera;
                
                cameraBackground.enabled = false;
                cameraRender.enabled = false;
                cameraToWorldCameraCanvas?.TurnOnOffDebugText(false);
                cameraToWorldManager2?.ToggleOnOffCameraMarkers(false);
                mediaPipePoints.SetActive(true);
            }
            else if (currentCameraMode == CameraModes.JustCamera)
            {
                currentCameraMode = CameraModes.CameraAndFrustumOff;
                
                cameraBackground.enabled = false;
                cameraRender.enabled = false;
                cameraToWorldCameraCanvas?.TurnOnOffDebugText(false);
                cameraToWorldManager2?.ToggleOnOffCameraMarkers(false);
                mediaPipePoints.SetActive(false);
            }
            else if (currentCameraMode == CameraModes.CameraAndFrustumOff)
            {
                currentCameraMode = CameraModes.CameraAndFrustum;
                
                cameraBackground.enabled = true;
                cameraRender.enabled = true;
                cameraToWorldCameraCanvas?.TurnOnOffDebugText(true);
                cameraToWorldManager2?.ToggleOnOffCameraMarkers(true);
                mediaPipePoints.SetActive(true);
            }
            
            // cameraBackground.enabled = !cameraBackground.enabled;
            // cameraRender.enabled = !cameraRender.enabled;
            // cameraToWorldCameraCanvas?.ToggleDebugText();
            // cameraToWorldManager2?.ToggleCameraMarkers();
        }
        else if (OVRInput.GetDown(OVRInput.Button.Four))
        {
            // cameraRender2.enabled = !cameraRender2.enabled;
        }
        else if (OVRInput.GetDown(OVRInput.Button.One))
        {
            if (currentControllerMode == ControllerModes.Quest3InHand)
            {
                currentControllerMode = ControllerModes.QuestProHandle;
                Quest3Controller.SetActive(false);
                QuestProHandle.gameObject.SetActive(true);
                QuestProHandle.SetLagTime(lagTimeStart);
            }
            else if (currentControllerMode == ControllerModes.QuestProHandle)
            {
                currentControllerMode = ControllerModes.QuestProHandleIncSpeed;
                Quest3Controller.SetActive(false);
                QuestProHandle.gameObject.SetActive(true);
                QuestProHandle.SetLagTime(lagTimeIncSpeed);
            }
            else if (currentControllerMode == ControllerModes.QuestProHandleIncSpeed)
            {
                currentControllerMode = ControllerModes.Quest3InHand;
                Quest3Controller.SetActive(true);
                QuestProHandle.gameObject.SetActive(false);
            }
        }
    }
}
