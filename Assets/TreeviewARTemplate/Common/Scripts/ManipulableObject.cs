using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ManipulableObject : MonoBehaviour
{
    [Header("General Config")]
    public BoxCollider boxCollider;
    public WorldObjects worldObjects;

    public enum HIGHLIGHT_METHOD
    {
        NONE,
        ACTIVATE_GAMEOBJECT,
        CHANGE_COLOR
    }
    public HIGHLIGHT_METHOD highlightMethod;
    public GameObject highlightGameObject;
    public Color normalColor;
    public Color highlightColor;

    [SerializeField] GameObject spriteHighlight;
    const float spriteHighlightScaleMultiplier = 0.22f;
    public bool START_ENABLED = false;

    [Header("Tap Select Config")]
    public float TAP_TIME = 0.5f;

    [Header("Move Config")]
    [Tooltip("Thresshold viewport portion to start moving")]
    public float MOVE_DIST = 0.05f;
    public bool VERTICAL_MOVEMENT_ENABLED = true;
    [Tooltip("Maximum viewport width portion to consider Vertical Movement")]
    public float VERTICAL_MOVEMENT_WIDTH_TOLERANCE = 0.025f;
    Vector3 movingStartLocalPos;
    [Tooltip("Time to get to target pos")]
    public float MOVING_SMOOTH = 0.5f;
    Vector3 movingVelocity; // for smooth

    // second finger
    int secondFingerId;
    Vector2 secondInputStartPosition;

    [Header("Scaling Config")]
    public bool SCALING_ENABLED = false;
    [Tooltip("Thresshold multiplier of original fingers distance to start scaling")]
    public float SCALING_DISTANCE_MULT_TRESHOLD = 0.5f;
    Vector3 scalingStartScale;
    [Tooltip("Time to get to target scale")]
    const float SCALING_SMOOTH = 0.5f;
    Vector3 scalingVelocity; // for smooth

    [Header("Rotation Config")]
    public bool ROTATION_ENABLED = true;
    [Tooltip("Thresshold angle to start rotating")]
    public float ROTATE_ANGLE = 25f;
    [Tooltip("Multiplier of rotation angle in screen space")]
    public float ROTATING_SENSIBILITY = 2f;
    [Tooltip("Time to get to target rotation")]
    public float ROTATING_SMOOTH = 0.5f;
    Quaternion rotatingStartRotation;
    Vector3 rotatingVelocity; // for smooth




    public enum State
    {
        DISABLED,
        READY,

        WAITING_TO_DRAG,    // tapping
        DRAGGING,
        MOVING_IN_PLANE,
        MOVING_VERTICAL,

        TWO_FINGERS,
        SCALING,            // two fingers - scale
        ROTATING,           // two fingers - rotate

        MOVING_TO_TARGET
    }
    public StateMachine<State> state = new StateMachine<State>();
    float inputStartTime;
    Vector2 inputStartPosition;
    bool wasSelectedBefore = false;
    int firstFingerId;

    Vector3 targetWorldPoint;



    public virtual void Select(bool select)
    {
        if (highlightMethod == HIGHLIGHT_METHOD.ACTIVATE_GAMEOBJECT)
        {
            highlightGameObject.SetActive(select);
        }
        else if (highlightMethod == HIGHLIGHT_METHOD.CHANGE_COLOR)
        {
            Renderer r = highlightGameObject.GetComponent<Renderer>();
            if (r != null)
            {
                foreach (Material m in r.materials)
                    m.color = select ? highlightColor : normalColor;
            }
        }

        if (spriteHighlight != null)
        {
            spriteHighlight.SetActive(select);
        }
    }

    public void RefreshSize(Vector3 size, Vector3 centerLocalPos)
    {
        if (boxCollider != null)
        {
            // Adjust Collider
            boxCollider.center = centerLocalPos;
            boxCollider.size = size;
        }

        // Adjust Highlight size
        float highlightExtraBase = 0.1f; // 10cm
        if (highlightGameObject != null)
        {
            highlightGameObject.transform.localScale = new Vector3(size.x + highlightExtraBase, highlightGameObject.transform.localScale.y, size.z + highlightExtraBase);
        }
        if (spriteHighlight != null)
        {
            float maxXZ = Mathf.Max(size.x, size.z);
            spriteHighlight.transform.localScale = new Vector3(
                (maxXZ + highlightExtraBase) * spriteHighlightScaleMultiplier,
                (maxXZ + highlightExtraBase) * spriteHighlightScaleMultiplier, // Z is in Y for a needed rotation
                spriteHighlight.transform.localScale.z
            );
        }
    }

    public void Enable(bool enable)
    {
        if (enable && state.CurrentState == ManipulableObject.State.DISABLED)
            state.SetState(ManipulableObject.State.READY);
        if (!enable && state.CurrentState != ManipulableObject.State.DISABLED)
            state.SetState(ManipulableObject.State.DISABLED);
    }

    private void Start()
    {
        if (START_ENABLED)
        {
            state.SetState(State.READY);
        }
    }

    public void OnMouseDown()
    {
        //Debug.Log("DOWN PRE First finger " + gameObject.name);
        if (state.CurrentState == State.DISABLED)
            return;
        if (Input.touchCount > 1)
            return;
        if (SceneExtension.IsAnyTouchOverUI())
            return; // Ignore if touch is over any UI element

        Debug.Log("DOWN First finger " + gameObject.name);
        if (state.CurrentState == State.READY)
        {
            // FIRST FINGER
            if (worldObjects != null)
            {
                wasSelectedBefore = (worldObjects.SelectedObject == gameObject);
                // Select it
                worldObjects.SelectedObject = gameObject;
            }
            else
            {
                wasSelectedBefore = true;
            }
            state.SetState(State.WAITING_TO_DRAG);
            inputStartTime = Time.time;
            inputStartPosition = Input.mousePosition;
            firstFingerId = -1;
            Debug.Log("furniture DOWN touches:" + Input.touchCount);
            if (Input.touchCount > 0)
            {
                Touch touch = Input.touches[0];
                Debug.Log("furniture DOWN touch id:" + touch.fingerId);
                firstFingerId = touch.fingerId;
            }
        }
    }
    public void OnMouseDrag()
    {
        if (state.CurrentState == State.DISABLED || state.CurrentState == State.READY)
            return;
        if (Input.touchCount > 1)
            return;
        if (SceneExtension.IsAnyTouchOverUI())
            return; // Ignore if touch is over any UI element

        //Debug.Log("DRAG " + gameObject.name + " inputPos: " + Input.mousePosition + "st: " + state.CurrentState.ToString());
        if (state.CurrentState == State.WAITING_TO_DRAG || state.CurrentState == State.DRAGGING)
        {
            Vector3 vpStart = Camera.main.ScreenToViewportPoint(inputStartPosition);
            Vector3 vpNow = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            if ((vpNow - vpStart).magnitude > MOVE_DIST)
            {
                movingStartLocalPos = transform.localPosition;
                movingVelocity = Vector3.zero;

                if (VERTICAL_MOVEMENT_ENABLED && Mathf.Abs(vpNow.x - vpStart.x) < VERTICAL_MOVEMENT_WIDTH_TOLERANCE)
                {
                    state.SetState(State.MOVING_VERTICAL);
                }
                else
                {
                    state.SetState(State.MOVING_IN_PLANE);
                }
            }
        }

        if (state.CurrentState == State.MOVING_IN_PLANE)
        {
            // Raycast and set new position
            UpdateMoveInPlane();
        }
        else if (state.CurrentState == State.MOVING_VERTICAL)
        {
            // Raycast and set new position
            UpdateMoveInVertical();
        }
    }
    void UpdateMoveInPlane()
    {
        // PROJECT TOUCH into local plane to get New Point
        Plane dragPlane = new Plane(transform.up, transform.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float rayDistance;
        if (dragPlane.Raycast(ray, out rayDistance))
        {
            Vector3 newPoint = ray.GetPoint(rayDistance);
            transform.position = Vector3.SmoothDamp(transform.position, newPoint, ref movingVelocity, MOVING_SMOOTH);
        }
    }
    void UpdateMoveInVertical()
    {
        // Project touch into Up vector - closest point actually...
        Vector3 pointInUp;
        Vector3 pointInRay;
        Ray touchRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        MathTools.ClosestPointsOnTwoLines(out pointInUp, out pointInRay,
            transform.position, transform.up,
            touchRay.origin, touchRay.direction
        );
        transform.position = Vector3.SmoothDamp(transform.position, pointInUp, ref movingVelocity, MOVING_SMOOTH);
    }
    public void OnMouseUp()
    {
        if (state.CurrentState == State.DISABLED || state.CurrentState == State.READY)
            return;
        if (state.CurrentState >= State.TWO_FINGERS)
            return;
        if (SceneExtension.IsAnyTouchOverUI())
            return; // Ignore if touch is over any UI element

        Debug.Log("furniture UP " + gameObject.name + " st:" + state.ToString());
        if (state.CurrentState == State.WAITING_TO_DRAG)
        {
            // TAPPED
            if (wasSelectedBefore)
            {
                // unselect
                if (worldObjects != null)
                    worldObjects.SelectedObject = null;
            }
            else
            {
                // keep it selected
            }
        }
        else if (state.CurrentState == State.MOVING_IN_PLANE)
        {
            // was moving in plane
        }
        else if (state.CurrentState == State.MOVING_VERTICAL)
        {
            // was moving in vertical
        }

        // Stop tapping/moving
        state.SetState(State.READY);
    }


    private void Update()
    {
        if (state.CurrentState == State.DISABLED)
            return;
        if (state.CurrentState == State.MOVING_TO_TARGET)
        {
            UpdateToTarget();
            return;
        }
        if (SceneExtension.IsAnyTouchOverUI())
            return; // Ignore if touch is over any UI element

        if (state.CurrentState == State.READY && (worldObjects==null || worldObjects.SelectedObject == gameObject))
        {
            if (Input.touchCount == 2)
            {
                // Allow to scale/rotate with fingers anywhere (not over item), if its selected

                inputStartTime = Time.time;
                Touch touch = Input.touches[0];
                Debug.Log("furniture 1 touch id:" + touch.fingerId);
                firstFingerId = touch.fingerId;
                inputStartPosition = touch.position;

                touch = Input.touches[1];
                Debug.Log("furniture 2 touch id:" + touch.fingerId);
                secondFingerId = touch.fingerId;
                secondInputStartPosition = touch.position;

                state.SetState(State.TWO_FINGERS);
            }
        }

        if (state.CurrentState == State.WAITING_TO_DRAG)
        {
            if (Time.time - inputStartTime > TAP_TIME)
            {
                state.SetState(State.DRAGGING);
            }
        }

        if (state.CurrentState == State.WAITING_TO_DRAG || 
            state.CurrentState == State.DRAGGING || 
            state.CurrentState == State.MOVING_IN_PLANE || 
            state.CurrentState == State.MOVING_VERTICAL)
        {
            // 1 finger down - Check for second finger
            if (Input.touchCount > 1)
            {
                // Start to track SECOND FINGER
                Touch touch = Input.touches[1];
                Debug.Log("furniture DOWN SECOND touch id:" + touch.fingerId + " touches:" + Input.touchCount);
                secondFingerId = touch.fingerId;
                secondInputStartPosition = touch.position;

                state.SetState(State.TWO_FINGERS);
            }
        }

        if (state.CurrentState >= State.TWO_FINGERS)
        {
            if (Input.touchCount != 2)
            {
                Debug.Log("TWO_FINGERS Stopped, touches now: " + Input.touchCount);
                state.SetState(State.READY);
                return;
            }

            if (state.CurrentState == State.TWO_FINGERS)
            {
                // Check to start Scale / Rotate                
                Vector2 firstPos = Input.touches[0].position;
                Vector2 secondPos = Input.touches[1].position;
                // check angle to start Rotating
                float origAngle = Vector2.SignedAngle(Vector2.up, secondInputStartPosition - inputStartPosition);
                float nowAngle = Vector2.SignedAngle(Vector2.up, secondPos - firstPos);
                // check distance to start scaling
                float origDistance = (secondInputStartPosition - inputStartPosition).magnitude;
                float nowDistance = (secondPos - firstPos).magnitude;
                if (ROTATION_ENABLED && Mathf.Abs(nowAngle - origAngle) > ROTATE_ANGLE)
                {
                    state.SetState(State.ROTATING);
                    rotatingStartRotation = transform.localRotation;
                }
                else if (SCALING_ENABLED &&
                    (nowDistance < origDistance * (1f - SCALING_DISTANCE_MULT_TRESHOLD) ||
                    nowDistance > origDistance * (1f + SCALING_DISTANCE_MULT_TRESHOLD)))
                {
                    state.SetState(State.SCALING);
                    scalingStartScale = transform.localScale;
                    scalingVelocity = Vector3.zero;
                }
            }

            if (state.CurrentState == State.SCALING)
            {
                UpdateScaling();
            }
            else if (state.CurrentState == State.ROTATING)
            {
                UpdateRotating();
            }
        }
    }
    void UpdateScaling()
    {
        if (Input.touchCount != 2)
        {
            Debug.LogError("SCALING with invalid touches: " + Input.touchCount);
            return;
        }
        Vector2 firstPos = Input.touches[0].position;
        Vector2 secondPos = Input.touches[1].position;
        // scale
        float origDistance = (secondInputStartPosition - inputStartPosition).magnitude;
        float nowDistance = (secondPos - firstPos).magnitude;
        Vector3 targetScale = scalingStartScale * (nowDistance / origDistance);
        transform.localScale = Vector3.SmoothDamp(transform.localScale, targetScale, ref scalingVelocity, SCALING_SMOOTH);
    }
    void UpdateRotating()
    {
        if (Input.touchCount != 2)
        {
            Debug.LogError("ROTATING with invalid touches: " + Input.touchCount);
            return;
        }
        Vector2 firstPos = Input.touches[0].position;
        Vector2 secondPos = Input.touches[1].position;
        // check angle to start Rotating
        float origAngle = Vector2.SignedAngle(Vector2.up, secondInputStartPosition - inputStartPosition);
        float nowAngle = Vector2.SignedAngle(Vector2.up, secondPos - firstPos);
        Quaternion targetRot = rotatingStartRotation * Quaternion.AngleAxis(-(nowAngle - origAngle) * ROTATING_SENSIBILITY, Vector3.up);
        Vector3 targetEuler = targetRot.eulerAngles;

        Vector3 smoothEuler = transform.localRotation.eulerAngles;
        smoothEuler.x = Mathf.SmoothDampAngle(smoothEuler.x, targetEuler.x, ref rotatingVelocity.x, ROTATING_SMOOTH);
        smoothEuler.y = Mathf.SmoothDampAngle(smoothEuler.y, targetEuler.y, ref rotatingVelocity.y, ROTATING_SMOOTH);
        smoothEuler.z = Mathf.SmoothDampAngle(smoothEuler.z, targetEuler.z, ref rotatingVelocity.z, ROTATING_SMOOTH);
        transform.localRotation = Quaternion.Euler(smoothEuler);
    }


    public bool CanSetTarget()
    {
        return (state.CurrentState == State.READY ||
            state.CurrentState == State.MOVING_TO_TARGET);
        // If handling touch states it can't be moved by joystick
    }
    public bool SetTarget(Vector3 targetWorldPoint)
    {
        if (!CanSetTarget())
            return false;

        state.SetState(State.MOVING_TO_TARGET);
        this.targetWorldPoint = targetWorldPoint;
        return true;
    }
    void UpdateToTarget()
    {
        //Set rotation
        transform.LookAt(targetWorldPoint, transform.up);

        //Move pos
        transform.position = Vector3.SmoothDamp(transform.position, targetWorldPoint, ref movingVelocity, MOVING_SMOOTH);
    }
    public bool ReleaseTarget()
    {
        if (state.CurrentState != State.MOVING_TO_TARGET)
            return false;

        state.SetState(State.READY);
        return true;
    }
}
