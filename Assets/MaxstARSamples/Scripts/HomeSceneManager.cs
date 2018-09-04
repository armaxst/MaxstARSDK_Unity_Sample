/*==============================================================================
Copyright 2017 Maxst, Inc. All Rights Reserved.
==============================================================================*/

using UnityEngine;
using System.Collections;

public class HomeSceneManager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void OnImageTargetClick()
    {
        SceneStackManager.Instance.LoadScene("Home", "ImageTracker");
    }

    public void OnMarkerImageClick()
    {
        SceneStackManager.Instance.LoadScene("Home", "MarkerTracker");
    }

    public void OnInstantImageClick()
    {
        SceneStackManager.Instance.LoadScene("Home", "InstantTracker");
    }

    public void OnSlamCick()
    {
        SceneStackManager.Instance.LoadScene("Home", "VisualSLAM");
    }

    public void OnObjectTargetClick()
    {
        SceneStackManager.Instance.LoadScene("Home", "ObjectTracker");
    }

    public void OnQR_BarcodeClick()
    {
        SceneStackManager.Instance.LoadScene("Home", "CodeScan");
    }

    public void OnCameraConfigClick()
    {
        SceneStackManager.Instance.LoadScene("Home", "CameraConfiguration");
    }

    public void OnCloudRecognizerClick()
    {
        SceneStackManager.Instance.LoadScene("Home", "CloudRecognizer");
    }
}