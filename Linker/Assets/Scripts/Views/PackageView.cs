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

    private List<SceneNode> listOfScenesNode_ = new List<SceneNode>();
    private List<DrawLine> listOfLinesBetweenSceneNodes_ = new List<DrawLine>();


    //重写UI注册事件
    protected override void registerViewEvent()
    {
        if (contentlayerPointAt_ != null)
        {
            contentlayerDrag_.IsSwallowTouch = true;
            contentlayerPointAt_.SetPointUpCallBack((Vector2 worldPos) => {
                var pos = TransformUtils.WorldPosToNodePos(worldPos, contentlayerDrag_.transform);
                GetController<PackageController>().ShowTapBtnsGroup(worldPos, pos);
            });
        }

        if (contentlayerDrag_ != null) {
            contentlayerDrag_.IsSwallowTouch = true;
            contentlayerDrag_.SetAllowArea(
                contentlayerDrag_.GetComponent<RectTransform>().sizeDelta.x / 2,
                contentlayerDrag_.GetComponent<RectTransform>().sizeDelta.y / 2,
                -contentlayerDrag_.GetComponent<RectTransform>().sizeDelta.x / 2 + AppController.VISIBLE_SIZE.x,
                -contentlayerDrag_.GetComponent<RectTransform>().sizeDelta.y / 2 + AppController.VISIBLE_SIZE.y
                );
            contentlayerDrag_.SetDragComplieCallBack((Vector2 worldPos) => {
                // Debug.Log("drag worldPos:" + worldPos.x + " y:" + worldPos.y);
            });
        }
    }

    //重写UI初始化方法
    public override void init()
    {
        registerViewEvent();
        m_tapBtnsGroups_.gameObject.SetActive(false);
    }


    private void clearAllLinesBetweenSceneNodes() {
        var count = listOfLinesBetweenSceneNodes_.Count;
        for (int i = 0; i < count; ++i) {
            var l = listOfLinesBetweenSceneNodes_[i];
            GameObject.Destroy(l.gameObject);
        }
        listOfLinesBetweenSceneNodes_.Clear();
    }

    private void generateAllLinesBetweenSceneNodes() {
        var count = listOfScenesNode_.Count;
        for (int i = 0;i<count;++i) {
            var sn = listOfScenesNode_[i];
            sn.DrawLineToLinkerSceneNode();
        }
    }

    private void generateLineBetweenSceneNodesByPoints(GPoint gp1,GPoint gp2) {
        var l = DrawLine.Create(gp1,gp2,Color.green,0.04f);
        listOfLinesBetweenSceneNodes_.Add(l);
    }
    

   
    public override void UpdateView(object data)
    {

        PackageInfoData pData = data as PackageInfoData;

        List<SceneNodeData> scenesList = pData.ScenesList;

        // 清理创建出来的线段
        clearAllLinesBetweenSceneNodes();

        //清理之前创建出来的节点
        var count = listOfScenesNode_.Count;
        for (int i = 0;i<count;++i) {
            var sceneNode = listOfScenesNode_[i];
            sceneNode.Dispose();
            Debug.Log("destroy gameObject:"+ sceneNode.gameObject.name);
            //GameObject.Destroy(sceneNode.gameObject);
            //PrefabsFactoryManager.GetInstance().GetFactoryByPrefabName()
            SceneNode.DestroyObj(sceneNode);
        }
        listOfScenesNode_.Clear();

        //清理之前创建的连接线段
        Debug.Log("scenesListCount:"+scenesList.Count);
        //生成新的节点
        //count = scenesList.Length;
        for (int i = 0;i<scenesList.Count;++i) {
            var sceneData = scenesList[i];
            var sceneNode = SceneNode.CreateSceneByData(sceneData,contentLayer_,this);
            sceneNode.PackageView = this;
            listOfScenesNode_.Add(sceneNode);
        }
        //生成新的连接线段
        generateAllLinesBetweenSceneNodes();
    }

    private SceneNode findSceneNodeById(string id) {
        var count = listOfScenesNode_.Count;
        for (int i = 0; i < count; ++i) {
            var sn = listOfScenesNode_[i];
            var data = sn.Data;
            if (data.ID == id) {
                return sn;
            }
        }
        return null;
    }

    // ----- 对外接口 -----
    public void GenerateLineBetweenSceneNodeByGPAndID(GPoint g1,string Id) {
        var aimSceneNode = findSceneNodeById(Id);
        if (aimSceneNode != null && g1 != null) {
            var aimPoint = aimSceneNode.GetSceneHeadPoint();
            if (aimPoint != null && g1 != null) {
                generateLineBetweenSceneNodesByPoints(g1,aimPoint);
            }
        }
    }

    public void LinkerOutputToScene(OutputPortData o1,SceneNodeData d2) {
        GetController<PackageController>().LinkerOutputToScene(o1, d2);
    }

    public void BreakOutputToScene(OutputPortData o1 ) {
        SceneNodeData d2 = findSceneNodeById(o1.SceneNodeID).Data;
        GetController<PackageController>().BreakOutputToScene(o1, d2);
    }


    public void ShowTapBtnsGroupByDicAndWorldPos(Dictionary<string, TapButtonCallBack> dic, Vector2 wp)
    {
        GetController<PackageController>().ShowTapBtnsGroupByDicAndWorldPos(dic, wp);
    }


    public void RemoveSceneData(SceneNodeData data) {
        GetController<PackageController>().RemoveSceneData(data);
    }
    /// <summary>
    /// 检查是否点在某个sceneNode 中，是的化返回sceneNodeData
    /// </summary>
    /// <param name="wp"></param>
    /// <returns></returns>
    public SceneNodeData CheckSceneNodesContentWorldPos(Vector2 wp) {
        SceneNodeData data = null;
        var localpos = TransformUtils.WorldPosToNodePos(wp,contentlayerDrag_.transform);
        var count = listOfScenesNode_.Count;
        for (int i = 0;i<count;++i) {
            var ns = listOfScenesNode_[i];
            var posNow = ns.transform.localPosition;
            var sizeNow = (ns.transform as RectTransform).sizeDelta;
            Rect r = new Rect(posNow.x-sizeNow.x/2,posNow.y - sizeNow.y/2,sizeNow.x,sizeNow.y);
            if (r.Contains(localpos)) {
                data = ns.Data;
                break;
            }
        }
        return data;
    }

    /// <summary>
    /// 添加出口
    /// </summary>
    /// <param name="data"></param>
    public void AddPortToSceneNodeBySceneData(SceneNodeData data)
    {
        GetController<PackageController>().AddPortToSceneNodeBySceneData(data);
    }

    /// <summary>
    /// 显示点击菜单栏
    /// </summary>
    /// <param name="touchWorldPos"></param>
    /// <param name="dic"></param>
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

    public void SaveData() {
        GetController<PackageController>().SaveData();
    }

}
