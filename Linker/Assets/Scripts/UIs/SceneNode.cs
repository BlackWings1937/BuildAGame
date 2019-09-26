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

    // 注册触控事件
    private void registerEvent()
    {
        var btnAdapt = GetComponent<BtnAdaptText>();
        var text = btnAdapt.MyText;
        var pointAt = text.GetComponent<PointAt>();
        if (pointAt != null)
        {
            pointAt.SetPointUpCallBack((Vector2 wp) =>
            {
                showInputField();
            });
            pointAt.SetPointCancleCallBack((Vector2 wp) =>
            {
                closeInputField();
                setText(myInputField_.text);
            });
        }
        if (myInputField_ != null)
        {
            myInputField_.onEndEdit.AddListener((string word) =>
            {
                closeInputField();
                setText(myInputField_.text);
            });
        }
    }


    private void Start()
    {
        registerEvent();


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

        //重绘内部线段
        ClearInternalLine();
        DrawInternalLine();
    }

    //设置出口列表
    private void setOutPutItemList(List<Dictionary<string,object>> l)
    {
        if (myOutPutBroard_ != null)
        {
            ClearInternalLine();
            myOutPutBroard_.UpdateTextItemsByStringList(l);
            DrawInternalLine();
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

    // ----- 对外接口 -----

    /// <summary>
    /// 通过data 创建sceneNode
    /// </summary>
    /// <param name="data">用于创建节点的信息</param>
    /// <returns></returns>
    public static SceneNode CreateSceneByData(Dictionary<string ,object> data) {
        var g = GameObject.Instantiate(Resources.Load("prefab/SceneNode")) as GameObject;
        g.GetComponent<SceneNode>().InitSceneByData(data);
        return g.GetComponent<SceneNode>();
    }

    /// <summary>
    /// 初始化节点的data
    /// </summary>
    private Dictionary<string, object> data_;

    /// <summary>
    /// 通过data初始化节点
    /// </summary>
    /// <param name="data"></param>
    public void InitSceneByData(Dictionary<string, object> data) {
        if (data!= null) {
            data_ = data;

            InitPoints();
            setText((string)data_["Name"]);

            var portInfo = (List<Dictionary<string, object>>)data_["LinkerInfo"];
            setOutPutItemList(portInfo);
        }
    }

    public GPoint GetSceneHeadPoint() { return GetPointByIndex(2); }

    public GPoint GetOutputPortPointByIndex(int i) {
        var textItems = myOutPutBroard_.GetTextItems();
        if (i < 0 || i >= textItems.Count) return null; 
        return textItems[i].GetComponent<GPoint>();
    }
}



/*
    private void debugInit()
    {
        setOutPutItemList(new List<string>() { "123", "abcdefg", "xxxxxx", "123", "abcdefg", "xxxxxxxx", "123", "abcdefg", "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx" });
    }*/
