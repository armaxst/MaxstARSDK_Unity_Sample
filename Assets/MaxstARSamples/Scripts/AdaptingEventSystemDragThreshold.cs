// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AdaptingEventSystemDragThreshold.cs" company="Supyrb">
//   Copyright (c) 2016 Supyrb. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   send@johannesdeml.com
// </author>
// --------------------------------------------------------------------------------------------------------------------

namespace Supyrb
{
    using UnityEngine;
    using UnityEngine.EventSystems;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine.UI;

    public class AdaptingEventSystemDragThreshold : MonoBehaviour
    {
        [SerializeField] private EventSystem eventSystem;
        [SerializeField] private int referenceDPI = 100;
        [SerializeField] private float referencePixelDrag = 8f;
        [SerializeField] private bool runOnAwake = true;

        void Awake()
        {
            if (runOnAwake)
            {
                UpdatePixelDrag(Screen.dpi);
            }
        }

        public void UpdatePixelDrag(float screenDpi)
        {
            if (eventSystem == null)
            {
                Debug.LogWarning("Trying to set pixel drag for adapting to screen dpi, " +
                    "but there is no event system assigned to the script", this);
            }
            eventSystem.pixelDragThreshold = Mathf.RoundToInt(screenDpi/ referenceDPI*referencePixelDrag);
                

        }
        List<Button> disableBtnList = new List<Button>();

        private void Update()
        {
            int count = Input.touchCount;

            if (count == 0)
                return;

            if (disableBtnList.Count != 0)
            {
                DisableBtnControll();
            }

            if (count < 2)
                return;

            for(int i = 1; i < count; i++)
            {
                PointerEventData eventData = new PointerEventData(EventSystem.current);
                eventData.position = Input.GetTouch(i).position;
                List<RaycastResult> raycastResults = new List<RaycastResult>();
                EventSystem.current.RaycastAll(eventData, raycastResults);
                if (raycastResults.Count > 0)
                {
                    for (int j = 0; j < raycastResults.Count; j++)
                    {
                        Button btn = raycastResults[j].gameObject.GetComponent<Button>();
                        if (btn != null)
                        {
                            disableBtnList.Add(btn);
                            btn.interactable = false;
                        }

                    }
                }
            }
          
        }

        void OnLevelWasLoaded(int level)
        {
            DisableBtnControll();
        }


        void DisableBtnControll()
        {
            for (int i = 0; i < disableBtnList.Count; i++)
            {
                disableBtnList[i].interactable = true;
            }

            disableBtnList.Clear();
        }


        void Reset()
        {
            if (eventSystem == null)
            {
                eventSystem = GetComponent<EventSystem>();
            }
        }

  
    }
}