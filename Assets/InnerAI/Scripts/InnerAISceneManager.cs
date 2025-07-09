using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InnerAISceneManager : MonoBehaviour
{
    public MenuManager menuManager;
    
    public SelectedExcercise selectedExcercise = SelectedExcercise.Default;
    public SelectedTracking selectedTracking = SelectedTracking.Default;
    
    public enum SelectedExcercise
    {
        Default = 0,
        Cycling = 1,
        FiveMinWarmUp = 2,
        FifteenMinSweatWorkout = 3,
        FlamingKatana = 4,
        SquatWorkout = 5,
        BicepCurl = 6,
        YogaSequence = 7
        
    }

    public enum SelectedTracking
    {
        Default = 0,
        QuestHeadset = 1,
        ExternalCamera = 2
    }
    
    public void SelectExcercise(int excerciseIndex)
    {
        selectedExcercise = (SelectedExcercise)excerciseIndex;
        
        menuManager.OpenWindow(4);
    }

    public void SelectTracking(int trackingIndex)
    {
        selectedTracking = (SelectedTracking)trackingIndex;

        switch (selectedTracking)
        {
            case SelectedTracking.QuestHeadset:
                StartWorkout();
                break;
            case SelectedTracking.ExternalCamera:
                menuManager.OpenWindow(5);
                break;
            default:
                break;
        }
    }

    public void ExternalCameraScreenProceed()
    {
        StartWorkout();
    }
    
    public void ExternalCameraScreenQuit()
    {
        SwitchSceneByName("FlamingKatanaWithMediaPipe_With_UI");
    }

    public void StartWorkout()
    {
        switch (selectedExcercise)
        {
            case SelectedExcercise.Cycling:
                WorkoutScenesToSelectBySelectedTracking("Francisco_MediaPipe_StationaryBike", "Francisco_MediaPipe_StationaryBike");
                break;
            case SelectedExcercise.FiveMinWarmUp:
                break;
            case SelectedExcercise.FifteenMinSweatWorkout:
                break;
            case SelectedExcercise.FlamingKatana:
                WorkoutScenesToSelectBySelectedTracking("FlamingKatanaWithMediaPipe", "FlamingKatanaWithMediaPipe");
                break;
            case SelectedExcercise.SquatWorkout:
                WorkoutScenesToSelectBySelectedTracking("Francisco_Movement_Squats", "Francisco_MediaPipe_Squats");
                break;
            case SelectedExcercise.BicepCurl:
                WorkoutScenesToSelectBySelectedTracking("Francisco_Movement_BicepCurls", "Francisco_MediaPipe_BicepCurls");
                break;
            case SelectedExcercise.YogaSequence:
                WorkoutScenesToSelectBySelectedTracking("Francisco_Movement_YogaSequence", "Francisco_Movement_YogaSequence");
                break;
            default:
                break;
        }
    }

    private void WorkoutScenesToSelectBySelectedTracking(string QuestHeadsetScene, string ExternalCameraScene)
    {
        if (selectedTracking == SelectedTracking.QuestHeadset)
        {
            SwitchSceneByName(QuestHeadsetScene);
        }
        else if (selectedTracking == SelectedTracking.ExternalCamera)
        {
            SwitchSceneByName(ExternalCameraScene);
        }
    }
    
    public void SwitchSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    
}
