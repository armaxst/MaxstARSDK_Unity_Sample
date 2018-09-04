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
	public class Trackable
    {
		private ulong cPtr;
		private float[] glPoseMatrix = new float[16];
		private byte[] idBytes = new byte[100];
		private byte[] nameBytes = new byte[100];
        private byte[] cloudNameBytes = new byte[100];
        private byte[] cloudMetaBytes = new byte[1024*1024];

		internal Trackable(ulong cPtr)
		{
			this.cPtr = cPtr;
		}

		/// <summary></summary>
		/// <returns>tracking target id</returns>
		public string GetId()
		{
			Array.Clear(idBytes, 0, idBytes.Length);
            NativeAPI.Trackable_getId(cPtr, idBytes);

            return Encoding.UTF8.GetString(idBytes).TrimEnd('\0');
		}

		/// <summary>
		/// </summary>
		/// <returns>tracking target name (map file name without extension)</returns>
		public string GetName()
		{
			Array.Clear(nameBytes, 0, nameBytes.Length);
            NativeAPI.Trackable_getName(cPtr, nameBytes);

            return Encoding.UTF8.GetString(nameBytes).TrimEnd('\0');
		}

        public string GetCloudName()
        {
            Array.Clear(cloudNameBytes, 0, cloudNameBytes.Length);
            NativeAPI.Trackable_getCloudName(cPtr, cloudNameBytes);

            return Encoding.UTF8.GetString(cloudNameBytes).TrimEnd('\0');
        }

        public string GetCloudMeta()
        {
            Array.Clear(cloudMetaBytes, 0, cloudMetaBytes.Length);
            int[] cloudMetaLength = new int[1];
            NativeAPI.Trackable_getCloudMeta(cPtr, cloudMetaBytes, cloudMetaLength);

            return Encoding.UTF8.GetString(cloudMetaBytes, 0, cloudMetaLength[0]).TrimEnd('\0');
        }

		/// <summary></summary>
		/// <returns>tracking pose matrix</returns>
		public Matrix4x4 GetPose()
		{
            NativeAPI.Trackable_getPose(cPtr, glPoseMatrix);

            if (AbstractARManager.Instance.WorldCenterModeSetting == 
				AbstractARManager.WorldCenterMode.TARGET)
			{
				return Matrix4x4.identity;
			}
			else
			{
				Matrix4x4 tempPose = MatrixUtils.GetUnityPoseMatrix(glPoseMatrix);
				Quaternion rotation = Quaternion.Euler(90, 0, 0);
                Matrix4x4 m = Matrix4x4.TRS(AbstractARManager.Instance.GetARCamera().transform.position, rotation, new Vector3(1, 1, 1));

				// First. Change world matrix
				tempPose = m * tempPose;

				rotation = Quaternion.Euler(-90, 0, 0);
				m = Matrix4x4.TRS(Vector3.zero, rotation, new Vector3(1, 1, 1));

				// Second. Change local matrix
				tempPose = tempPose * m;

				return tempPose;
			}
		}

		// Get target pose for camera transform (When world center mode is TARGET)
		internal Matrix4x4 GetTargetPose()
		{
            NativeAPI.Trackable_getPose(cPtr, glPoseMatrix);

            return MatrixUtils.GetUnityPoseMatrix(glPoseMatrix);
		}

		/// <summary>
		/// Get Trackable width
		/// </summary>
		/// <returns></returns>
		public float GetWidth()
		{
            return NativeAPI.Trackable_getWidth(cPtr);
        }

		/// <summary>
		/// Get Trackable height
		/// </summary>
		/// <returns></returns>

		public float GetHeight()
		{
            return NativeAPI.Trackable_getHeight(cPtr);
        }
    }
}