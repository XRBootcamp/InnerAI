using System;
using System.Collections;
using Mediapipe.Unity;
using Meta.WitAi.Attributes;
using UnityEngine;

public class AssignHips : MonoBehaviour
{
    public MultiPoseLandmarkListWithMaskAnnotation listAnnotation;


    private void Start()
    {
        StartCoroutine(CheckLandmarks());
    }
    
    

    IEnumerator CheckLandmarks()
    {
        while (true)
        {
            Debug.Log("Landmarks: " + listAnnotation.children.Count);
            yield return new WaitForSeconds(0.5f);
        }
    }
}
