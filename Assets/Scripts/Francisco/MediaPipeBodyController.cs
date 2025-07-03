using System;
using UnityEngine;

public class MediaPipeBodyController : MonoBehaviour
{
    [SerializeField] private MediaPipeToUnityBodyJoint[] mediaPipeToUnityJoints;
    [SerializeField] private Transform mediaPipeTransformPointsParent;
    [SerializeField] private Transform hips;
    [SerializeField] private Transform spineLower;
    [SerializeField] private Transform root;
    [SerializeField] private Transform chest;
    [SerializeField] private Transform neck;
    private static MediaPipeBodyController instance;
    public static MediaPipeBodyController Instance => instance;

    public Transform MediaPipeTransformPointsParent
    {
        get => mediaPipeTransformPointsParent;
        set => mediaPipeTransformPointsParent = value;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (!mediaPipeTransformPointsParent) return;
        SetUnityPose();
        SetHips();
    }

    private void SetHips()
    {
        if (hips && spineLower && root)
        {
            // get the mid position between left hip and right hip
            Vector3 centerPosition = (mediaPipeTransformPointsParent.GetChild(23).position + mediaPipeTransformPointsParent.GetChild(24).position) / 2f;
            hips.position = centerPosition;
            spineLower.position = centerPosition;
            Vector3 rootPosition = (mediaPipeTransformPointsParent.GetChild(29).position + mediaPipeTransformPointsParent.GetChild(30).position) / 2f;
            root.position = rootPosition;
            Vector3 chestPosition = (mediaPipeTransformPointsParent.GetChild(11).position + mediaPipeTransformPointsParent.GetChild(12).position) / 2f;
            chest.position = chestPosition;
            Vector3 neckPosition = (mediaPipeTransformPointsParent.GetChild(9).position + mediaPipeTransformPointsParent.GetChild(10).position) / 2f;
            neck.position = neckPosition;
        }
    }

    private void SetUnityPose()
    {
        foreach (var joint in mediaPipeToUnityJoints)
        {
            Transform unityJointTransform = joint.unityJointTransform;
            if (unityJointTransform == null) continue;

            Transform mediaPipeTransform = mediaPipeTransformPointsParent.GetChild(joint.mediaPipeJointId);
            if (mediaPipeTransform != null)
            {
                unityJointTransform.position = mediaPipeTransform.position;
                unityJointTransform.rotation = mediaPipeTransform.rotation;
            }
        }
    }
}

[System.Serializable]
public struct MediaPipeToUnityBodyJoint
{
    public int mediaPipeJointId;
    public Transform unityJointTransform;
}