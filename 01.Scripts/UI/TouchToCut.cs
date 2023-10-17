using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchToCut : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        RunManager.instance.OnPressDownCuttingBtn();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        RunManager.instance.OnPressUpCuttingBtn();
    }
}
