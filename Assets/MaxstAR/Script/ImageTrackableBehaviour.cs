/*==============================================================================
Copyright 2017 Maxst, Inc. All Rights Reserved.
==============================================================================*/

using UnityEngine;
using System.IO;
using JsonFx.Json;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using UnityEngine.Rendering;

namespace maxstAR
{
    public class ImageTrackableBehaviour : AbstractImageTrackableBehaviour
    {
		UnityEngine.Video.VideoPlayer videoPlayer;

		void Start()
		{
			videoPlayer = GetComponentInChildren<UnityEngine.Video.VideoPlayer>();
		}

		public override void OnTrackSuccess(string id, string name, Matrix4x4 poseMatrix)
        {
			Renderer[] rendererComponents = GetComponentsInChildren<Renderer>(true);
			Collider[] colliderComponents = GetComponentsInChildren<Collider>(true);

			// Enable renderers
			foreach (Renderer component in rendererComponents)
			{
				component.enabled = true;
			}

			// Enable colliders
			foreach (Collider component in colliderComponents)
			{
				component.enabled = true;
			}

			transform.position = MatrixUtils.PositionFromMatrix(poseMatrix);
			transform.rotation = MatrixUtils.QuaternionFromMatrix(poseMatrix);

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

        public override void OnTrackFail()
        {
			Renderer[] rendererComponents = GetComponentsInChildren<Renderer>(true);
			Collider[] colliderComponents = GetComponentsInChildren<Collider>(true);

			// Disable renderer
			foreach (Renderer component in rendererComponents)
			{
				component.enabled = false;
			}

			// Disable collider
			foreach (Collider component in colliderComponents)
			{
				component.enabled = false;
			}

			if (videoPlayer != null)
			{
				if (videoPlayer.isPlaying)
				{
					Debug.Log("Video Stop");
					videoPlayer.Stop();
				}
			}
        }
    }
}