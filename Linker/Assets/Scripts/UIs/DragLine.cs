/*
 * 用来触发和绘制用户拽出来的线条效果
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragLine : MonoBehaviour
{
    // ----- 定义类型 -----
    public enum State {
        E_UP,
        E_BOTTOM,
        E_LEFT,
        E_RIGHT,
        E_MID,
        E_RD,
    }
    // ----- 私有成员 -----
    [SerializeField]
    private GPoint gpOrignal_;
    [SerializeField]
    private GPoint gpDrag_;
    [SerializeField]
    private Drag dragPoint_;

    private DrawLine l_;

    [SerializeField]
    private State state_ = State.E_MID;

    private TouchObjectCallBack cb_ = null;


    // ----- 私有方法 -----
    private void resetGpDrag() {
        gpDrag_.transform.localPosition = gpOrignal_.transform.localPosition;
    }

    // ----- 初始化方法 -----
    private void Awake()
    {
        init();
    }
    private void init() {
        registerEvent();
        if (gpOrignal_ != null && gpDrag_!=null && gpOrignal_ != gpDrag_) {
            l_ = DrawLine.Create(gpOrignal_,gpDrag_,Color.green,0.04f);
        }
    }
    private void registerEvent() {
        if (dragPoint_!= null) {
            dragPoint_.SetDragComplieCallBack((Vector2 wp)=>{
                if (cb_!= null)
                {
                    cb_(wp);
                }
                resetGpDrag();
            });
        }
    }

    private void updateOrignalPointPos() {
        var pos = Vector2.zero;
        var rt = dragPoint_.transform as RectTransform;
        /*
        switch (state_) {
            case State.E_UP:
                pos = new Vector2(0,rt.sizeDelta.y/2);
                break;
            case State.E_BOTTOM:
                pos = new Vector2(0, -rt.sizeDelta.y / 2);
                break;
            case State.E_LEFT:
                pos = new Vector2(-rt.sizeDelta.x/2,0);
                break;
            case State.E_RIGHT:
                pos = new Vector2(rt.sizeDelta.x / 2, 0);
                break;
            case State.E_MID:
                pos = Vector2.zero;
                break;
            case State.E_RD:
                pos = new Vector2(rt.sizeDelta.x / 2, -rt.sizeDelta.y / 2);
                break;
        }*/
        gpOrignal_.transform.localPosition = pos;
    }


    // ----- 对外接口 -----

    /// <summary>
    /// 显示拖拽线段
    /// </summary>
    public void On() {
        this.gameObject.SetActive(true);
        this.dragPoint_.gameObject.SetActive(true);
        this.l_.gameObject.SetActive(true);
    }

    /// <summary>
    /// 隐藏拖拽线段
    /// </summary>
    public void Off() {
        this.gameObject.SetActive(false);
        this.dragPoint_.gameObject.SetActive(false);
        this.l_.gameObject.SetActive(false);
    }

    /// <summary>
    /// 设置拖拽线段触发区域大小
    /// </summary>
    /// <param name="size"></param>
    public void SetSize(Vector2 size) {
        ((RectTransform)this.dragPoint_.transform).sizeDelta = size;
        updateOrignalPointPos();
        resetGpDrag();
    }

    /// <summary>
    /// 设置拖拽松手后的回掉方法
    /// </summary>
    /// <param name="cb"></param>
    public void SetDragComplieCallBack(TouchObjectCallBack cb) {
        cb_ = cb;
    }

}
