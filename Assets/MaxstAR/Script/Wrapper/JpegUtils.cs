/*==============================================================================
Copyright 2017 Maxst, Inc. All Rights Reserved.
==============================================================================*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace maxstAR
{
	internal class JpegUtils
	{
		public static Dimensions GetJpegDimensions(byte[] bytes)
		{
			using (var ms = new MemoryStream(bytes))
			{
				return GetJpegDimensions(ms);
			}
		}

		public static Dimensions GetJpegDimensions(string filePath)
		{
			using (var fs = File.OpenRead(filePath))
			{
				return GetJpegDimensions(fs);
			}
		}

		public static Dimensions GetJpegDimensions(Stream fs)
		{
			if (!fs.CanSeek) throw new ArgumentException("Stream must be seekable");
			long blockStart;
			var buf = new byte[4];
			fs.Read(buf, 0, 4);
			if (buf.SequenceEqual(new byte[] { 0xff, 0xd8, 0xff, 0xe0 }))
			{
				blockStart = fs.Position;
				fs.Read(buf, 0, 2);
				var blockLength = ((buf[0] << 8) + buf[1]);
				fs.Read(buf, 0, 4);
				if (Encoding.ASCII.GetString(buf, 0, 4) == "JFIF"
					&& fs.ReadByte() == 0)
				{
					blockStart += blockLength;
					while (blockStart < fs.Length)
					{
						fs.Position = blockStart;
						fs.Read(buf, 0, 4);
						blockLength = ((buf[2] << 8) + buf[3]);
						if (blockLength >= 7 && buf[0] == 0xff && buf[1] == 0xc0)
						{
							fs.Position += 1;
							fs.Read(buf, 0, 4);
							var height = (buf[0] << 8) + buf[1];
							var width = (buf[2] << 8) + buf[3];
							return new Dimensions(width, height);
						}
						blockStart += blockLength + 2;
					}
				}
			}
			return null;
		}
	}
}
