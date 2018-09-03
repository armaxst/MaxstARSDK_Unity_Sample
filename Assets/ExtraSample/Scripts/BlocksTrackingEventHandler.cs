using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlocksTrackingEventHandler : TrackingEventHandler
{
	UnityEngine.Video.VideoPlayer videoPlayer;

	private void Start()
	{
		videoPlayer = GetComponentInChildren<UnityEngine.Video.VideoPlayer>();
	}

	public override void OnTrackingSuccess()
	{

		if (videoPlayer != null)
		{
			if (!videoPlayer.isPrepared)
			{
				videoPlayer.Prepare();
				return;
			}

			if (videoPlayer.isPrepared && !videoPlayer.isPlaying)
			{
				Debug.Log("Video Play");
				videoPlayer.Play();
			}
		}
	}

	public override void OnTrackingFail()
	{

		if (videoPlayer != null)
		{
			if (videoPlayer.isPlaying)
			{
				Debug.Log("Video Stop");
				videoPlayer.Pause();
			}
		}
	}
}
