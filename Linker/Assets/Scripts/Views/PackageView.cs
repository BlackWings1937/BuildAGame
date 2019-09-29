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
    [SerializeField]
    private PointAt contentlayerPointAt_;

    [SerializeField]
    private Drag contentlayerDrag_;

    //重写UI注册事件
    protected override void registerViewEvent()
    {
        if (contentlayerPointAt_ != null)
        {
            contentlayerDrag_.IsSwallowTouch = true;
            contentlayerPointAt_.SetPointUpCallBack((Vector2 worldPos) => {
                //Debug.Log("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx1");
                GetController<PackageController>().ShowTapBtnsGroup(worldPos);
            });
        }

        if (contentlayerDrag_ != null) {
            contentlayerDrag_.IsSwallowTouch = true;
            contentlayerDrag_.SetAllowArea(
                contentlayerDrag_.GetComponent<RectTransform>().sizeDelta.x/2,
                contentlayerDrag_.GetComponent<RectTransform>().sizeDelta.y/2,
                - contentlayerDrag_.GetComponent<RectTransform>().sizeDelta.x / 2 + AppController.VISIBLE_SIZE.x,
                - contentlayerDrag_.GetComponent<RectTransform>().sizeDelta.y/2 + AppController.VISIBLE_SIZE.y
                );
            contentlayerDrag_.SetDragComplieCallBack((Vector2 worldPos)=> {
               // Debug.Log("drag worldPos:" + worldPos.x + " y:" + worldPos.y);
            });
        }
    }

    //重写UI初始化方法
    public override void init()
    {
        registerViewEvent();
    }

    private List<SceneNode> listOfScenesNode_ = new List<SceneNode>();
    public override void UpdateView(object data)
    {

        PackageInfoData pData = data as PackageInfoData;
        Debug.Log("update view..");

        List<SceneNodeData> scenesList = pData.ScenesList;

        //清理之前创建出来的节点
        var count = listOfScenesNode_.Count;
        for (int i = 0;i<count;++i) {
            var sceneNode = listOfScenesNode_[i];
            sceneNode.Dispose();
            GameObject.Destroy(sceneNode);
        }

        //清理之前创建的连接线段
        Debug.Log("scenesListCount:"+scenesList.Count);
        //生成新的节点
        //count = scenesList.Length;
        for (int i = 0;i<scenesList.Count;++i) {
            var sceneData = scenesList[i];
            var sceneNode = SceneNode.CreateSceneByData(sceneData,contentLayer_);
            listOfScenesNode_.Add(sceneNode);
        }
        //生成新的连接线段
    }

    // ----- 对外接口 -----
    public void ShowBtnsGroupByDic(Vector2 touchWorldPos, Dictionary<string, TapButtonCallBack> dic) {
        if (m_tapBtnsGroups_ != null)
        {
            if (m_tapBtnsGroups_.gameObject.activeSelf == false)
            {
                m_tapBtnsGroups_.SetEventByDic(dic);
                Vector2 worldPos = touchWorldPos;
                Vector2 nodePoint = TransformUtils.WorldPosToNodePos(worldPos, cuiLayer_);
                m_tapBtnsGroups_.gameObject.GetComponent<TapButtonsGroup>().SetPosition(new Vector3(nodePoint.x, nodePoint.y, m_tapBtnsGroups_.transform.position.z));
                m_tapBtnsGroups_.gameObject.SetActive(true);
            }
            else {
                m_tapBtnsGroups_.gameObject.SetActive(false);
            }
        }
    }
    public void CloseBtnsGroup() {
    //    closeTapBtnGroups();
    }

}
