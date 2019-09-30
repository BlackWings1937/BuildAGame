using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Drag : TouchObject
{
    //拖拽节点的开始位置
    private Vector2 startPos_;
    //触碰的初始化位置
    private Vector2 startTouchPos_;

    // 拖拽完成的回调
    private TouchObjectCallBack dragComplieCallBack_;

    //设施拖拽完成的事件
    public void SetDragComplieCallBack(TouchObjectCallBack cb) { dragComplieCallBack_ = cb; }

    //物体拖拽允许区间
    private float minX_ = 0;
    private float minY_ = 0;
    private float maxX_ = 0;
    private float maxY_ = 0;
    private bool isActiveAllowArea_ = false;
    public void SetAllowArea(float minx, float miny, float maxx, float maxy) { minX_ = minx; minY_ = miny; maxX_ = maxx; maxY_ = maxy; isActiveAllowArea_ = false; }
    public bool IsActiveAllowArea{get{ return isActiveAllowArea_; } set { isActiveAllowArea_ = value; } }


    private bool backResult_ = false;
    private bool isDragStart = false;
    public override bool OnTouchBegan(Vector2 worldPos)
    {

        backResult_ = false;
        if (isTouchInTouchObject(worldPos)) {
            startPos_ = transform.localPosition;
            startTouchPos_ = TransformUtils.WorldPosToNodePos(worldPos, transform.parent);
            isDragStart = true;
        } else {
        }
        return backResult_;
    }

    private void setLocalPosition(Transform t,Vector2 pos) {
        if (IsActiveAllowArea)
        {
            t.localPosition = new Vector3(Mathf.Min(Mathf.Max(minX_, pos.x) ,maxX_) , Mathf.Min(Mathf.Max(minY_, pos.y), maxY_), t.localPosition.z);
        }
        else {
            t.localPosition = new Vector3(pos.x, pos.y, t.localPosition.z);
        }
    }

    public override bool OnTouchMoved(Vector2 worldPos)
    {

        if (isDragStart) {
            var nowTouchPos = TransformUtils.WorldPosToNodePos(worldPos, transform.parent);
            var diff = new Vector2(nowTouchPos.x - startTouchPos_.x, nowTouchPos.y - startTouchPos_.y);
            setLocalPosition(transform, new Vector2(startPos_.x + diff.x, startPos_.y + diff.y));

            var distance = Mathf.Sqrt(diff.x * diff.x + diff.y * diff.y);
            if (distance > 0)
            {
                backResult_ = IsSwallowTouch;
            }
            return backResult_;
        }
        return false;
    }

    public override bool OnTouchEnded(Vector2 worldPos)
    {

        if (isDragStart) {
            var nowTouchPos = TransformUtils.WorldPosToNodePos(worldPos, transform.parent);
            var diff = new Vector2(nowTouchPos.x - startTouchPos_.x, nowTouchPos.y - startTouchPos_.y);
            setLocalPosition(transform, new Vector2(startPos_.x + diff.x, startPos_.y + diff.y));
            //通知事件
            invokeCallBack(dragComplieCallBack_, worldPos);
        }
        isDragStart = false;
        return false ;
    }
}
