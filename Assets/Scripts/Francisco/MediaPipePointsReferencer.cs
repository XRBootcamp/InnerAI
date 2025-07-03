using UnityEngine;

public class MediaPipePointsReferencer : MonoBehaviour
{
    void Start()
    {
        if (MediaPipeBodyController.Instance && MediaPipeBodyController.Instance.MediaPipeTransformPointsParent == null)
        {
            MediaPipeBodyController.Instance.MediaPipeTransformPointsParent = transform;
        }
    }

    
}
