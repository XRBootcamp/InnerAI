using System;
using UnityEngine;
using System.Collections.Generic;

public class DelayedObjectFollow : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Delay in seconds to match passthrough lag")]
    private float lagTime = 0.1f; // Adjustable lag time in seconds

    [SerializeField]
    [Tooltip("Controller Transform to follow")]
    private Transform controllerTransform; // Reference to the Quest 3 controller transform

    // Structure to store transform data with timestamp
    private struct TransformData
    {
        public Vector3 position;
        public Quaternion rotation;
        public float time;
    }

    private Queue<TransformData> transformBuffer = new Queue<TransformData>();
    
    private Vector3 startPosition;
    private Quaternion startRotation;

    private void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
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

        // Apply delayed transform if available
        if (transformBuffer.Count > 0)
        {
            TransformData delayedData = transformBuffer.Peek();
            transform.position = delayedData.position + startPosition;
            transform.rotation = delayedData.rotation * startRotation;
        }
    }

    // Optional: Clear buffer on disable to avoid stale data
    void OnDisable()
    {
        transformBuffer.Clear();
    }
}