using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointAtEndCall : PointAt
{
    public override bool OnTouchBegan(Vector2 worldPos)
    {
        base.OnTouchBegan(worldPos);
        return false;
    }

    public override bool OnTouchEnded(Vector2 worldPos)
    {

        var diff = Mathf.Sqrt((worldPos.x - orignalPos_.x) * (worldPos.x - orignalPos_.x) + (worldPos.y - orignalPos_.y) * (worldPos.y - orignalPos_.y));
        if (diff <= MAX_MOVE_DIFF && isTouchMoved_ == false)
        {
            if (isTouchInTouchObject(worldPos))
            {
                invokeCallBack(pointUpCallBack_, worldPos);
                isTouchMoved_ = false;
                return IsSwallowTouch;
            }
        }

        invokeCallBack(pointCancleCallBack_, worldPos);
        isTouchMoved_ = false;
        return false;
    }
}
