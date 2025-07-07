using System;
    using UnityEngine;
    
    public class BikeRPMCalculator : MonoBehaviour
    {
        [SerializeField] private TMPro.TMP_Text displayRPMText;
        [SerializeField] private float movementThreshold = 0.03f;
        [SerializeField] private int smoothingFrames = 5;
    
        private float rightFootLastLowestY;
        private int rightFootRevolutions;
        private float timer;
        private float rpm;
        private bool rightFootGoingUp;
    
        private float[] rightFootBuffer;
        private int bufferIndex;
    
        private void Start()
        {
            rightFootBuffer = new float[smoothingFrames];
        }
    
        void Update()
        {
            if (!MediaPipeBodyController.Instance.MediaPipeTransformPointsParent) return;
            Vector3 rightFootPos =
                MediaPipeBodyController.Instance.MediaPipeTransformPointsParent.GetChild(28).position;
    
            float smoothedRightY = SmoothValue(rightFootPos.y, rightFootBuffer, ref bufferIndex);
            timer += Time.deltaTime;
    
            if (IsFootMoving(smoothedRightY, rightFootLastLowestY))
            {
                CheckFootRevolution(ref smoothedRightY, ref rightFootLastLowestY, ref rightFootRevolutions, ref rightFootGoingUp);
            }
    
            if (timer >= 1f)
            {
                rpm = rightFootRevolutions * 60f;
                if (displayRPMText) displayRPMText.text = $"RPM: {rpm:F2}";
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