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
    internal class MapViewer
    {
        private static MapViewer instance = null;

        internal static MapViewer GetInstance()
        {
            if (instance == null)
            {
                instance = new MapViewer();
            }
            return instance;
        }

        private MapViewer()
        {
        }

        internal bool Initialize(string fileName)
        {
            return NativeAPI.MapViewer_initialize(fileName);
        }

        internal void Deinitialize()
        {
            NativeAPI.MapViewer_deInitialize();
        }

        internal IntPtr GetJson()
        {
            return NativeAPI.MapViewer_getJson();
        }

        internal int Create(int idx)
        {
            return NativeAPI.MapViewer_create(idx);
        }

        internal void GetIndices(out int indices)
        {
            NativeAPI.MapViewer_getIndices(out indices);
        }

        internal void GetTexCoords(out float texCoords)
        {
            NativeAPI.MapViewer_getTexCoords(out texCoords);
        }

        internal int GetImageSize(int idx)
        {
            return NativeAPI.MapViewer_getImageSize(idx);
        }

        internal void GetImage(int idx, out byte image)
        {
            NativeAPI.MapViewer_getImage(idx, out image);
        }
    }
}