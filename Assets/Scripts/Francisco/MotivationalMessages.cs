using UnityEngine;

public class MotivationalMessages : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text motivationalMessage;
    [SerializeField] private Animation motivationalMessageAnimation;
    [SerializeField] private string[] motivationalMessages = new string[]
    {
        "WELL DONE!",
        "EXCELLENT!",
        "FANTASTIC!"
    };
    
    public void ShowMessage()
    {
        if (motivationalMessage == null || motivationalMessageAnimation == null) return;

        motivationalMessage.text = motivationalMessages[Random.Range(0, motivationalMessages.Length)];
        motivationalMessageAnimation.Play();
    }
    
}
