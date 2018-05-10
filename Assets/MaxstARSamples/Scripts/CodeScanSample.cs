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

	private bool cameraStartDone = false;

    private CameraBackgroundBehaviour cameraBackgroundBehaviour = null;

    void Awake()
    {
		base.Awake();

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
	}

	void Update()
	{
		if (!cameraStartDone)
		{
			StartCamera();
		}

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
			StopCamera();
		}
	}

	void OnDestroy()
	{
		TrackerManager.GetInstance().StopTracker();
		TrackerManager.GetInstance().DestroyTracker();
		StopCamera();
	}

	public void StartCodeScan()
	{
		startScanBtn.interactable = false;
		btnText.text = "Scanning...";
		codeFormatText.text = "";
		codeValueText.text = "";
		TrackerManager.GetInstance().StartTracker(TrackerManager.TRACKER_TYPE_CODE_SCANNER);
	}

	private void StartCamera()
	{
		if (!cameraStartDone)
		{
			Debug.Log("Unity StartCamera");
			ResultCode result = CameraDevice.GetInstance().Start();
			if (result == ResultCode.Success)
			{
				cameraStartDone = true;
				StartCoroutine(AutoFocusCoroutine());
				//CameraDevice.GetInstance().SetAutoWhiteBalanceLock(true);   // For ODG-R7 preventing camera flickering
			}
		}
	}

	private void StopCamera()
	{
		if (cameraStartDone)
		{
			Debug.Log("Unity StopCamera");
			CameraDevice.GetInstance().Stop();
			cameraStartDone = false;
			StopCoroutine(AutoFocusCoroutine());
		}
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
