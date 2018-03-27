/*==============================================================================
Copyright 2017 Maxst, Inc. All Rights Reserved.
==============================================================================*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace maxstAR
{
	public class SceneManager
	{
		private static SceneManager instance = new SceneManager();

		public static SceneManager Instance
		{
			get { return instance; }
		}

		private SceneManager()
		{
		}

		public void SceneUpdated()
		{
			SceneUpdatedInternal();
		}

		public static void SceneUpdatedInternal()
		{
			AbstractTrackableBehaviour[] trackables =
				(AbstractTrackableBehaviour[])UnityEngine.Object.FindObjectsOfType(typeof(AbstractTrackableBehaviour));

			CheckForDuplicates(trackables);
		}

		private static void CheckForDuplicates(AbstractTrackableBehaviour[] trackables)
		{
			for (int i = 0; i < trackables.Length; ++i)
			{
				string nameA = trackables[i].TrackableName;
				for (int j = i + 1; j < trackables.Length; ++j)
				{
					if (trackables[i].TrackableId == null || trackables[i].TrackableId.Length == 0)
					{
						continue;
					}

					if (trackables[i].TrackableId == trackables[j].TrackableId)
					{
						Debug.LogError("Duplicate Trackables detected: " + nameA);
					}
				}
			}
		}
	}
}