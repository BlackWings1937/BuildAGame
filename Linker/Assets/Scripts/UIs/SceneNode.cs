using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SceneNode : PointParent
{
    // 节点名称输入框
    [SerializeField]
    private InputField myInputField_;

    // 出口面板
    [SerializeField]
    private VerticalTextItemGroup myOutPutBroard_;

    //package界面
    private PackageView packageView_ = null;
    public PackageView PackageView {
        get { return packageView_; }
        set { packageView_ = value; }
    }


    // 初始化自身连接点
    protected override void InitPoints()
    {
        var rt = (RectTransform)transform;
        var p1 = generatePointByLocalPos(new Vector2(0, -rt.sizeDelta.y / 2));
        listPoints_.Add(p1);
        var p2 = generatePointByLocalPos(new Vector2(rt.sizeDelta.x / 2, -rt.sizeDelta.y / 2));
        listPoints_.Add(p2);
        var p3 = generatePointByLocalPos(new Vector2(-rt.sizeDelta.x / 2, rt.sizeDelta.y / 2));
        listPoints_.Add(p3);
    }

    // 点击触发区域
    [SerializeField]
    private PointAt pointAt_ = null;

    // 显示弹出选项
    private void showTapBtns() {
        if (pv_ != null)
        {
            var dic = new Dictionary<string, TapButtonCallBack>();
            dic.Add("复制场景",()=> {
                if (pv_!= null) {
                    pv_.RemoveSceneData(data_);
                }
            });
            dic.Add("删除场景", () => { });
            var size = (transform as RectTransform).sizeDelta;
            var pos =  transform.TransformPoint(0, 0 + size.y / 2, 0);
            
            pv_.ShowTapBtnsGroupByDicAndWorldPos(dic,new Vector2(pos.x,pos.y));//
        }
    }


    // 注册触控事件
    private void registerEvent()
    {
        var btnAdapt = GetComponent<BtnAdaptText>();
        var text = btnAdapt.MyText;
        var pointAt = text.GetComponent<PointAtEndCall>();
        if (pointAt != null)
        {
            pointAt.SetPointUpCallBack((Vector2 wp) =>
            {
                Debug.Log("point end call");
                showInputField();
               
            });
            pointAt.SetPointCancleCallBack((Vector2 wp) =>
            {
                if (myInputField_.gameObject.activeSelf) {
                    setText(myInputField_.text);

                }
                closeInputField();

            });
        }

        if (pointAt_ != null) {
            pointAt_.SetPointUpCallBack((Vector2 wp)=> {
                
                showTapBtns();
            });
        }

        if (myInputField_ != null)
        {
            myInputField_.onEndEdit.AddListener((string word) =>
            {
                if (myInputField_.gameObject.activeSelf)
                {
                    setText(myInputField_.text);
                }
                closeInputField();
            });
        }
        GetComponent<Drag>().SetDragComplieCallBack((Vector2 v) => {
            Debug.Log("saveData...");
            data_.X = transform.localPosition.x;
            data_.Y = transform.localPosition.y;
            packageView_.SaveData();
        });
    }

    private void Start()
    {
        registerEvent();
        GetComponent<Drag>().IsSwallowTouch = true;
        GetComponent<BtnAdaptText>().MyText.GetComponent<PointAtEndCall>().IsSwallowTouch = true;
        //GetComponent<BtnAdaptText>().
    }

    // 绘制内部连接线段
    private List<DrawLine> listOfLines_ = new List<DrawLine>();
    private void DrawInternalLine()
    {
        if (myOutPutBroard_ != null)
        {
            var textItems = myOutPutBroard_.GetTextItems();
            for (int i = 0; i < textItems.Count; ++i)
            {
                var tlp = textItems[i].GetComponent<PointParent>();
                var l1 = DrawLine.Create(tlp.GetPointByIndex(0), tlp.GetPointByIndex(1), Color.green, 0.04f);
                var l2 = DrawLine.Create(tlp.GetPointByIndex(0), GetPointByIndex(1), Color.green, 0.04f);
                var l3 = DrawLine.Create(GetPointByIndex(0), GetPointByIndex(1), Color.green, 0.04f);
                listOfLines_.Add(l1);
                listOfLines_.Add(l2);
                listOfLines_.Add(l3);
            }
        }
    }

    // 清除内部连接线段
    private void ClearInternalLine()
    {
        for (int i = 0; i < listOfLines_.Count; ++i)
        {
            GameObject.Destroy(listOfLines_[i].gameObject);
        }
        listOfLines_.Clear();
    }

    // 更新节点连接点
    private void updatePoints()
    {
        var rt = (RectTransform)transform;
        var p1 = GetPointByIndex(0);
        var p2 = GetPointByIndex(1);
        if (p1 != null && p2 != null)
        {
            p1.transform.localPosition = new Vector2(0, -rt.sizeDelta.y / 2);
            p2.transform.localPosition = new Vector2(rt.sizeDelta.x / 2, -rt.sizeDelta.y / 2);
        }
    }


    //设置节点名称
    [SerializeField]
    private float outputBoardPadding = 30;
    private void setText(string t)
    {
        //设置节点名称
        if (t == "")
            t = "未命名场景";
        GetComponent<BtnAdaptText>().SetText(t);

        // 更新出口面板位置
        var aimX = outputBoardPadding + ((RectTransform)transform).sizeDelta.x / 2 + ((RectTransform)myOutPutBroard_.transform).sizeDelta.x / 2;
        myOutPutBroard_.transform.localPosition = new Vector3(aimX, 0, myOutPutBroard_.transform.localPosition.z);

        // 更新连接点位置
        updatePoints();

        // 更新触发器大小
        (pointAt_.transform as RectTransform).sizeDelta = (transform as RectTransform).sizeDelta;

        //重绘内部线段
        ClearInternalLine();
        DrawInternalLine();

        if (data_ != null && packageView_ != null) {
            data_.Name = t;
            packageView_.SaveData();
        }
    }

    //设置出口列表
    private void setOutPutItemList(List<OutputPortData> l)
    {
        if (myOutPutBroard_ != null)
        {
            ClearInternalLine();
            myOutPutBroard_.UpdateTextItemsByStringList(l,data_);
            DrawInternalLine();
        }
    }

    private void setPackageViewToOutPutItemList(PackageView pv) {
        if (myOutPutBroard_ != null) {
            myOutPutBroard_.SetPackageView(pv);
        }
    }

    private void infoOutPutItemDrawLines() {
        if (myOutPutBroard_ != null) {
            myOutPutBroard_.InfoItemToDrawLines();
        }
    }

    // 显示输入节点名称面板
    private void showInputField()
    {
        if (myInputField_ != null)
        {
            myInputField_.gameObject.SetActive(true);
            myInputField_.ActivateInputField();// = true;
        }
    }

    // 关闭输入节点名称面板
    private void closeInputField()
    {
        if (myInputField_ != null)
        {
            myInputField_.gameObject.SetActive(false);
        }
    }
    private PackageView pv_;
    // ----- 对外接口 -----

    /// <summary>
    /// 通过data 创建sceneNode
    /// </summary>
    /// <param name="data">用于初始化的数据</param>
    /// <param name="parent">SceneNode的母节点</param>
    /// <param name="packageView">packageView</param>
    /// <returns></returns>
    public static SceneNode CreateSceneByData(SceneNodeData data, Transform parent, PackageView pv)
    {
        
        var f = PrefabsFactoryManager.GetInstance().GetFactoryByPrefabName("SceneNode");
        if (f == null) { Debug.LogError("create scene Node fail"); return null; }
        var g = PrefabsFactoryManager.GetInstance().GetFactoryByPrefabName("SceneNode").Get();
        g.GetComponent<SceneNode>().InitSceneByData(data, parent, pv);
        return g.GetComponent<SceneNode>();
    }

    public static void DestroyObj(SceneNode n)
    {
        if (n == null) { Debug.LogError(" param scenenode is null"); return; }
        var f = PrefabsFactoryManager.GetInstance().GetFactoryByPrefabName("SceneNode");
        if (f == null) { Debug.LogError("destroy scene Node fail"); return; }
        f.Recycle(n.gameObject);
    }

    /// <summary>
    /// 清理节点管理的所有东西
    /// </summary>
    public void Dispose()
    {
        ClearInternalLine();
    }


    /// <summary>
    /// 初始化节点的data
    /// </summary>
    private SceneNodeData data_;

    public SceneNodeData Data { get { return data_; } }

    /// <summary>
    /// 初始化sceneNode
    /// </summary>
    /// <param name="data">用于初始化的数据</param>
    /// <param name="parent">SceneNode的母节点</param>
    /// <param name="packageView">packageView</param>
    public void InitSceneByData(SceneNodeData data,Transform parent,PackageView packageView) {
        if (data!= null) {
            data_ = data;
            pv_ = packageView;
            InitPoints();
            setText(data.Name);
            this.transform.SetParent(parent,false);
            var x = data.X;//todo 将坐标转换成母节点的本地坐标
            var y = data.Y;
            var z = data.Z;
            //transform.position = new Vector3(x,y,z);
            Debug.Log("sceneNode local pos:"+x+"y:"+y);
            transform.localPosition = new Vector3(x,y,0);
            //(List<Dictionary<string, object>>)
            var portInfo = data.LinkersInfo;
            setPackageViewToOutPutItemList(packageView);
            setOutPutItemList(portInfo);
        }
    }

    public GPoint GetSceneHeadPoint() { return GetPointByIndex(2); }

    public GPoint GetOutputPortPointByIndex(int i) {
        var textItems = myOutPutBroard_.GetTextItems();
        if (i < 0 || i >= textItems.Count) return null; 
        return textItems[i].GetComponent<GPoint>();
    }

    public void DrawLineToLinkerSceneNode() {
        infoOutPutItemDrawLines();
    }
}



/*
    private void debugInit()
    {
        setOutPutItemList(new List<string>() { "123", "abcdefg", "xxxxxx", "123", "abcdefg", "xxxxxxxx", "123", "abcdefg", "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx" });
    }*/
