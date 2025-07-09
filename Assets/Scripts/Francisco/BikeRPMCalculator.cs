using System;
using Meta.WitAi.TTS.Utilities;
using UnityEngine;
    
    public class BikeRPMCalculator : MonoBehaviour
    {
        [SerializeField] private TTSSpeaker ttsSpeaker;
        [SerializeField] private MotivationalMessages motivationalMessages;
        [SerializeField] private ParticleSystem sequenceCompletedParticles;
        [SerializeField] private int sequenceDuration = 20;
        [SerializeField] private int rpmMinTarget = 60;
        [SerializeField] private int rpmMaxTarget = 200;
        [SerializeField] private string[] ttsOnSequenceRepetitionsCompleted = new string[]
        {
            "Great job! You completed the exercise!",
            "Well done! You've finished the sequence!",
            "Fantastic! You've completed the exercise sequence!",
        };

        [SerializeField] private TMPro.TMP_Text timeDurationDisplay;
        [SerializeField] private TMPro.TMP_Text displayRPMText;
        [SerializeField] private float movementThreshold = 0.03f;
    
        private float rightFootLastLowestY;
        private int rightFootRevolutions;
        private float timer;
        private float rpm;
        private bool rightFootGoingUp;
    
        private float timeSinceLastTTS;
        private bool hasFinishedSequence;
        
        private float currentSequenceTime;
    
        private void Start()
        {
            ttsSpeaker.Speak($"Let's start the cycling exercise. Keep pedaling for {sequenceDuration} seconds.");
        }
    
        void Update()
        {
            if (hasFinishedSequence) return;
            if (!MediaPipeBodyController.Instance.MediaPipeTransformPointsParent) return;
            Vector3 rightFootPos =
                MediaPipeBodyController.Instance.MediaPipeTransformPointsParent.GetChild(28).position;
    
            timer += Time.deltaTime;
    
            if (IsFootMoving(rightFootPos.y, rightFootLastLowestY))
            {
                CheckFootRevolution(ref rightFootPos.y, ref rightFootLastLowestY, ref rightFootRevolutions, ref rightFootGoingUp);
            }
            CountTime();
            if (CheckSequenceTime()) return;
            CheckCurrentSpeed();
            if (timer >= 1f)
            {
                rpm = rightFootRevolutions * 60f;
                if (displayRPMText) displayRPMText.text = $"RPM: {rpm:F2}";
                rightFootRevolutions = 0;
                timer = 0f;
            }
        }

        private void CountTime()
        {
            if (rpm >= rpmMinTarget && rpm <= rpmMaxTarget)
            {
                currentSequenceTime += Time.deltaTime;
                // display time in 00:00 format
                if (timeDurationDisplay)
                {
                    int minutes = Mathf.FloorToInt(currentSequenceTime / 60);
                    int seconds = Mathf.FloorToInt(currentSequenceTime % 60);
                    timeDurationDisplay.text = $"{minutes:00}:{seconds:00}";
                }
            }
        }

        private void CheckCurrentSpeed()
        {
            if (timeSinceLastTTS < 5f)
            {
                timeSinceLastTTS += Time.deltaTime;
                return;
            }
            timeSinceLastTTS = 0f;
            if (rpm > rpmMaxTarget) ttsSpeaker.Speak("Slow down!");
            else if (rpm < rpmMinTarget) ttsSpeaker.Speak("Speed up!");
        }

        private bool CheckSequenceTime()
        {
            if (currentSequenceTime >= sequenceDuration)
            {
                hasFinishedSequence = true;
                motivationalMessages.ShowMessage();
                sequenceCompletedParticles.Play();
                timeDurationDisplay.text = string.Empty;
                displayRPMText.text = string.Empty;
                ttsSpeaker.Speak(
                    ttsOnSequenceRepetitionsCompleted[UnityEngine.Random.Range(0, ttsOnSequenceRepetitionsCompleted.Length)]);
                return true;
            }
            return false;
        }
    
        private bool IsFootMoving(float currentY, float lastY)
        {
            return Mathf.Abs(currentY - lastY) > movementThreshold;
        }
    
        private void CheckFootRevolution(ref float currentFootY, ref float lastLowestY, ref int revolutions,
            ref bool isGoingUp)
        {
            if (currentFootY > lastLowestY + 0.01f)
            {
                isGoingUp = true;
            }
            else if (currentFootY < lastLowestY - 0.01f)
            {
                isGoingUp = false;
            }
    
            if (!isGoingUp && currentFootY < lastLowestY)
            {
                lastLowestY = currentFootY;
            }
            else if (isGoingUp && currentFootY > lastLowestY + 0.05f)
            {
                revolutions++;
                lastLowestY = currentFootY;
            }
        }
    }