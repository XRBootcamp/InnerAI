using System;
using Meta.WitAi.TTS.Utilities;
using UnityEngine;

public class SquatsCounter : MonoBehaviour
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
    [SerializeField] private TMPro.TMP_Text displaySquatsText;

    [SerializeField]
    private float squatDownDistanceThreshold = 0.2f; // Min distance between hands and knees to register a squat down

    [SerializeField]
    private float squatStandUpThreshold = 0.5f; // Min distance between hands and knees to register standing up

    private bool isSquatting = false;
    private int squatCount = 0;
    private bool hasFinishedSequence;

    void Start()
    {
        SetCounter();
        if (ttsSpeaker)
        {
            ttsSpeaker.Speak("Let's start the squats exercise. Lower your body down.");
        }
    }

    private void SetCounter()
    {
        string squatsText = sequenceRepetitions == 0 ? squatCount.ToString() : $"{squatCount}/{sequenceRepetitions}";
        if (displaySquatsText) displaySquatsText.text = squatsText;
    }

    private void Update()
    {
        if (hasFinishedSequence) return;
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
            if (ttsSpeaker) ttsSpeaker.Speak("Body up!");
        }

        // Check if standing up
        if (isSquatting && (leftHandToKneeDistance >= squatStandUpThreshold &&
                            rightHandToKneeDistance >= squatStandUpThreshold))
        {
            isSquatting = false;
            squatCount++;
            SetCounter();
            motivationalMessages.ShowMessage();
            if (squatCount >= sequenceRepetitions)
            {
                if (sequenceCompletedParticles) sequenceCompletedParticles.Play();
                ttsSpeaker.Speak(ttsOnSequenceRepetitionsCompleted[UnityEngine.Random.Range(0, ttsOnSequenceRepetitionsCompleted.Length)]);
                hasFinishedSequence = true;
            }
            else
            {
                if (ttsSpeaker) ttsSpeaker.Speak("Body down!");
            }
        }
    }
}