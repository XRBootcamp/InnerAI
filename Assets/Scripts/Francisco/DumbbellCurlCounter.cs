using UnityEngine;

public class DumbbellCurlCounter : MonoBehaviour
{
    [Header("Movement Detection")] [SerializeField]
    private float movementThreshold = 0.03f; // Minimum Y-change to register movement

    [Header("Rep Counting")] [SerializeField]
    private int totalReps;

    [SerializeField] private bool isMovingUp;
    [SerializeField] private bool repInProgress;
    [SerializeField] private TMPro.TMP_Text repCountText; // UI Text to display rep count


    private void Update()
    {
        if (!MediaPipeBodyController.Instance.MediaPipeTransformPointsParent) return;
        Vector3 rightShoulderPos =
            MediaPipeBodyController.Instance.MediaPipeTransformPointsParent.GetChild(11).position; // Right shoulder
        Vector3 rightHipPos =
            MediaPipeBodyController.Instance.MediaPipeTransformPointsParent.GetChild(23).position; // Right hip
        Vector3 rightHandPos =
            MediaPipeBodyController.Instance.MediaPipeTransformPointsParent.GetChild(15).position; // Right hand
        // Smooth hand Y-position to reduce jitter

        // Check if hand is moving up or down
        if (repInProgress)
        {
            if (isMovingUp && rightHandPos.y < rightShoulderPos.y + movementThreshold &&
                rightHandPos.y > rightShoulderPos.y - movementThreshold)
            {
                isMovingUp = false;
                repInProgress = false;
                totalReps++;
                repCountText.text = totalReps.ToString();
            }
        }
        else
        {
            // Start a new rep if hand is near the right hip
            if (rightHandPos.y < rightHipPos.y + movementThreshold &&
                rightHandPos.y > rightHipPos.y - movementThreshold)
            {
                repInProgress = true;
                isMovingUp = true;
            }
        }
    }
}