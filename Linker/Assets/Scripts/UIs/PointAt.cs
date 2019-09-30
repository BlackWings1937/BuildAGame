using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PointAt : TouchObject
{

    // 记录点击时的世界点
    protected Vector2 orignalPos_;
    // 单点最后偏移值
    protected readonly static float MAX_MOVE_DIFF = 2F;
    //是否移动过的标记
    protected bool isTouchMoved_ = false;
    //是否点击在区域内
    private bool isTouchInArea_ = false;
    public override bool OnTouchBegan(Vector2 worldPos)
    {
        isTouchInArea_ = false;
        isTouchMoved_ = false;
        orignalPos_ = worldPos;
        if (isTouchInTouchObject(worldPos)) {
            isTouchInArea_ = true;
            invokeCallBack(pointDownCallBack_,worldPos);
            return IsSwallowTouch;
        } else {
            isTouchInArea_ = false;
            return false;
        } 
    }
    public override bool OnTouchMoved(Vector2 worldPos)
    {

        var diff = Mathf.Sqrt((worldPos.x - orignalPos_.x) * (worldPos.x - orignalPos_.x) + (worldPos.y - orignalPos_.y) * (worldPos.y - orignalPos_.y));
        if (diff > MAX_MOVE_DIFF)
        {
            isTouchMoved_ = true;
        }
        base.OnTouchMoved(worldPos);
        return false;
    }
    public override bool OnTouchEnded(Vector2 worldPos)
    {
        if (gameObject.name == "Text123") {
            Debug.Log("point at End:" + gameObject.name);
        }
        var diff = Mathf.Sqrt((worldPos.x-orignalPos_.x)* (worldPos.x - orignalPos_.x) + (worldPos.y - orignalPos_.y) * (worldPos.y - orignalPos_.y));
        if (diff <= MAX_MOVE_DIFF && isTouchMoved_ == false)
        {
            if (isTouchInTouchObject(worldPos)) {
                invokeCallBack(pointUpCallBack_, worldPos);
                isTouchMoved_ = false;
                return false;
            }
        }

        invokeCallBack(pointCancleCallBack_,worldPos);
        isTouchMoved_ = false;
        return false;
    }


    protected TouchObjectCallBack pointDownCallBack_ = null;
    protected TouchObjectCallBack pointUpCallBack_ = null;
    protected TouchObjectCallBack pointCancleCallBack_ = null;


    public void SetPointDownCallBack(TouchObjectCallBack cb) { pointDownCallBack_ = cb; }
    public void SetPointUpCallBack(TouchObjectCallBack cb) { pointUpCallBack_ = cb; }
    public void SetPointCancleCallBack(TouchObjectCallBack cb ) { pointCancleCallBack_ = cb; }


}
