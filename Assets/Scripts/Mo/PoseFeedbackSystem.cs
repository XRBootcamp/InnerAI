using System;
using System.Collections;
using Meta.WitAi.TTS.Utilities;
using Meta.XR.Movement.BodyTrackingForFitness;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections.Generic;

public class PoseFeedbackSystem : MonoBehaviour
{
    [SerializeField] private FeedbackRequirements[] poses;
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
    
    [Header ("Screenshots for poses")] 
    [SerializeField] private GameObject feedback;
    [SerializeField] private UnityEngine.UI.Image[] screenshotImages;
    private List<Texture2D> capturedScreenshots = new List<Texture2D>();
    private List<bool> screenshotTakenForPose = new List<bool>();

    
    private int currentPoseIndex;
    private int completedPoses;
    private bool isPoseCompleted;
    private int currentSequenceRepetitions = 0;
    private bool isSequenceOver;
    private Coroutine poseVerificationCoroutine;
    private bool switchingPose;
    private float timeSinceLastPoseChange;
    private bool showingFeedback = false;
    
    void Start()
    {
        if (poses == null || poses.Length == 0) return;
        feedback.SetActive(false);
        
        //Debug
        feedback.SetActive(true);
        
        screenshotTakenForPose.Clear();
        for (int i = 0; i < poses.Length; i++)
        {
            screenshotTakenForPose.Add(false);
        }
        
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
        if (isPoseCompleted || showingFeedback) return;
        isPoseCompleted = true;
        poseVerificationCoroutine = StartCoroutine(StartPoseVerification());
    }
    
    public void OnPoseDefiance()
    {
        if (poses[currentPoseIndex].minTimeInPose == 0 || switchingPose || timeSinceLastPoseChange < 1f || showingFeedback) return;
        
        if (!HasScreenshotForCurrentPose())
        {
            CaptureScreenshot();
        }
        
        if (poseVerificationCoroutine != null)
        {
            StopCoroutine(poseVerificationCoroutine);
            poseVerificationCoroutine = null;
        }
        isPoseCompleted = false;
        string instructionMessage = $"You didn't hold enough. {poses[currentPoseIndex].ttsInstructionMessage} and hold for {poses[currentPoseIndex].minTimeInPose} seconds.";
        ttsSpeaker.Speak(instructionMessage);
    }

    private bool HasScreenshotForCurrentPose()
    {
        return currentPoseIndex < screenshotTakenForPose.Count && screenshotTakenForPose[currentPoseIndex];
    }

    private IEnumerator StartPoseVerification()
    {
        yield return new WaitForSeconds(poses[currentPoseIndex].minTimeInPose);
        timeSinceLastPoseChange = 0f;
        currentPoseIndex++;
        completedPoses++;
        
        if (completedPoses >= poses.Length)
        {
            Debug.Log("Pose sequence completed!");
            currentSequenceRepetitions++;
            string counterText = sequenceRepetitions == 0 ? $"{currentSequenceRepetitions}" : $"{currentSequenceRepetitions}/{sequenceRepetitions}";
            sequenceRepetitionsCounter.text = counterText;
            motivationalMessages?.ShowMessage();
            
            isSequenceOver = true;
            sequenceCompletedParticles.Play();
            ttsSpeaker?.Speak(ttsOnSequenceRepetitionsCompleted[Random.Range(0, ttsOnSequenceRepetitionsCompleted.Length)]);
            
            // Deactivate all pose detectors
            foreach (var pose in poses)
            {
                pose.poseDetector.transform.parent.gameObject.SetActive(false);
            }
            
            // Force show feedback after 5 seconds
            StartCoroutine(ForceShowFeedback());
        }
        else
        {
            SetActivePose();
            if (currentPoseIndex < poses.Length)
            {
                Debug.Log($"Next pose: {poses[currentPoseIndex].poseDetector.name}");
            }
        }
    }
    
    private void CaptureScreenshot()
    {
        StartCoroutine(CaptureScreenshotCoroutine());
    }

    private IEnumerator CaptureScreenshotCoroutine()
    {
        yield return new WaitForEndOfFrame();
    
        Texture2D screenshot = new Texture2D(200, 200, TextureFormat.RGB24, false);
        screenshot.ReadPixels(new Rect(Screen.width/2 - 100, Screen.height/2 - 100, 200, 200), 0, 0);
        screenshot.Apply();
    
        capturedScreenshots.Add(screenshot);
        
        if (currentPoseIndex < screenshotTakenForPose.Count)
        {
            screenshotTakenForPose[currentPoseIndex] = true;
        }
    }
    
    private void ShowScreenshotGallery()
    {
        Debug.Log("Forcing feedback screen to appear!");
        feedback.SetActive(true);
        showingFeedback = true;
    
        for (int i = 0; i < capturedScreenshots.Count && i < screenshotImages.Length; i++)
        {
            Sprite sprite = Sprite.Create(capturedScreenshots[i], new Rect(0, 0, 200, 200), Vector2.one * 0.5f);
            screenshotImages[i].sprite = sprite;
            screenshotImages[i].gameObject.SetActive(true);
        }
        
        Debug.Log($"Feedback active: {feedback.activeSelf}, Screenshots count: {capturedScreenshots.Count}");
    }
    
    private IEnumerator ForceShowFeedback()
    {
        Debug.Log("Waiting 5 seconds before showing feedback...");
        yield return new WaitForSeconds(5f);
        Debug.Log("5 seconds passed, forcing feedback to show!");
        ShowScreenshotGallery();
    }

    private void StartSequence()
    {
        currentPoseIndex = 0;
        completedPoses = 0;
        isSequenceOver = false;
        showingFeedback = false;
        capturedScreenshots.Clear();
        
        for (int i = 0; i < screenshotTakenForPose.Count; i++)
        {
            screenshotTakenForPose[i] = false;
        }
        
        feedback.SetActive(false);
        SetActivePose();
        Debug.Log("Pose sequence started.");
    }
    
    // Method to be called by Repeat button
    public void RepeatExercise()
    {
        Debug.Log("Repeating exercise...");
        StartSequence();
    }
    
    // Method to be called by Exit button
    public void ExitExercise()
    {
        Debug.Log("Exiting exercise...");
        feedback.SetActive(false);
        isSequenceOver = true;
        showingFeedback = false;
        
        // Deactivate all pose detectors
        foreach (var pose in poses)
        {
            pose.poseDetector.transform.parent.gameObject.SetActive(false);
        }
        
        // Add any additional exit logic here (like returning to main menu, etc.)
    }
}

[System.Serializable]
public struct FeedbackRequirements
{
    public BodyPoseAlignmentDetector poseDetector;
    public float minTimeInPose;
    public float maxTimeInPose;
    public int repetitionsRequired;
    public string ttsInstructionMessage;
}