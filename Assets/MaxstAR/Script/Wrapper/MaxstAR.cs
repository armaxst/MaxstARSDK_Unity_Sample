/*==============================================================================
Copyright 2017 Maxst, Inc. All Rights Reserved.
==============================================================================*/

using UnityEngine;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace maxstAR
{
	/// <summary>
	/// Set device environment
	/// </summary>
	public class MaxstAR
    {
		/// <summary>
		/// Get ar sdk version as string
		/// </summary>
		/// <returns>SDK Version</returns>
		public static string GetVersion()
		{
			byte[] versionBytes = new byte[10];
            NativeAPI.getVersion(versionBytes, versionBytes.Length);

            string versionString = Encoding.UTF8.GetString(versionBytes).TrimEnd('\0');
			return versionString;
		}

		/// <summary>
		/// Notify Surface (normally screen size) size changed
		/// </summary>
		/// <param name="surfaceWidth">surface width</param>
		/// <param name="surfaceHeight">surface height</param>
		public static void OnSurfaceChanged(int surfaceWidth, int surfaceHeight)
		{
            NativeAPI.onSurfaceChanged(surfaceWidth, surfaceHeight);
        }

		/// <summary>
		/// Notify screen orientation chagned
		/// </summary>
		/// <param name="orientation">ScreenOrientation enum value</param>
		public static void SetScreenOrientation(int orientation)
		{
            NativeAPI.setScreenOrientation(orientation);
        }
	}
}