using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InnerAISceneReset : MonoBehaviour
{
    public static InnerAISceneReset Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            // SwitchSceneByName("Main Menu");
            SceneManager.LoadScene(0);
        }
    }

    public void SwitchSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    
}
