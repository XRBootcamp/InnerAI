using System;
using UnityEngine;

public class SquatsCounter : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text displaySquatsText;

    [SerializeField]
    private float squatDownDistanceThreshold = 0.2f; // Min distance between hands and knees to register a squat down

    [SerializeField]
    private float squatStandUpThreshold = 0.5f; // Min distance between hands and knees to register standing up

    private bool isSquatting = false;
    private int squatCount = 0;

    void Start()
    {
        SetCounter();
    }

    private void SetCounter()
    {
        if (displaySquatsText) displaySquatsText.text = $"Squats: {squatCount}";
    }

    private void Update()
    {
        if (!MediaPipeBodyController.Instance.MediaPipeTransformPointsParent) return;

        // Get hand and knee positions
        Vector3 leftHandPos =
            MediaPipeBodyController.Instance.MediaPipeTransformPointsParent.GetChild(15).position; // Left hand
        Vector3 rightHandPos =
            MediaPipeBodyController.Instance.MediaPipeTransformPointsParent.GetChild(16).position; // Right hand
        Vector3 leftKneePos =
            MediaPipeBodyController.Instance.MediaPipeTransformPointsParent.GetChild(25).position; // Left knee
        Vector3 rightKneePos =
            MediaPipeBodyController.Instance.MediaPipeTransformPointsParent.GetChild(26).position; // Right knee

        // Calculate distances between hands and knees
        float leftHandToKneeDistance = Vector3.Distance(leftHandPos, leftKneePos);
        float rightHandToKneeDistance = Vector3.Distance(rightHandPos, rightKneePos);

        // Check if squatting down
        if (!isSquatting && (leftHandToKneeDistance <= squatDownDistanceThreshold &&
                             rightHandToKneeDistance <= squatDownDistanceThreshold))
        {
            isSquatting = true;
        }

        // Check if standing up
        if (isSquatting && (leftHandToKneeDistance >= squatStandUpThreshold &&
                            rightHandToKneeDistance >= squatStandUpThreshold))
        {
            isSquatting = false;
            squatCount++;
            SetCounter();
        }
    }
}