using UnityEngine;

public class MediaPipeSwordTracking : MonoBehaviour
{
    [SerializeField] private GameObject swordPrefab;

    [SerializeField]
    private Vector3 swordOffset = new Vector3(0, 0.1f, 0); // Offset to position the sword slightly above the hand

    private GameObject sword;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (!MediaPipeBodyController.Instance.MediaPipeTransformPointsParent)
        {
            if (sword != null)
            {
                Destroy(sword); // Destroy the sword if MediaPipeTransformPointsParent is not set
                sword = null;
            }

            return;
        }
        
        // Check if the sword is already instantiated
        if (sword == null)
        {
            Transform rightHand =
                MediaPipeBodyController.Instance.MediaPipeTransformPointsParent.GetChild(16); // Right hand
            // Instantiate the sword at the right hand position
            sword = Instantiate(swordPrefab, rightHand);
        }
    }
}