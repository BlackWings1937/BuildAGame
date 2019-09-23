using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointAtAndWait : TouchObject
{
    public override bool OnTouchBegan(Vector2 worldPos)
    {
        CancelInvoke("invokeEvent");
        return true;
    }
    public override bool OnTouchMoved(Vector2 worldPos)
    {
        if (!isTouchInTouchObject(worldPos))
        {
            CancelInvoke("invokeEvent");
        }
        return false;
    }
    public override bool OnTouchEnded(Vector2 worldPos)
    {
        if (isTouchInTouchObject(worldPos))
        {
            Invoke("invokeEvent",waitTime_);
        }
        return false;
    }

    private void invokeEvent() {
        invokeCallBack(pointAndWaitCallBack_,new Vector2());
    }

    private TouchObjectCallBack pointAndWaitCallBack_ = null;
    public void SetPointAndWaitCallBack(TouchObjectCallBack cb) { pointAndWaitCallBack_ = cb; }

    [SerializeField]
    private float waitTime_  = 0.5f;

    public float WaitTime { get { return waitTime_; } set { waitTime_ = value; } }
}

