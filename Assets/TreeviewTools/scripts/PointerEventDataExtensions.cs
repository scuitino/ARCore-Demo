using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class PointerEventDataExtensions
{
    public static float Pressure(this PointerEventData instance)
    {
        for (var i = 0; i < Input.touches.Length; i++)
        {
            var touch = Input.touches[i];
            if (touch.fingerId == instance.pointerId)
                return touch.pressure;
        }

        return 1f;
    }
}