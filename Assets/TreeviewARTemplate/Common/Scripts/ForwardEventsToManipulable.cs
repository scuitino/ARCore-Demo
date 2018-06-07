using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwardEventsToManipulable : MonoBehaviour
{
    // This is used in Collider gameobject, if different of Manipulable

    [SerializeField] ManipulableObject manipulable;

    public void OnMouseDown()
    {
        manipulable.OnMouseDown();
    }
    public void OnMouseUp()
    {
        manipulable.OnMouseUp();
    }
    public void OnMouseDrag()
    {
        manipulable.OnMouseDrag();
    }
}
