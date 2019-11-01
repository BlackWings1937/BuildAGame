using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneNodeV2 : SceneNode
{
    [SerializeField]
    private VerticalTextItemGroup myOutPutBroadL_;

    protected override void setOutPutItemList(List<OutputPortData> l)
    {
        if (myOutPutBroard_!=null && myOutPutBroadL_ != null) {

            List<OutputPortData> lR = new List<OutputPortData>();
            List<OutputPortData> lL = new List<OutputPortData>();
            var count = l.Count;
            for (int i = 0; i < count; ++i)
            {
                OutputPortData opd = l[i];
                if (opd.State == OutputPortData.PortState.E_Empty)
                {
                    lR.Add(opd);
                }
                else
                {
                    var sid = opd.SceneNodeID;
                    var d = pv_.FindSceneNodeDataByID(sid);
                    if (d != null)
                    {
                        var odx = d.X;
                        var sdx = transform.localPosition.x;
                        if (odx < sdx)
                        {
                            lL.Add(opd);
                        }
                        else
                        {
                            lR.Add(opd);
                        }
                    }
                    else
                    {
                        lR.Add(opd);
                    }
                }
            }
            //base.setOutPutItemList(l);

            ClearInternalLine();
            myOutPutBroadL_.UpdateTextItemsByStringList(lL, data_);
            myOutPutBroard_.UpdateTextItemsByStringList(lR, data_);
            DrawInternalLine();
        }

    }
    protected override void infoOutPutItemDrawLines()
    {
        // base.infoOutPutItemDrawLines();
        if (myOutPutBroard_ != null)
        {
            myOutPutBroard_.InfoItemToDrawLines();

        }
        if (myOutPutBroadL_ != null)
        {
            myOutPutBroadL_.InfoItemToDrawLines();

        }
    }
    protected override void DrawInternalLine()
    {
        base.DrawInternalLine();
        if (myOutPutBroadL_ != null)
        {
            var textItems = myOutPutBroadL_.GetTextItems();
            for (int i = 0; i < textItems.Count; ++i)
            {
                var tlp = textItems[i].GetComponent<PointParent>();
             
                var l1 = DrawLine.Create(tlp.GetPointByIndex(0), tlp.GetPointByIndex(1), Color.green, 0.04f);
                var l2 = DrawLine.Create(tlp.GetPointByIndex(1), GetPointByIndex(3), Color.green, 0.04f);
                var l3 = DrawLine.Create(GetPointByIndex(0), GetPointByIndex(3), Color.green, 0.04f);
                listOfLines_.Add(l1);
                listOfLines_.Add(l2);
                listOfLines_.Add(l3);   /**/
            }
        }
    }
    protected override void setPackageViewToOutPutItemList(PackageView pv)
    {
        base.setPackageViewToOutPutItemList(pv);
        if (myOutPutBroadL_ != null)
        {
            myOutPutBroadL_.SetPackageView(pv);
        }
    }
    protected override void setText(string t)
    {
        //设置节点名称
        if (t == "")
            t = "未命名场景";
        GetComponent<BtnAdaptText>().SetText(t);

        // 更新出口面板位置
        var aimX = outputBoardPadding + ((RectTransform)transform).sizeDelta.x / 2 + ((RectTransform)myOutPutBroard_.transform).sizeDelta.x / 2;
        myOutPutBroard_.transform.localPosition = new Vector3(aimX, 0, myOutPutBroard_.transform.localPosition.z);

        aimX = -outputBoardPadding- ((RectTransform)transform).sizeDelta.x / 2-((RectTransform)myOutPutBroadL_.transform).sizeDelta.x / 2;
        myOutPutBroadL_.transform.localPosition = new Vector3(aimX, 0, myOutPutBroadL_.transform.localPosition.z);

        // 更新连接点位置
        updatePoints();

        // 更新触发器大小
        (pointAt_.transform as RectTransform).sizeDelta = (transform as RectTransform).sizeDelta;

        //重绘内部线段
        ClearInternalLine();
        DrawInternalLine();

        if (data_ != null && packageView_ != null)
        {
            data_.Name = t;
            packageView_.SaveData();
        }
    }

    protected override void updatePoints()
    {
        base.updatePoints();
        var rt = (RectTransform)transform;
        var p3 = GetPointByIndex(2);
        var p4 = GetPointByIndex(3);
        var p5 = GetPointByIndex(4);

        if (p3 !=null && p4 !=null) {
            p3.transform.localPosition = new Vector2(-rt.sizeDelta.x / 2, rt.sizeDelta.y / 2);
            p4.transform.localPosition = new Vector2(-rt.sizeDelta.x / 2, -rt.sizeDelta.y / 2);
            p5.transform.localPosition = new Vector2(rt.sizeDelta.x / 2, rt.sizeDelta.y / 2);
        }
    }

    protected override void InitPoints()
    {
        base.InitPoints();
        var rt = (RectTransform)transform;
        var p4 = generatePointByLocalPos(new Vector2(-rt.sizeDelta.x / 2, -rt.sizeDelta.y / 2));
        listPoints_.Add(p4);
        var p5 = generatePointByLocalPos(new Vector2(rt.sizeDelta.x / 2, rt.sizeDelta.y / 2));
        listPoints_.Add(p5);
        var p6 = generatePointByLocalPos(new Vector2(0,0));
        listPoints_.Add(p6);


    }


    public static SceneNodeV2 CreateSceneByData(SceneNodeData data, Transform parent, PackageView pv)
    {

        var f = PrefabsFactoryManager.GetInstance().GetFactoryByPrefabName("SceneNodeV2");
        if (f == null) { Debug.LogError("create scene Node fail"); return null; }
        var g = PrefabsFactoryManager.GetInstance().GetFactoryByPrefabName("SceneNodeV2").Get();
        g.GetComponent<SceneNodeV2>().InitSceneByData(data, parent, pv);
        return g.GetComponent<SceneNodeV2>();
    }

    public static void DestroyObj(SceneNodeV2 n)
    {
        if (n == null) { Debug.LogError(" param scenenode is null"); return; }
        var f = PrefabsFactoryManager.GetInstance().GetFactoryByPrefabName("SceneNodeV2");
        if (f == null) { Debug.LogError("destroy scene Node fail"); return; }
        f.Recycle(n.gameObject);
    }

    public override void InitSceneByData(SceneNodeData data, Transform parent, PackageView packageView)
    {
        this.myOutPutBroadL_.MyState = VerticalTextItemGroup.State.E_LEFT;
        this.myOutPutBroard_.MyState = VerticalTextItemGroup.State.E_RIGHT;
        base.InitSceneByData(data, parent, packageView);

    }
}
