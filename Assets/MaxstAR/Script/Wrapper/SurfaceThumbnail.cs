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
	/// Contains surface thumbnail image information of first keyframe
	/// </summary>
	public class SurfaceThumbnail
    {
		private ulong cPtr;

		internal SurfaceThumbnail(ulong cPtr)
		{
			this.cPtr = cPtr;
		}

		/// <summary>
		/// </summary>
		/// <returns>image width</returns>
		public int GetWidth()
		{
            return NativeAPI.SurfaceThumbnail_getWidth(cPtr);
        }

		/// <summary>
		/// </summary>
		/// <returns>image height</returns>
		public int GetHeight()
		{
            return NativeAPI.SurfaceThumbnail_getHeight(cPtr);
        }

		/// <summary>
		/// </summary>
		/// <returns>image data length</returns>
		public int GetLength()
		{
            return NativeAPI.SurfaceThumbnail_getLength(cPtr);
        }

		/// <summary>
		/// </summary>
		/// <returns>thumbnail image data buffer</returns>
		public byte [] GetData()
		{
			int length = GetLength();
			byte[] data = new byte[length];
            NativeAPI.SurfaceThumbnail_getData(cPtr, data, length);

            return data;
		}
    }
}
