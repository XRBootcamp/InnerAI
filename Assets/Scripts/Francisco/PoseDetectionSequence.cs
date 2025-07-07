using Meta.WitAi.TTS.Utilities;
using Meta.XR.Movement.BodyTrackingForFitness;
using UnityEngine;

public class PoseDetectionSequence : MonoBehaviour
{
    [SerializeField] private PoseRequirements[] poses;
    [SerializeField] private TMPro.TextMeshProUGUI sequenceRepetitionsCounter;
    [SerializeField] private MotivationalMessages motivationalMessages;
    [SerializeField] private TTSSpeaker ttsSpeaker;
    [SerializeField] private int sequenceRepetitions = 0;
    [SerializeField] private string ttsOnSequenceRepetitionsCompleted = "Great job! You completed the exercise!";

    
    private int currentPoseIndex;
    private int completedPoses;
    private bool isPoseCompleted;
    private int currentSequenceRepetitions = 0;
    private float timer;
    private bool isSequenceOver;

    void Start()
    {
        if (poses == null || poses.Length == 0) return;
        StartSequence();
    }

    private void SetActivePose()
    {
        foreach (var pose in poses)
        {
            if (!isSequenceOver && currentPoseIndex < poses.Length && pose.poseDetector == poses[currentPoseIndex].poseDetector)
            {
                pose.poseDetector.transform.parent.gameObject.SetActive(true);
                isPoseCompleted = false;
                ttsSpeaker.Speak(pose.ttsInstructionMessage);
            }
            else
            {
                pose.poseDetector.transform.parent.gameObject.SetActive(false);
            }
        }
    }

    public void OnPoseCompliance()
    {
        if (isPoseCompleted) return;
        isPoseCompleted = true;
        currentPoseIndex++;
        SetActivePose();
        completedPoses++;
        if (completedPoses >= poses.Length)
        {
            Debug.Log("Pose sequence completed!");
            // Trigger any additional logic for completing the sequence
            currentSequenceRepetitions++;
            string counterText = sequenceRepetitions == 0 ? $"{currentSequenceRepetitions}" : $"{currentSequenceRepetitions}/{sequenceRepetitions}";
            sequenceRepetitionsCounter.text = counterText;
            motivationalMessages?.ShowMessage();
            if (sequenceRepetitions == 0 || currentSequenceRepetitions < sequenceRepetitions) StartSequence();
            else
            {
                isSequenceOver = true;
                ttsSpeaker?.Speak(ttsOnSequenceRepetitionsCompleted);
                SetActivePose();
            }
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
    public string ttsInstructionMessage;
}