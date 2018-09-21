using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using maxstAR;

public class RotationController : MonoBehaviour
{
	private Vector3 rotationVector;

	private TKRotationRecognizer rotationRecognizer = new TKRotationRecognizer();

    public bool rotationX = false;
    public bool rotationY = false;
    public bool rotationZ = false;

	private bool isRotationState = false;

    private bool isInitialize = false;

	public bool getRotationState()
	{
		return isRotationState;
	}

	private void Awake()
	{
        rotationVector = transform.localEulerAngles;
        isInitialize = true;
	}

	public void initializeRotation()
	{
        if (isInitialize)
        {
            transform.localEulerAngles = rotationVector;
        }
	}

	private void OnEnable()
	{
        rotationRecognizer = new TKRotationRecognizer();
        rotationRecognizer.gestureRecognizedEvent += (r) =>
        {
            float rotaionValue = -(r.deltaRotation * 2.0f);
            if (rotationX)
            {
                transform.Rotate(rotaionValue, 0, 0);
            }
            else if (rotationY)
            {
                transform.Rotate(0, rotaionValue, 0);
            }
            else if (rotationZ)
            {
                transform.Rotate(0, 0, rotaionValue);
            }
        };
        TouchKit.addGestureRecognizer(rotationRecognizer);
	}

	private void Update()
	{
        if (Input.touchCount == 2)
		{
			isRotationState = true;
		}
        else if (Input.touchCount == 0)
		{
			isRotationState = false;
		}
	}

	private void OnDisable()
	{
        TouchKit.removeGestureRecognizer(rotationRecognizer);
	}
}