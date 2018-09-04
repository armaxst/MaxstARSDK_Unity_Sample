/*==============================================================================
Copyright 2017 Maxst, Inc. All Rights Reserved.
==============================================================================*/

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace maxstAR
{
	/// <summary>
	/// Contains tracked targets informations
	/// </summary>
    public class TrackingResult
    {
		private ulong cPtr;

		internal TrackingResult(ulong cPtr)
		{
			this.cPtr = cPtr;
		}

		/// <summary>
		/// Get tracking target count. Current version ar engine could not track multi target.
		/// That feature will be implemented not so far future.
		/// </summary>
		/// <returns>tracking target count</returns>
		public int GetCount()
		{
            return NativeAPI.TrackingResult_getCount(cPtr);
        }

		/// <summary>
		/// Get tracking target information
		/// </summary>
		/// <param name="index">target index</param>
		/// <returns>Trackable class instance</returns>
		public Trackable GetTrackable(int index)
		{
			Trackable t;
            t = new Trackable(NativeAPI.TrackingResult_getTrackable(cPtr, index));

            return t;
		}
    }
}
