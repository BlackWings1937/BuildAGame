using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PointAt : TouchObject
{

    // 记录点击时的世界点
    private Vector2 orignalPos_;
    // 单点最后偏移值
    private readonly static float MAX_MOVE_DIFF = 2F; 

    public override bool OnTouchBegan(Vector2 worldPos)
    {
        orignalPos_ = worldPos;
        if (isTouchInTouchObject(worldPos)) {
            invokeCallBack(pointDownCallBack_,worldPos);
            return IsSwallowTouch;
        } else {
            return false;
        } 
    }
    public override void OnTouchMoved(Vector2 worldPos)
    {

        base.OnTouchMoved(worldPos);
    }
    public override void OnTouchEnded(Vector2 worldPos)
    {
        var diff = Mathf.Sqrt((worldPos.x-orignalPos_.x)* (worldPos.x - orignalPos_.x) + (worldPos.y - orignalPos_.y) * (worldPos.y - orignalPos_.y));
        if (diff<=MAX_MOVE_DIFF)
        {
            invokeCallBack(pointUpCallBack_, worldPos);
        }
        else
        {
            invokeCallBack(pointCancleCallBack_, worldPos);
        }
    }


    private TouchObjectCallBack pointDownCallBack_ = null;
    private TouchObjectCallBack pointUpCallBack_ = null;
    private TouchObjectCallBack pointCancleCallBack_ = null;


    public void SetPointDownCallBack(TouchObjectCallBack cb) { pointDownCallBack_ = cb; }
    public void SetPointUpCallBack(TouchObjectCallBack cb) { pointUpCallBack_ = cb; }
    public void SetPointCancleCallBack(TouchObjectCallBack cb ) { pointCancleCallBack_ = cb; }


}
