using Meta.XR.Movement.BodyTrackingForFitness;
using UnityEngine;

public class PoseDetectionSequence : MonoBehaviour
{
    [SerializeField] private PoseRequirements[] poses;
    [SerializeField] private Counter counter;
    private int currentPoseIndex = 0;
    private int completedPoses = 0;

    void Start()
    {
        if (poses == null || poses.Length == 0) return;
        StartSequence();
    }

    private void SetActivePose()
    {
        foreach (var pose in poses)
        {
            if (currentPoseIndex < poses.Length && pose.poseDetector == poses[currentPoseIndex].poseDetector)
            {
                pose.poseDetector.transform.parent.gameObject.SetActive(true);
                pose.poseDetector._poseEvents.OnCompliance.AddListener(OnPoseCompliance);
            }
            else
            {
                pose.poseDetector.transform.parent.gameObject.SetActive(false);
            }
        }
    }

    private void OnPoseCompliance()
    {
        poses[currentPoseIndex].poseDetector._poseEvents.OnCompliance.RemoveListener(OnPoseCompliance);
        currentPoseIndex++;
        SetActivePose();
        completedPoses++;
        if (completedPoses >= poses.Length)
        {
            Debug.Log("Pose sequence completed!");
            // Trigger any additional logic for completing the sequence
            counter?.Add(1);
            StartSequence();
        }
        else
        {
            if (currentPoseIndex < poses.Length)
            {
                Debug.Log($"Next pose: {poses[currentPoseIndex].poseDetector.name}");
            }
            else
            {
                Debug.Log("No more poses in the sequence.");
            }
        }
    }

    private void StartSequence()
    {
        currentPoseIndex = 0;
        completedPoses = 0;
        SetActivePose();
        Debug.Log("Pose sequence started.");
    }
}

[System.Serializable]
public struct PoseRequirements
{
    public BodyPoseAlignmentDetector poseDetector;
    public float minTimeInPose;
    public float maxTimeInPose;
    public int repetitionsRequired;
}