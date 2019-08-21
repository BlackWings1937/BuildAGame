/*
 * package 界面的 view
 * 
 * title: view
 * 
 * 用来管理packageView 的所有UI
 */


using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;


public class PackageView : BaseView
{

    // 内容层和ui层
    [SerializeField]
    private RectTransform contentLayer_;
    [SerializeField]
    private RectTransform uiLayer_;
    [SerializeField]
    private RectTransform cuiLayer_;

    // tapBtnsGroup
    [SerializeField]
    private TapButtonsGroup m_tapBtnsGroups_;

    //重写UI注册事件
    protected override void registerViewEvent()
    {

    }

    //重写UI初始化方法
    public override void init()
    {
        registerViewEvent();
    }

    private void Update()
    {
        updateTouch();
    }

    //指令contentLayer 根据一个向量做移动
    private void contentLayerMoveByOri(Vector2 ori) {
        contentLayer_.localPosition = new Vector3(contentLayer_.localPosition.x + ori.x, contentLayer_.localPosition.y + ori.y, contentLayer_.localPosition.z); }
    //指令contentLayer 移动到目标位置
    private void contentLayerMoveToPos(Vector2 pos) {
        // 面板上的position 是localposition
        contentLayer_.localPosition = new Vector3(pos.x,pos.y,contentLayer_.localPosition.z); }
    private void showTapBtnGroups()
    {
        Debug.Log("show btn tap group..");
        GetController<PackageController>().ShowTapBtnsGroup();
    }
    private void closeTapBtnGroups() { if (m_tapBtnsGroups_ != null) {
            m_tapBtnsGroups_.gameObject.SetActive(false);
        }
        isActiveUpdateTouch_ = true;
    }

    // 表示目前用户鼠标的状态
    private bool isMouseDown_ = false;
    private Vector2 oriPos_ = Vector2.zero;
    private Vector2 oriContentLayerPos_ = Vector2.zero;
    private static readonly float MouseMaxStaticValue = 10f;
    private bool isActiveUpdateTouch_ = true;
    //更新手指触碰
    private void updateTouch() {
        if (!isActiveUpdateTouch_) { return; }
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            isMouseDown_ = true;
            oriPos_ = Input.mousePosition;
            oriContentLayerPos_ = contentLayer_.localPosition;
        }

        if (isMouseDown_) {
            var ori = new Vector2(Input.mousePosition.x - oriPos_.x, Input.mousePosition.y - oriPos_.y);
            var newPos = new Vector2(oriContentLayerPos_.x + ori.x, oriContentLayerPos_.y + ori.y);
            contentLayerMoveToPos(newPos);
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            isMouseDown_ = false;
            var ori = new Vector2(Input.mousePosition.x - oriPos_.x, Input.mousePosition.y - oriPos_.y);

            if (Mathf.Sqrt(ori.x * ori.x + ori.y * ori.y) < MouseMaxStaticValue)
            {
                showTapBtnGroups();
            }
        }
    }

    // ----- 对外接口 -----
    public void ShowBtnsGroupByDic(Dictionary<string, TapButtonCallBack> dic) {
        if (m_tapBtnsGroups_ != null) {
            m_tapBtnsGroups_.SetEventByDic(dic);
            Vector2 pos = Input.mousePosition;
            Vector2 worldPos = TransformUtils.ScreenPosToWorldPos(pos);
            Vector2 nodePoint = TransformUtils.WorldPosToNodePos(worldPos, cuiLayer_);
            m_tapBtnsGroups_.transform.localPosition = new Vector3(nodePoint.x, nodePoint.y,m_tapBtnsGroups_.transform.position.z);
            m_tapBtnsGroups_.gameObject.SetActive(true);
        }
        isActiveUpdateTouch_ = false;
    }
    public void CloseBtnsGroup() { closeTapBtnGroups(); }

}
