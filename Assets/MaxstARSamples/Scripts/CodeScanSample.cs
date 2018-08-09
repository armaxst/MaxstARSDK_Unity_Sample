/*==============================================================================
Copyright 2017 Maxst, Inc. All Rights Reserved.
==============================================================================*/

using UnityEngine;
using System.Collections.Generic;
using System;
using System.Text;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using JsonFx.Json;
using System.Collections;

using maxstAR;

public class CodeScanSample : ARBehaviour
{
	public Text codeFormatText;
	public Text codeValueText;
	public Button startScanBtn;
	private Text btnText;

    private CameraBackgroundBehaviour cameraBackgroundBehaviour = null;

    void Awake()
    {
		Init();

        cameraBackgroundBehaviour = FindObjectOfType<CameraBackgroundBehaviour>();
        if (cameraBackgroundBehaviour == null)
        {
            Debug.LogError("Can't find CameraBackgroundBehaviour.");
            return;
        }
    }

	void Start()
	{
		if (startScanBtn != null)
		{
			btnText = startScanBtn.GetComponentInChildren<Text>();
		}

		StartCodeScan();
		StartCameraInternal();
	}

	void Update()
	{
        TrackingState state = TrackerManager.GetInstance().UpdateTrackingState();

        cameraBackgroundBehaviour.UpdateCameraBackgroundImage(state);

        string codeScanResult = state.GetCodeScanResult();
		if (!codeScanResult.Equals("") && codeScanResult.Length > 0)
		{
			TrackerManager.GetInstance().StopTracker();
			TrackerManager.GetInstance().DestroyTracker();
			startScanBtn.interactable = true;
			btnText.text = "Start Scan";

			Dictionary<string, string> resultAsDicionary =
				new JsonReader(codeScanResult).Deserialize<Dictionary<string, string>>();

			codeFormatText.text = "Format : " + resultAsDicionary["Format"];
			codeValueText.text = "Value : " + resultAsDicionary["Value"];
		}
	}

	void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			TrackerManager.GetInstance().StopTracker();
			startScanBtn.interactable = true;
			btnText.text = "Start Scan";
			StopCameraInternal();
		}
		else
		{
			StartCodeScan();
			StartCameraInternal();
		}
	}

	void OnDestroy()
	{
		TrackerManager.GetInstance().StopTracker();
		TrackerManager.GetInstance().DestroyTracker();
		StopCameraInternal();
	}

	public void StartCodeScan()
	{
		startScanBtn.interactable = false;
		btnText.text = "Scanning...";
		codeFormatText.text = "";
		codeValueText.text = "";
		TrackerManager.GetInstance().StartTracker(TrackerManager.TRACKER_TYPE_CODE_SCANNER);
	}

	private void StartCameraInternal()
	{
		StartCamera();
		StartCoroutine(AutoFocusCoroutine());
	}

	private void StopCameraInternal()
	{
		StopCamera();
		StopCoroutine(AutoFocusCoroutine());
	}

	IEnumerator AutoFocusCoroutine()
	{
		while (true)
		{
			CameraDevice.GetInstance().SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_AUTO);
			yield return new WaitForSeconds(3.0f);
		}
	}
}
