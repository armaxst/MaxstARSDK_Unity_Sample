using UnityEngine;
using System.Collections.Generic;
using System.Text;
using UnityEngine.SceneManagement;

using maxstAR;

public class MultiImageTargetA : MonoBehaviour
{
	private Dictionary<string, ImageTrackableBehaviour> imageTrackablesMap =
		new Dictionary<string, ImageTrackableBehaviour>();

	private bool startTrackerDone = false;
	public ARManager arManagerObject;

	void Start()
	{
		imageTrackablesMap.Clear();
		ImageTrackableBehaviour[] imageTrackables = FindObjectsOfType<ImageTrackableBehaviour>();
		foreach (var trackable in imageTrackables)
		{
			imageTrackablesMap.Add(trackable.TrackableName, trackable);
			Debug.Log("Trackable add: " + trackable.TrackableName);
		}

		AddTrackerData();

		if (arManagerObject != null)
		{
			DontDestroyOnLoad(arManagerObject);
		}
	}

	private void AddTrackerData()
	{
		foreach (var trackable in imageTrackablesMap)
		{
			if (trackable.Value.TrackerDataFileName.Length == 0)
			{
				continue;
			}

			if (trackable.Value.StorageType == StorageType.AbsolutePath)
			{
				TrackerManager.GetInstance().AddTrackerData(trackable.Value.TrackerDataFileName);
			}
			else
			{
				if (Application.platform == RuntimePlatform.Android)
				{
					TrackerManager.GetInstance().AddTrackerData(trackable.Value.TrackerDataFileName, true);
				}
				else
				{
					TrackerManager.GetInstance().AddTrackerData(Application.streamingAssetsPath + "/" + trackable.Value.TrackerDataFileName);
				}
			}
		}

		TrackerManager.GetInstance().LoadTrackerData();
	}

	private void DisableAllTrackables()
	{
		foreach (var trackable in imageTrackablesMap)
		{
			trackable.Value.OnTrackFail();
		}
	}

	void Update()
	{
		if (Input.GetKey(KeyCode.Escape))
		{
			SceneStackManager.Instance.LoadPrevious();
		}

		if (!startTrackerDone)
		{
			TrackerManager.GetInstance().StartTracker(TrackerManager.TRACKER_TYPE_IMAGE);
			startTrackerDone = true;
		}

		DisableAllTrackables();

		TrackingState state = TrackerManager.GetInstance().UpdateTrackingState();
		TrackingResult trackingResult = state.GetTrackingResult();

		for (int i = 0; i < trackingResult.GetCount(); i++)
		{
			Trackable trackable = trackingResult.GetTrackable(i);
			imageTrackablesMap[trackable.GetName()].OnTrackSuccess(
				trackable.GetId(), trackable.GetName(), trackable.GetPose());
		}
	}

	public void SetNormalMode()
	{
		TrackerManager.GetInstance().SetTrackingOption(TrackerManager.TrackingOption.NORMAL_MODE);
	}

	public void SetExtendedMode()
	{
		TrackerManager.GetInstance().SetTrackingOption(TrackerManager.TrackingOption.EXTEND_MODE);
	}

	public void SetMultiMode()
	{
		TrackerManager.GetInstance().SetTrackingOption(TrackerManager.TrackingOption.MULTI_MODE);
	}

	void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			TrackerManager.GetInstance().StopTracker();
		}
	}

	void OnDestroy()
	{
		imageTrackablesMap.Clear();
		TrackerManager.GetInstance().SetTrackingOption(TrackerManager.TrackingOption.NORMAL_MODE);
		TrackerManager.GetInstance().StopTracker();
		TrackerManager.GetInstance().DestroyTracker();
	}

	public void OnNextClicked()
	{
		SceneManager.LoadScene("MultiImageTargetB");
	}
}