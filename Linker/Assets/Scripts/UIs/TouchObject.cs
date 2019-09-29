using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void TouchObjectCallBack(Vector2 worldPos);

public class TouchObject : MonoBehaviour
{
    [SerializeField]
    private int order_ = 0;
    public int Order { get { return order_; } set { order_ = value; } }

    /// <summary>
    /// 是否吞噬触碰
    /// </summary>
    [SerializeField]
    private bool isSwallowTouch_ = false;
    public bool IsSwallowTouch { get { return isSwallowTouch_; } set { isSwallowTouch_ = value; } }

    //检查触碰区域是否再节点返回内
    protected bool isTouchInTouchObject(Vector2 worldpos) {
        var rectTransform = (RectTransform)transform;
        var localPos = TransformUtils.WorldPosToNodePos(worldpos,transform);
        return (-rectTransform.sizeDelta.x / 2 < localPos.x) && (-rectTransform.sizeDelta.y / 2 < localPos.y) &&
            (localPos.x<rectTransform.sizeDelta.x/2)&&(localPos.y<rectTransform.sizeDelta.y/2);
    }

    protected bool invokeCallBack(TouchObjectCallBack cb,Vector2 worldPos) {
        if (cb!= null) {
            cb(worldPos);
            return true;
        }
        return false;
    }

    // 触碰开始，返回是否吞噬触碰的信号
    public virtual bool OnTouchBegan(Vector2 worldPos) { return false; }
    public virtual bool OnTouchMoved(Vector2 worldPos) { return false; }
    public virtual bool OnTouchEnded(Vector2 worldPos) { return false; }
    


    //创建和销毁时注册和注销触碰管理器
    private void Start()
    {
        TouchesManager.GetInstance().Add(this);
    }
    private void OnDestroy()
    {
        if (!TouchesManager.IsInstanceBeDestroyed()) { TouchesManager.GetInstance().Remove(this); }
    }
}
