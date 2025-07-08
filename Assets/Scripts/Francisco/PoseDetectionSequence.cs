using System;
using System.Collections;
using Meta.WitAi.TTS.Utilities;
using Meta.XR.Movement.BodyTrackingForFitness;
using UnityEngine;
using Random = UnityEngine.Random;

public class PoseDetectionSequence : MonoBehaviour
{
    [SerializeField] private PoseRequirements[] poses;
    [SerializeField] private TMPro.TextMeshProUGUI sequenceRepetitionsCounter;
    [SerializeField] private MotivationalMessages motivationalMessages;
    [SerializeField] private TTSSpeaker ttsSpeaker;
    [SerializeField] private int sequenceRepetitions = 0;
    [SerializeField] private ParticleSystem sequenceCompletedParticles;
    [SerializeField] private string[] ttsOnSequenceRepetitionsCompleted = new string[]
    {
        "Great job! You completed the exercise!",
        "Well done! You've finished the sequence!",
        "Fantastic! You've completed the exercise sequence!",
    };

    
    private int currentPoseIndex;
    private int completedPoses;
    private bool isPoseCompleted;
    private int currentSequenceRepetitions = 0;
    private bool isSequenceOver;
    private Coroutine poseVerificationCoroutine;
    private bool switchingPose;
    private float timeSinceLastPoseChange;
    void Start()
    {
        if (poses == null || poses.Length == 0) return;
        StartSequence();
    }

    private void Update()
    {
        timeSinceLastPoseChange += Time.deltaTime;
    }

    private void SetActivePose()
    {
        switchingPose = true;
        foreach (var pose in poses)
        {
            if (!isSequenceOver && currentPoseIndex < poses.Length && pose.poseDetector == poses[currentPoseIndex].poseDetector)
            {
                pose.poseDetector.transform.parent.gameObject.SetActive(true);
                isPoseCompleted = false;
                string instructionMessage = pose.minTimeInPose == 0 ? pose.ttsInstructionMessage : $"{pose.ttsInstructionMessage} and hold for {pose.minTimeInPose} seconds.";
                ttsSpeaker.Speak(instructionMessage);
            }
            else
            {
                pose.poseDetector.transform.parent.gameObject.SetActive(false);
            }
        }
        switchingPose = false;
    }

    public void OnPoseCompliance()
    {
        if (isPoseCompleted) return;
        isPoseCompleted = true;
        poseVerificationCoroutine = StartCoroutine(StartPoseVerification());
    }
    
    public void OnPoseDefiance()
    {
        if (poses[currentPoseIndex].minTimeInPose == 0 || switchingPose || timeSinceLastPoseChange < 1f) return;
        if (poseVerificationCoroutine != null)
        {
            StopCoroutine(poseVerificationCoroutine);
            poseVerificationCoroutine = null;
        }
        isPoseCompleted = false;
        string instructionMessage = $"You didn't hold enough. {poses[currentPoseIndex].ttsInstructionMessage} and hold for {poses[currentPoseIndex].minTimeInPose} seconds.";
        ttsSpeaker.Speak(instructionMessage);
    }

    private IEnumerator StartPoseVerification()
    {
        yield return new WaitForSeconds(poses[currentPoseIndex].minTimeInPose);
        timeSinceLastPoseChange = 0f;
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
                sequenceCompletedParticles.Play();
                ttsSpeaker?.Speak(ttsOnSequenceRepetitionsCompleted[Random.Range(0, ttsOnSequenceRepetitionsCompleted.Length)]);
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