/*==============================================================================
Copyright 2017 Maxst, Inc. All Rights Reserved.
==============================================================================*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

namespace maxstAR
{
    /// <summary>
    /// Define constant values
    /// </summary>
    public class MaxstARUtils
    {

        // Path
        internal const string ImageTargetTexturePath = "Assets/Editor/MaxstAR/Textures";
        internal const string MarkerTargetTexturePath = "Assets/Editor/MaxstAR/Marker";

		internal static bool IsDirectXAPI()
		{
			return (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D9 ||
				SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D12 ||
				SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D11);
		}

		internal const int NATIVE_RENDER_EVENT_DRAW_BG_GL = 0x10000;
		internal const int NATIVE_RENDER_EVENT_DRAW_BG_DX = 0x10001;
		internal const int NATIVE_RENDER_EVENT_DRAW_ZOMBIE_GL = 0x20000;
		internal const int NATIVE_RENDER_EVENT_DRAW_ZOMBIE_DX = 0x20001;
    }
}
