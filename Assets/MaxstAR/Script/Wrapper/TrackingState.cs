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
	/// Container for individual tracking information
	/// </summary>
	public class TrackingState
	{
        public enum TrackingStatus
        {
            START = 1,
            STOP = 2,
            RECOGNITION = 3,
            TRACKING = 4,
            CLOUD_CONNECTING = 5,
            CLOUD_NETWORK_ERROR = 6,
            STATUS_UNKNOWN = 7
        }

		private ulong cPtr;

		internal TrackingState(ulong trackingStateCPtr)
		{
			cPtr = trackingStateCPtr;
		}

		internal ulong GetTrackingStateCPtr()
		{
			return cPtr;
		}

		/// <summary>Output the number of trackers that have been tracked successfully.</summary>
		/// <returns>Return trackable count.</returns>
		public TrackingResult GetTrackingResult()
		{
			ulong trackingResultCPtr;

            trackingResultCPtr = NativeAPI.TrackingState_getTrackingResult(cPtr);

            return new TrackingResult(trackingResultCPtr);
		}

		/// <summary>Outputs the recognition result of the bar code / QR code.</summary>
		/// <returns>Return code scan string</returns>
		public string GetCodeScanResult()
		{
            int length = NativeAPI.TrackingState_getCodeScanResultLength(cPtr);
            if (length > 0)
            {
                byte[] result = new byte[length];
                NativeAPI.TrackingState_getCodeScanResult(cPtr, result, length);
                return Encoding.UTF8.GetString(result);
            }
            else
            {
                return "";
            }
        }

		/// <summary>
		/// Extract image used for tracking
		/// </summary>
		/// <returns>Image instance</returns>
		public TrackedImage GetImage()
		{
			ulong Image_Cptr = 0;
            Image_Cptr = NativeAPI.TrackingState_getImage(cPtr);

            return new TrackedImage(Image_Cptr);
		}

		internal TrackedImage GetImage(bool splitYUV)
        {
            ulong Image_Cptr = 0;
            Image_Cptr = NativeAPI.TrackingState_getImage(cPtr);

            return new TrackedImage(Image_Cptr, splitYUV);
        }

        public TrackingStatus getTrackingStatus()
        {
            CloudRecognitionController cloudRecognitionController = CloudRecognitionController.Instance;

            if (cloudRecognitionController.getCloudStatus() == CloudRecognitionController.CloudState.CLOUDSTATE_START)
            {
                return TrackingStatus.START;
            }
            else if (cloudRecognitionController.getCloudStatus() == CloudRecognitionController.CloudState.CLOUDSTATE_STOP)
            {
                return TrackingStatus.STOP;
            }
            else if (cloudRecognitionController.getCloudStatus() == CloudRecognitionController.CloudState.CLOUDSTATE_TRACKING)
            {
                return TrackingStatus.TRACKING;
            }
            else if (cloudRecognitionController.getCloudStatus() == CloudRecognitionController.CloudState.CLOUDSTATE_CONNECTING)
            {
                return TrackingStatus.CLOUD_CONNECTING;
            }
            else if (cloudRecognitionController.getCloudStatus() == CloudRecognitionController.CloudState.CLOUDSTATE_CONNECT)
            {
                return TrackingStatus.CLOUD_CONNECTING;
            }
            else if (cloudRecognitionController.getCloudStatus() == CloudRecognitionController.CloudState.CLOUDSTATE_FEATURE_COLLECT_READY)
            {
                return TrackingStatus.RECOGNITION;
            }
            else
            {
                return TrackingStatus.STATUS_UNKNOWN;
            }
        }
	}
}
