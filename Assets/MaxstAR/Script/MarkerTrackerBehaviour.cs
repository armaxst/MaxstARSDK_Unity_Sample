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
	public class MarkerTrackerBehaviour : AbstractMarkerTrackableBehaviour
	{
        [SerializeField]
        private int markerID = 0;

        [SerializeField]
        private float markerSize = 1.0f;

        public int MarkerID
        {
            get
            {
                return markerID;
            }

            set
            {
                markerID = value;
            }
        }

        public float MarkerSize
        {
            get
            {
                return markerSize;
            }

            set
            {
                markerSize = value;
            }
        }

        public void SetMarkerTrackerFileName(int id, float size) {
            string temp = "id" + id.ToString() + " : " + size.ToString();
          //  Debug.Log("0 : " + temp);
           // temp = " : " + size.ToString();
           // Debug.Log("1 : " + temp);
     
            TrackerDataFileName = "id" + id.ToString() + " : " + size.ToString();
            Debug.Log(TrackerDataFileName);
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
		}
	}
}
