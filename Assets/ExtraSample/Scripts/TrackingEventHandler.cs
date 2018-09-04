using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TrackingEventHandler : MonoBehaviour
{
	public abstract void OnTrackingFail();
	public abstract void OnTrackingSuccess();
}
