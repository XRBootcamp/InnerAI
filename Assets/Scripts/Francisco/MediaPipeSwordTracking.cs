using System.Collections;
using Meta.XR;
using PassthroughCameraSamples;
using UnityEngine;

public class MediaPipeSwordTracking : MonoBehaviour
{
    [SerializeField] private GameObject swordPrefab;
    [SerializeField] private WebCamTextureManager webCamTextureManager;
    [SerializeField] private EnvironmentRaycastManager envRaycastManager;

    private bool isActive = false;
    private GameObject sword;
    private const float YoloInputSize = 640f;
    private PassthroughCameraIntrinsics intrinsics;
    private Vector2Int camRes;
    private float halfWidth;
    private float halfHeight;
    private float imageWidth;
    private float imageHeight;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    IEnumerator Start()
    {
        yield return new WaitUntil(() => WebCamTextureManager.WebCamTexture != null);
        isActive = true;

    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive) return;
        if (!MediaPipeBodyController.Instance.MediaPipeTransformPointsParent)
        {
            // if (sword)
            // {
            //     Destroy(sword); // Destroy the sword if MediaPipeTransformPointsParent is not set
            //     sword = null;
            // }

            return;
        }

        intrinsics = PassthroughCameraUtils.GetCameraIntrinsics(webCamTextureManager.Eye);
        camRes = intrinsics.Resolution;

        imageWidth = 1280;
        imageHeight = 960;
        halfWidth = imageWidth * 0.5f;
        halfHeight = imageHeight * 0.5f;
        // Check if the sword is already instantiated
        if (!sword)
        {
            // Instantiate the sword at the right hand position
            sword = Instantiate(swordPrefab);
        }

        var detectedCenterX = -MediaPipeBodyController.Instance.MediaPipeTransformPointsParent.GetChild(15).localPosition.x; // Right hand X
        var detectedCenterY = -MediaPipeBodyController.Instance.MediaPipeTransformPointsParent.GetChild(15).localPosition.y;
        var adjustedCenterX = detectedCenterX - halfWidth;
        var adjustedCenterY = detectedCenterY - halfHeight;
        var perX = (adjustedCenterX + halfWidth) / imageWidth;
        var perY = (adjustedCenterY + halfHeight) / imageHeight;

        var centerPixel = new Vector2(perX * camRes.x, (1.0f - perY) * camRes.y);
        var centerRay = PassthroughCameraUtils.ScreenPointToRayInWorld(webCamTextureManager.Eye,
            new Vector2Int(Mathf.RoundToInt(centerPixel.x), Mathf.RoundToInt(centerPixel.y)));
        // var centerRay = PassthroughCameraUtils.ScreenPointToRayInWorld(webCamTextureManager.Eye,
        //     new Vector2Int(Mathf.RoundToInt(detectedCenterX),Mathf.RoundToInt(detectedCenterY)));

        if (envRaycastManager.Raycast(centerRay, out var centerHit))
        {
            var markerWorldPos = centerHit.point;
            if (sword)
            {
                // Update the sword position to the raycast hit point
                sword.transform.position = markerWorldPos;
                sword.transform.rotation = Quaternion.LookRotation(centerHit.normal, Vector3.up);
            }
        }
    }
}