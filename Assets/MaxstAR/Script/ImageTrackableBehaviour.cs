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
		public string trackableId { get; set; }
		public string trackableName { get; set; }
		public Matrix4x4 trackablePose { get; set; }
		public bool result { get; set; }

		private TrackingEventHandler trackingEventHandler;

		private void Awake()
		{
			trackingEventHandler = GetComponent<TrackingEventHandler>();
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

			if (trackingEventHandler != null)
			{
				trackingEventHandler.OnTrackingSuccess();
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

			if (trackingEventHandler != null)
			{
				trackingEventHandler.OnTrackingFail();
			}
		}

		public void ApplyResult()
		{
			if (result == true)
			{
				OnTrackSuccess(trackableId, trackableName, trackablePose);
			}
			else
			{
				OnTrackFail();
			}
		}
    }
}