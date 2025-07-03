using System;
using UnityEngine;
using System.Collections.Generic;

public class DelayedObjectFollowWithOffset : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Delay in seconds to match passthrough lag")]
    private float lagTime = 0.1f; // Adjustable lag time in seconds

    [SerializeField]
    [Tooltip("Controller Transform to follow")]
    private Transform controllerTransform; // Reference to the Quest 3 controller transform

    // [SerializeField]
    [Tooltip("Local position offset from the controller in its local space")]
    private Vector3 localPositionOffset = Vector3.zero; // Local position offset

    // [SerializeField]
    [Tooltip("Local rotation offset from the controller (Euler angles)")]
    private Vector3 localRotationOffset = Vector3.zero; // Local rotation offset (Euler angles)

    // Structure to store transform data with timestamp
    private struct TransformData
    {
        public Vector3 position;
        public Quaternion rotation;
        public float time;
    }

    private Queue<TransformData> transformBuffer = new Queue<TransformData>();

    private void Start()
    {
        localPositionOffset = transform.localPosition;
        localRotationOffset = transform.localEulerAngles;
        transform.parent = null;
    }

    void Update()
    {
        if (controllerTransform == null)
        {
            Debug.LogWarning("Controller Transform not assigned!");
            return;
        }

        // Store current controller transform with timestamp
        TransformData data = new TransformData
        {
            position = controllerTransform.position,
            rotation = controllerTransform.rotation,
            time = Time.time
        };
        transformBuffer.Enqueue(data);

        // Remove outdated entries
        while (transformBuffer.Count > 0 && (Time.time - transformBuffer.Peek().time) > lagTime)
        {
            transformBuffer.Dequeue();
        }

        // Apply delayed world-space transform with offsets if available
        if (transformBuffer.Count > 0)
        {
            TransformData delayedData = transformBuffer.Peek();

            // Apply local position offset in controller's local space, then convert to world space
            Vector3 offsetInWorld = delayedData.rotation * localPositionOffset;
            transform.position = delayedData.position + offsetInWorld;

            // Apply local rotation offset
            Quaternion offsetRotation = Quaternion.Euler(localRotationOffset);
            transform.rotation = delayedData.rotation * offsetRotation;
        }
    }

    // Clear buffer on disable to avoid stale data
    void OnDisable()
    {
        transformBuffer.Clear();
    }
}