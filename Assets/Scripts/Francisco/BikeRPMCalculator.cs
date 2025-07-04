using System;
using UnityEngine;

public class BikeRPMCalculator : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text displayRPMText;
    [SerializeField] private float movementThreshold = 0.03f; // Min Y-change to register movement
    [SerializeField] private float minSpeedForRPM = 0.1f; // Min foot speed to count as pedaling
    [SerializeField] private int smoothingFrames = 5; // Number of frames for averaging jitter

    private float leftFootLastLowestY;
    private float rightFootLastLowestY;
    private int leftFootRevolutions;
    private int rightFootRevolutions;
    private float timer;
    private float rpm;

    private bool leftFootGoingUp;
    private bool rightFootGoingUp;

    // Buffers for smoothing
    private float[] leftFootBuffer;
    private float[] rightFootBuffer;
    private int bufferIndex;
    private void Start()
    {
        leftFootBuffer = new float[smoothingFrames];
        rightFootBuffer = new float[smoothingFrames];
    }
    void Update()
    {
        if (!MediaPipeBodyController.Instance.MediaPipeTransformPointsParent) return;
        Vector3 leftFootPos =
            MediaPipeBodyController.Instance.MediaPipeTransformPointsParent.GetChild(29).position; // Left foot
        Vector3 rightFootPos =
            MediaPipeBodyController.Instance.MediaPipeTransformPointsParent.GetChild(30).position; // Right foot
        
        // Smooth foot positions to reduce jitter
        float smoothedLeftY = SmoothValue(leftFootPos.y, leftFootBuffer, ref bufferIndex);
        float smoothedRightY = SmoothValue(rightFootPos.y, rightFootBuffer, ref bufferIndex);
        timer += Time.deltaTime;
        
        // Only check for revolutions if feet are moving significantly
        if (IsFootMoving(smoothedLeftY, leftFootLastLowestY))
        {
            CheckFootRevolution(ref smoothedLeftY, ref leftFootLastLowestY, ref leftFootRevolutions, ref leftFootGoingUp);
        }

        if (IsFootMoving(smoothedRightY, rightFootLastLowestY))
        {
            CheckFootRevolution(ref smoothedRightY, ref rightFootLastLowestY, ref rightFootRevolutions, ref rightFootGoingUp);
        }

        // Calculate RPM every second (average of both feet)
        if (timer >= 1f)
        {
            float totalRevolutions = leftFootRevolutions + rightFootRevolutions;
            rpm = (totalRevolutions / 2f) * 60f; // Convert to RPM (revolutions per minute)

            // Display RPM
            if (displayRPMText) displayRPMText.text = $"RPM: {rpm:F2}";

            // Reset counters
            leftFootRevolutions = 0;
            rightFootRevolutions = 0;
            timer = 0f;
        }
    }
    
    private float SmoothValue(float newValue, float[] buffer, ref int index)
    {
        buffer[index % smoothingFrames] = newValue;
        index++;
        float sum = 0f;
        for (int i = 0; i < smoothingFrames; i++)
        {
            sum += buffer[i];
        }
        return sum / smoothingFrames;
    }
    
    // Check if foot is moving meaningfully (above noise threshold)
    private bool IsFootMoving(float currentY, float lastY)
    {
        return Mathf.Abs(currentY - lastY) > movementThreshold;
    }

    private void CheckFootRevolution(ref float currentFootY, ref float lastLowestY, ref int revolutions,
        ref bool isGoingUp)
    {
        // Detect direction (up or down)
        if (currentFootY > lastLowestY + 0.01f) // Adjust threshold as needed
        {
            isGoingUp = true;
        }
        else if (currentFootY < lastLowestY - 0.01f)
        {
            isGoingUp = false;
        }

        // If foot was going up and now going down, it passed the highest point
        if (!isGoingUp && currentFootY < lastLowestY)
        {
            lastLowestY = currentFootY;
        }
        // If foot was going down and now going up, it passed the lowest point (count revolution)
        else if (isGoingUp && currentFootY > lastLowestY + 0.05f) // Small threshold to avoid noise
        {
            revolutions++;
            lastLowestY = currentFootY;
        }
    }
}