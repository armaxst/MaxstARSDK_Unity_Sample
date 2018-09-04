/*==============================================================================
Copyright 2017 Maxst, Inc. All Rights Reserved.
==============================================================================*/

using UnityEngine;
using System.IO;
using JsonFx.Json;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine.Rendering;

namespace maxstAR
{
	/// <summary>
	/// Serve 3dmap file recognition and tracking
	/// </summary>
	public abstract class AbstractObjectTrackableBehaviour : AbstractTrackableBehaviour
	{
		void Start()
		{
			AbstractMapViewerBehaviour mapViewerBehaviour = FindObjectOfType<AbstractMapViewerBehaviour>();
			if (mapViewerBehaviour != null)
			{
				DestroyImmediate(mapViewerBehaviour.gameObject);
			}
		}

		/// <summary>
		/// Notify 3dmap file is changed. 
		/// </summary>
		/// <param name="trackerFileName">3dmap file name</param>
		protected override void OnTrackerDataFileChanged(string trackerFileName)
		{
			if (trackerFileName == null || trackerFileName.Length == 0)
			{
				return;
			}

			TrackableId = null;
			TrackableName = Path.GetFileNameWithoutExtension(trackerFileName);
			int startIdx = trackerFileName.LastIndexOf("/") + 1;
			int endIdx = trackerFileName.LastIndexOf(".");
			if (endIdx > startIdx)
			{
				string fileName = trackerFileName.Substring(startIdx, endIdx - startIdx);
			}
		}
	}
}
