using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARBehaviour : MonoBehaviour
{
	public void Awake()
	{
		BackKeyHandler backKeyHandler = BackKeyHandler.Instance;
	}

	public void OnClickBackButton()
	{
		SceneStackManager.Instance.LoadPrevious();
	}
}
