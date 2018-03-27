/*==============================================================================
Copyright 2017 Maxst, Inc. All Rights Reserved.
==============================================================================*/

using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SceneStackManager 
{
	private Stack<string> sceneStack;

	private static SceneStackManager instance = new SceneStackManager();

	private SceneStackManager()
	{
		sceneStack = new Stack<string>();
	}

	public static SceneStackManager Instance
	{
		get { return instance; }
	}

	public void LoadScene(string previousName, string nextName)
	{
		sceneStack.Push(previousName);
		SceneManager.LoadScene(nextName);
	}

	public void LoadPrevious()
	{
		if (sceneStack.Count > 0)
		{
			SceneManager.LoadScene(sceneStack.Pop());
		}
		else
		{
			Debug.Log("Last scene");
		}
	}

}
