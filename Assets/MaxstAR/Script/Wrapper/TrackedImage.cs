using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace maxstAR
{
	/// <summary>
	/// image data which is used for tracker and rendering
	/// </summary>
	public class TrackedImage
	{
		private static byte[] data = null;

		private ulong trackedImageCPtr;
		private int index;
		private int width;
		private int height;
		private int length;
		private ColorFormat colorFormat;
		private bool splitYuv = false;

		internal TrackedImage(ulong cPtr)
		{
			if (cPtr == 0)
			{
				return;
			}

			trackedImageCPtr = cPtr;

            index = NativeAPI.TrackedImage_getIndex(trackedImageCPtr);
            width = NativeAPI.TrackedImage_getWidth(trackedImageCPtr);
            height = NativeAPI.TrackedImage_getHeight(trackedImageCPtr);
            length = NativeAPI.TrackedImage_getLength(trackedImageCPtr);
            colorFormat = (ColorFormat)NativeAPI.TrackedImage_getFormat(trackedImageCPtr);
        }

		internal TrackedImage(ulong cPtr, bool splitYuv) : this(cPtr)
		{
			this.splitYuv = splitYuv;
		}

		/// <summary>
		/// Get image index
		/// </summary>
		/// <returns>image index</returns>
		public int GetIndex()
		{
			return index;
		}

		/// <summary>
		/// Get width
		/// </summary>
		/// <returns></returns>
		public int GetWidth()
		{
			return width;
		}

		/// <summary>
		/// Get height
		/// </summary>
		/// <returns></returns>
		public int GetHeight()
		{
			return height;
		}

		/// <summary>
		/// Get length (width * height * bits per pixel)
		/// </summary>
		/// <returns></returns>
		public int GetLength()
		{
			return length;
		}

		/// <summary>
		/// Image format
		/// </summary>
		/// <returns></returns>
		public ColorFormat GetFormat()
		{
			return colorFormat;
		}

        public ulong getImageCptr() 
        {
            return trackedImageCPtr;
        }

		/// <summary>
		/// Get image data which used tracking engine
		/// </summary>
		/// <returns>Image byte array</returns>
		public byte[] GetData()
		{
			if (length == 0)
			{
				return null;
			}

			if (data == null)
			{
				data = new byte[length];
			}

            NativeAPI.TrackedImage_getData(trackedImageCPtr, data, length);

            return data;
		}

		/// <summary>
		/// Get image data which used tracking engine
		/// </summary>
		/// <returns>Image data pointer with native address</returns>

		public IntPtr GetDataPtr()
		{
			if (length == 0)
			{
				return IntPtr.Zero;
			}

			IntPtr imagePtr = IntPtr.Zero;

            if (splitYuv && colorFormat == ColorFormat.YUV420sp)
            {
                imagePtr = (IntPtr)NativeAPI.TrackedImage_getDataYuv420spSplitPtr(trackedImageCPtr);
            }
            else if (splitYuv && colorFormat == ColorFormat.YUV420_888)
            {
                imagePtr = (IntPtr)NativeAPI.TrackedImage_getDataYuv420_888SplitPtr(trackedImageCPtr);
            }
            else
            {
                imagePtr = (IntPtr)NativeAPI.TrackedImage_getDataPtr(trackedImageCPtr);
            }

            return imagePtr;
		}
	}
}
