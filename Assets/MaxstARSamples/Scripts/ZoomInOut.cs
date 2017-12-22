using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomInOut : MonoBehaviour {

    float originalScale = 1.0f;
    float savedScale = 0.5f;
    float beforeRenderStateSavedScale = 0.0f;
    float renderStateSavedScale = 0.0f;

    bool firstTouch = false;
    float firstTouchScale = 0.0f;

    public float minimumScale = 0.01f;
    public float maximumScale = 1.0f;

    public float scaleSpeed = 0.001f;

    private bool isScaleState = false;
    private bool isInitialized = false;

    private Vector3 initializeScaleVector;

    public bool getScaleState()
    {
        return isScaleState;
    }

    private void Start()
    {
        isInitialized = true;
        initializeScaleVector = transform.localScale;
        originalScale = transform.localScale.x;
        savedScale = transform.localScale.x;
    }

    public void initilizzeScale()
    {
        if (isInitialized)
        {
            transform.localScale = initializeScaleVector;
        }
    }

    void Update()
    {
        if (gameObject.transform.localScale.x < minimumScale)
        {
			if (initializeScaleVector.x > 0.0f)
			{
				transform.localScale = initializeScaleVector;
			}  
        }


        if (gameObject.activeSelf && Input.touchCount == 2)
        {
            isScaleState = true;
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            float lineScalePow = Mathf.Pow((touchZero.position.x - touchOne.position.x), 2) + Mathf.Pow((touchZero.position.y - touchOne.position.y), 2);
            float lineScale = Mathf.Sqrt(lineScalePow);

			if (!firstTouch)
			{
                firstTouch = true;
                firstTouchScale = lineScale;
			}

            lineScale =  (float)((lineScale - firstTouchScale) * scaleSpeed);

            if (savedScale + lineScale + renderStateSavedScale > minimumScale && savedScale + lineScale + renderStateSavedScale < maximumScale)
            {
                transform.localScale = new Vector3(savedScale + lineScale + renderStateSavedScale, savedScale + lineScale + renderStateSavedScale, savedScale + lineScale + renderStateSavedScale);
                beforeRenderStateSavedScale = lineScale + renderStateSavedScale;
            }
        }
        else if(gameObject.activeSelf && Input.touchCount == 0)
        {
            isScaleState = false;
            firstTouch = false;
            firstTouchScale = 0.0f;
            renderStateSavedScale = beforeRenderStateSavedScale;
			
        }
        else if(gameObject.activeSelf == false && isInitialized)
        {
            isScaleState = false;
			firstTouch = false;
			firstTouchScale = 0.0f;
            savedScale = originalScale;
            transform.localScale = new Vector3(originalScale , originalScale, originalScale );
            renderStateSavedScale = 0.0f;
        }
    }
}
