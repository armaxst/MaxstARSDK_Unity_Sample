/*==============================================================================
Copyright 2017 Maxst, Inc. All Rights Reserved.
==============================================================================*/

using System;
using UnityEngine;
using System.Runtime.InteropServices;

namespace maxstAR
{
	/// <summary>
	/// Matrix converting utililty class
	/// </summary>
	public class MatrixUtils
	{
		internal static Matrix4x4 ConvertGLMatrixToUnityMatrix4x4(float[] glMatrix)
		{
			Matrix4x4 matrix = Matrix4x4.zero;
			matrix[0, 0] = glMatrix[0];
			matrix[1, 0] = glMatrix[1];
			matrix[2, 0] = glMatrix[2];
			matrix[3, 0] = glMatrix[3];

			matrix[0, 1] = glMatrix[4];
			matrix[1, 1] = glMatrix[5];
			matrix[2, 1] = glMatrix[6];
			matrix[3, 1] = glMatrix[7];

			matrix[0, 2] = glMatrix[8];
			matrix[1, 2] = glMatrix[9];
			matrix[2, 2] = glMatrix[10];
			matrix[3, 2] = glMatrix[11];

			matrix[0, 3] = glMatrix[12];
			matrix[1, 3] = glMatrix[13];
			matrix[2, 3] = glMatrix[14];
			matrix[3, 3] = glMatrix[15];
			return matrix;
		}

		internal static Matrix4x4 ConvertGLProjectionToUnityProjection(float[] projection)
		{
			Matrix4x4 unityProjection = new Matrix4x4();

			unityProjection[0, 0] = projection[0];    // x
			unityProjection[1, 0] = projection[1];    // x
			unityProjection[2, 0] = projection[2];
			unityProjection[3, 0] = projection[3];

			unityProjection[0, 1] = -projection[4];   // y
			unityProjection[1, 1] = -projection[5];   // y
			unityProjection[2, 1] = -projection[6];
			unityProjection[3, 1] = -projection[7];

			unityProjection[0, 2] = -projection[8];
			unityProjection[1, 2] = -projection[9];
			unityProjection[2, 2] = -projection[10];   // z
			unityProjection[3, 2] = -projection[11];   // z

			unityProjection[0, 3] = projection[12];
			unityProjection[1, 3] = projection[13];
			unityProjection[2, 3] = projection[14];
			unityProjection[3, 3] = projection[15];
			return unityProjection;
		}

		internal static Matrix4x4 GetUnityPoseMatrix(float[] glMatrix)
		{
			Matrix4x4 unityMatrix = ConvertGLMatrixToUnityMatrix4x4(glMatrix);
			return GetUnityPoseMatrix(unityMatrix);
		}

		internal static Matrix4x4 GetUnityPoseMatrix(Matrix4x4 matrix)
		{
			Quaternion q = QuaternionFromMatrix(matrix);
			Vector3 tempEuler = q.eulerAngles;
			tempEuler.x = -tempEuler.x;
			tempEuler.z = -tempEuler.z;
			return Matrix4x4.TRS(new Vector3(matrix.m03, -matrix.m13, matrix.m23), Quaternion.Euler(tempEuler), new Vector3(1.0f, 1.0f, 1.0f));    //Translate + Quaternion + Scale = Matrix4x4
		}

		internal static void ApplyLocalTransformFromMatrix(Transform transform, Matrix4x4 matrix)
		{
			transform.localScale = ScaleFromMatrix(matrix);
			transform.localRotation = QuaternionFromMatrix(matrix);
			transform.localPosition = PositionFromMatrix(matrix);
		}

		internal static Quaternion InvQuaternionFromMatrix(Matrix4x4 input)
	{
			Quaternion q = QuaternionFromMatrix(input);
			q.w = -q.w;
			return q;
		}

		/// <summary>
		/// Get position factor from matrix
		/// </summary>
		/// <param name="input">Unity Matrix4x4</param>
		/// <returns>postion</returns>
		public static Vector3 PositionFromMatrix(Matrix4x4 input)
		{
			return new Vector3(input.m03, input.m13, input.m23);
		}

		/// <summary>
		/// Get scale factor from matrix
		/// </summary>
		/// <param name="input">Untiy Matrix4x4</param>
		/// <returns>scale</returns>
		public static Vector3 ScaleFromMatrix(Matrix4x4 input)
		{
			float x = Mathf.Sqrt(input.m00 * input.m00 + input.m01 * input.m01 + input.m02 * input.m02);
			float y = Mathf.Sqrt(input.m10 * input.m10 + input.m11 * input.m11 + input.m12 * input.m12);
			float z = Mathf.Sqrt(input.m20 * input.m20 + input.m21 * input.m21 + input.m22 * input.m22);
			return new Vector3(x, y, z);
		}

		internal static Matrix4x4 MatrixFromQuaternionAndTranslate(Quaternion qua, Vector3 pos)
		{
			Matrix4x4 result = MatrixFromQuaternion(qua);
			result[12] = pos.x;
			result[13] = pos.y;
			result[14] = pos.z;
			return result;
		}

		/// <summary>
		/// Get Matrix4x4 from quaternion
		/// </summary>
		/// <param name="input">Quaternion</param>
		/// <returns>Unity Matrix4x4</returns>
		public static Matrix4x4 MatrixFromQuaternion(Quaternion input)
		{
			float qx = input.x;
			float qy = input.y;
			float qz = input.z;
			float qw = input.w;

			Matrix4x4 result = new Matrix4x4();
			result[0] = 1 - 2 * qy * qy - 2 * qz * qz;
			result[1] = 2 * qx * qy + 2 * qz * qw;
			result[2] = 2 * qx * qz - 2 * qy * qw;
			result[3] = 0;

			result[4] = 2 * qx * qy - 2 * qz * qw;
			result[5] = 1 - 2 * qx * qx - 2 * qz * qz;
			result[6] = 2 * qy * qz + 2 * qx * qw;
			result[7] = 0;

			result[8] = 2 * qx * qz + 2 * qy * qw;
			result[9] = 2 * qy * qz - 2 * qx * qw;
			result[10] = 1 - 2 * qx * qx - 2 * qy * qy;
			result[11] = 0;

			result[12] = 0;
			result[13] = 0;
			result[14] = 0;
			result[15] = 1;
			return result;
		}

		/// <summary>
		/// Get orientation from matrix
		/// </summary>
		/// <param name="m">unity matrix</param>
		/// <returns>orientation</returns>
		public static Quaternion QuaternionFromMatrix(Matrix4x4 m)
		{
			return Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1));
		}
	}
}