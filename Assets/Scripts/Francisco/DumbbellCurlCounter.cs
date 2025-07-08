using System;
using Meta.WitAi.TTS.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

public class DumbbellCurlCounter : MonoBehaviour
{
    [SerializeField] private TTSSpeaker ttsSpeaker;
    [SerializeField] private MotivationalMessages motivationalMessages;
    [SerializeField] private ParticleSystem sequenceCompletedParticles;
    [SerializeField] private int sequenceRepetitions = 5;
    [SerializeField] private string[] ttsOnSequenceRepetitionsCompleted = new string[]
    {
        "Great job! You completed the exercise!",
        "Well done! You've finished the sequence!",
        "Fantastic! You've completed the exercise sequence!",
    };
    
    [Header("Movement Detection")] [SerializeField]
    private float movementThreshold = 0.03f; // Minimum Y-change to register movement

    [Header("Rep Counting")] [SerializeField]
    private int totalReps;

    [SerializeField] private bool isMovingUp;
    [SerializeField] private bool repInProgress;
    [SerializeField] private TMPro.TMP_Text repCountText; // UI Text to display rep count

    private bool hasFinishedSequence;

    private void Start()
    {
        if (ttsSpeaker) ttsSpeaker.Speak("Let's start the biceps curl exercise. Low your arm.");
    }

    private void Update()
    {
        if (hasFinishedSequence) return;
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
                string repsText = sequenceRepetitions == 0 ? totalReps.ToString() : $"{totalReps}/{sequenceRepetitions}";
                repCountText.text = repsText;
                motivationalMessages.ShowMessage();
                if (totalReps >= sequenceRepetitions)
                {
                    if (sequenceCompletedParticles) sequenceCompletedParticles.Play();
                    ttsSpeaker.Speak(ttsOnSequenceRepetitionsCompleted[Random.Range(0, ttsOnSequenceRepetitionsCompleted.Length)]);
                    hasFinishedSequence = true;
                }
                else
                {
                    ttsSpeaker.Speak("Arm down!");
                }
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
                if (ttsSpeaker) ttsSpeaker.Speak("Arm up!");
            }
        }
    }
}