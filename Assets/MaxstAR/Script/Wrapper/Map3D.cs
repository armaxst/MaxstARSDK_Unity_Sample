/*==============================================================================
Copyright 2017 Maxst, Inc. All Rights Reserved.
==============================================================================*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace maxstAR
{
    internal class Map3D
    {
        public int width = 0;
        public int height = 0;
        public int imageCount = 0;
        public int vertexCount = 0;
        public float[][] poseMatrices = null;
        public Point3Df[] vertices = null;
    }
}
