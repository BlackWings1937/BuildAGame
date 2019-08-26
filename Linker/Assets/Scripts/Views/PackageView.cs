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

    // 内容层
   // [SerializeField]
   // private PointAt contentlayerPointAt_;

    [SerializeField]
    private Drag contentlayerDrag_;

    //重写UI注册事件
    protected override void registerViewEvent()
    {
        //if (contentlayerPointAt_ != null)
        //{
        //    contentlayerPointAt_.SetPointUpCallBack((Vector2 worldPos) => {
        //        Debug.Log("worldPos:" + worldPos.x + " y:" + worldPos.y);
        //    });
        //}

        if (contentlayerDrag_ != null) {
            contentlayerDrag_.SetAllowArea(
                contentlayerDrag_.GetComponent<RectTransform>().sizeDelta.x/2,
                contentlayerDrag_.GetComponent<RectTransform>().sizeDelta.y/2,
                - contentlayerDrag_.GetComponent<RectTransform>().sizeDelta.x / 2 + AppController.VISIBLE_SIZE.x,
                - contentlayerDrag_.GetComponent<RectTransform>().sizeDelta.y/2 + AppController.VISIBLE_SIZE.y
                );
            contentlayerDrag_.SetDragComplieCallBack((Vector2 worldPos)=> {

            });
        }
    }

    //重写UI初始化方法
    public override void init()
    {
        registerViewEvent();
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
        //isActiveUpdateTouch_ = false;
    }
    public void CloseBtnsGroup() {
    //    closeTapBtnGroups();
    }

}
