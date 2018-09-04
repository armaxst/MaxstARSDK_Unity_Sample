using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace maxstAR
{
	/// <summary>
	/// Camera image format
	/// </summary>
	public enum ColorFormat
	{
		/// <summary>
		/// RGB 24
		/// </summary>
		RGB888 = 1,

		/// <summary>
		/// Same android NV21
		/// </summary>
		YUV420sp = 2,

		/// <summary>
		/// Same android NV12
		/// </summary>
		YUV420 = 3,

		/// <summary>
		/// For android camera2 support (Not used yet)
		/// </summary>
		YUV420_888 = 4,

		/// <summary>
		/// Gray 8 bit image
		/// </summary>
		GRAY8 = 5
	}
}
