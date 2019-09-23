using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoublePointAt : TouchObject
{
    public override bool OnTouchBegan(Vector2 worldPos)
    {
        if (isTouchInTouchObject(worldPos)) {
            return true;
        } else {
            return false;
        }
    }
    public override bool OnTouchEnded(Vector2 worldPos)
    {
        if (isTouchInTouchObject(worldPos))
        {
            pointAt();
        }
        else
        {
        }
        return false;
    }

    //等待第二次点击的标记值
    private bool isDoubleClickActive_ = false;

    //等待第二次点击的回调时间
    [SerializeField]
    private float doubleClickWaitTime_ = 0.2f;


    //等待第二次点击的回调
    private static readonly string STR_DOUBLECLICKWAITCALLBACK = "doubleClickWaitCallBack";
    private void doubleClickWaitCallBack() {
        isDoubleClickActive_ = false;
    }


    //用户点击操作
    private void pointAt() {
        // 取消之前的等待第二次点击回调
        CancelInvoke(STR_DOUBLECLICKWAITCALLBACK);
        if (isDoubleClickActive_) {
            invokeCallBack(doubleClickCallBack_,Vector2.zero);
            isDoubleClickActive_ = false;
        } else {
            isDoubleClickActive_ = true;
            Invoke(STR_DOUBLECLICKWAITCALLBACK, doubleClickWaitTime_);
        }
    }

    //double click 事件回调
    private TouchObjectCallBack doubleClickCallBack_ = null;


    //设置 double click 事件回调
    public void SetDoubleClickCallBack(TouchObjectCallBack t) {
        doubleClickCallBack_ = t;
    }
}
