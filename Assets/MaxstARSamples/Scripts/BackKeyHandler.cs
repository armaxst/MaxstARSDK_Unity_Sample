/*==============================================================================
Copyright 2017 Maxst, Inc. All Rights Reserved.
==============================================================================*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackKeyHandler : Singleton<BackKeyHandler>
{
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			SceneStackManager.Instance.LoadPrevious();
		}
	}
}