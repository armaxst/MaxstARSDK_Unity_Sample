/*==============================================================================
Copyright 2017 Maxst, Inc. All Rights Reserved.
==============================================================================*/

using UnityEngine;
using System.Collections;

public class ExtramHomeSceneManager : ARBehaviour
{
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void OnImageTrackerClick()
    {
		SceneStackManager.Instance.LoadScene("ExtraHome", "ExtraImageTrackerKnight");
    }

	public void OnImageTrackerMultiTargetClick()
    {
		SceneStackManager.Instance.LoadScene("ExtraHome", "ExtraImageTrackerMultiTarget");
    }

	public void OnInstantTrackerBrushClick()
    {
		SceneStackManager.Instance.LoadScene("ExtraHome", "ExtraInstantTrackerBrush");
    }

	public void OnExtraInstantTrackerGridCick()
    {
		SceneStackManager.Instance.LoadScene("ExtraHome", "ExtraInstantTrackerGrid");
    }

	public void OnExtraInstantTrackerMultiContentsClick()
    {
		SceneStackManager.Instance.LoadScene("ExtraHome", "ExtraInstantTrackerMultiContents");
    }

	public void OnExtraVisualSLAMBrushClick()
    {
		SceneStackManager.Instance.LoadScene("ExtraHome", "ExtraVisualSLAMBrush");
    }

	public void OnExtraVisualSLAMKnightClick()
    {
		SceneStackManager.Instance.LoadScene("ExtraHome", "ExtraVisualSLAMKnight");
    }

    public void OnExtraInstantTrainingClick()
    {
        SceneStackManager.Instance.LoadScene("ExtraHome", "InstantTraining");
    }
}