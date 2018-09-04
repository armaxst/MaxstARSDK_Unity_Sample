/*==============================================================================
Copyright 2017 Maxst, Inc. All Rights Reserved.
==============================================================================*/

using UnityEngine;
using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace maxstAR
{
    /// <summary>
    /// API call return value
    /// </summary>
    public enum ResultCode
    {
		/// <summary>
		/// Success
		/// </summary>
		Success = 0,

		/// <summary>
		/// Permission state unknown
		/// </summary>
		CameraPermissionIsNotResolved = 100,

		/// <summary>
		/// No Camera can be usable
		/// </summary>
		CameraDevicedRestriced = 101,

		/// <summary>
		/// Camera permission is not grated
		/// </summary>
		CameraPermissionIsNotGranted = 102,

		/// <summary>
		/// Camera is alreay opened
		/// </summary>
		CameraAlreadyOpened = 103,

		/// <summary>
		/// Camera access exception
		/// </summary>
		CameraAccessException = 104,

		/// <summary>
		/// Camera not exist
		/// </summary>
		CameraNotExist = 105,

		/// <summary>
		/// Camera open timeout
		/// </summary>
		CameraOpenTimeOut = 106,

		/// <summary>
		/// Flash light is not supported
		/// </summary>
		FlashLightUnsupported = 107,

		/// <summary>
		/// Tracker already started
		/// </summary>
		TrackerAlreadyStarted = 200,

		/// <summary>
		/// Unknown error
		/// </summary>
		UnknownError = 1000,
    }
}