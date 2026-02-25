using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputController : MonoBehaviour
{
    public event Action<SwipeDirection> OnSwipe;

    public bool detectSwipeOnlyAfterRelease = true;
    public float SWIPE_THRESHOLD = 20f;

    private Vector2 fingerDownPos;
    private Vector2 fingerUpPos;

    private SwipeDirection lastSwipeDirection = SwipeDirection.None;

    private void Update()
    {
        CheckTouch();
    }

    private void CheckTouch()
    {
#if UNITY_EDITOR
        if (EventSystem.current.IsPointerOverGameObject()) { return; }

        if (Input.GetMouseButtonDown(0))
        {
            fingerUpPos = Input.mousePosition;
            fingerDownPos = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            if (!detectSwipeOnlyAfterRelease)
            {
                fingerDownPos = Input.mousePosition;
                CheckSwipe();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            fingerDownPos = Input.mousePosition;
            CheckSwipe();
            lastSwipeDirection = SwipeDirection.None;
        }
        return;
#else

        foreach (Touch touch in Input.touches)
        {
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId)) { return; }

            if (touch.phase == TouchPhase.Began)
            {
                fingerUpPos = touch.position;
                fingerDownPos = touch.position;
            }

            if (touch.phase == TouchPhase.Moved)
            {
                if (!detectSwipeOnlyAfterRelease)
                {
                    fingerDownPos = touch.position;
                    CheckSwipe();
                }
            }

            if (touch.phase == TouchPhase.Ended)
            {
                fingerDownPos = touch.position;
                CheckSwipe();
            }
        }
#endif
    }

    private void CheckSwipe()
    {
        Vector2 swipeDirection = fingerDownPos - fingerUpPos;
        float swipeDistance = swipeDirection.magnitude;

        if (swipeDistance > SWIPE_THRESHOLD)
        {
            if (Mathf.Abs(swipeDirection.x) > Mathf.Abs(swipeDirection.y))
            {
                SendSwipe(swipeDirection.x > 0 ? SwipeDirection.Right : SwipeDirection.Left);
            }
            else
            {
                SendSwipe(swipeDirection.y > 0 ? SwipeDirection.Up : SwipeDirection.Down);
            }

            if (detectSwipeOnlyAfterRelease)
            {
                fingerUpPos = fingerDownPos;
            }
        }
    }

    private void SendSwipe(SwipeDirection swipe)
    {
        if (lastSwipeDirection != swipe)
        {
            lastSwipeDirection = swipe;
            OnSwipe?.Invoke(lastSwipeDirection);
            //Debug.Log($"[InputController] > SendSwipe > lastSwipeDirection: {lastSwipeDirection}");
        }
    }
}