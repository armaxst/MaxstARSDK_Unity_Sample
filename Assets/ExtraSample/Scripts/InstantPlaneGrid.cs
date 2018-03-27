using UnityEngine;
using System.Collections;

public class InstantPlaneGrid
{
	private float lineSpacing = 0.1f;
	private int subSplit = 2;
	private int numOfLines = 15;
	private Color lineColor = new Color(1, 1, 1, 1);
	private bool enableDrawing = false;
	private Material lineMaterial;

	public InstantPlaneGrid(Material lineMaterial)
	{
		this.lineMaterial = lineMaterial;
	}

	public void EnableDrawing(bool enableDrawing)
	{
		this.enableDrawing = enableDrawing;
	}

    public void Draw(Matrix4x4 pose)
	{
		if (!enableDrawing)
		{
			return;
		}

		lineMaterial.SetPass(0);

		GL.PushMatrix();
		//GL.MultMatrix(transform.localToWorldMatrix);
        GL.MultMatrix(pose);
		float fMinDist = -lineSpacing * numOfLines;
		float fMaxDist = lineSpacing * numOfLines;
		float fMain;

		int m;
		int s;

		GL.Begin(GL.LINES);
		GL.Color(lineColor);

		for (m = 0; m <= numOfLines; m++)
		{
			fMain = m * lineSpacing;

			GL.Vertex3(fMinDist, 0, fMain);
			GL.Vertex3(fMaxDist, 0, fMain);
			GL.Vertex3(fMinDist, 0, -fMain);
			GL.Vertex3(fMaxDist, 0, -fMain);

			GL.Vertex3(fMain, 0, fMinDist);
			GL.Vertex3(fMain, 0, fMaxDist);
			GL.Vertex3(-fMain, 0, fMinDist);
			GL.Vertex3(-fMain, 0, fMaxDist);
		}

		for (m = 0; m < numOfLines; m++)
		{
			fMain = m * lineSpacing;

			for (s = 1; s < subSplit; s++)
			{
				float fSub = fMain + (lineSpacing / subSplit * s);

				GL.Vertex3(fMinDist, 0, fSub);
				GL.Vertex3(fMaxDist, 0, fSub);
				GL.Vertex3(fMinDist, 0, -fSub);
				GL.Vertex3(fMaxDist, 0, -fSub);

				GL.Vertex3(fSub, 0, fMinDist);
				GL.Vertex3(fSub, 0, fMaxDist);
				GL.Vertex3(-fSub, 0, fMinDist);
				GL.Vertex3(-fSub, 0, fMaxDist);
			}
		}

		GL.End();
		GL.PopMatrix();
	}
}