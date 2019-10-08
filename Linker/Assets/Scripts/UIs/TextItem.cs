using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextItem : PointParent
{
    [SerializeField]
    private DragLine dl_;//拖拽线段

    [SerializeField]
    private PointAt pa_;//点击触发对象

    // Start is called before the first frame update
    protected override void InitPoints()
    {
        var rt = (RectTransform)transform;
        var p1 = generatePointByLocalPos(caculatePosByIndex(0));
        var p2 = generatePointByLocalPos(caculatePosByIndex(1));
        listPoints_.Add(p1);
        listPoints_.Add(p2);
    }
    private void updatePointsPos() {
        for (int i = 0; i < 2; ++i)
            listPoints_[i].transform.localPosition = caculatePosByIndex(i);
    }

    private Vector2 caculatePosByIndex(int index) {
        var result = Vector2.zero;
        if (0 <= index && index < 2) {
            var rt = (RectTransform)transform;
            if (index == 0) {
                result = new Vector2(-rt.sizeDelta.x / 2, -rt.sizeDelta.y / 2);
            } else if (index == 1) {
                result = new Vector2(rt.sizeDelta.x / 2, -rt.sizeDelta.y / 2);
            }
        }
        return result;
    }

    private void onDragLine() {
        if (dl_ != null&& pa_!=null) {
            dl_.On();
            pa_.gameObject.SetActive(false);
        }
    }
    private void offDragLine() {
        if (dl_ != null&&pa_!=null) {
            dl_.Off();
            pa_.gameObject.SetActive(true);
        }
    }
    private void setDragLineDragTriggleSize(Vector2 size) {
        if (dl_ != null&&pa_!=null) {
            dl_.SetSize(size);
            (pa_.transform as RectTransform).sizeDelta = size;
        }
    }
    private void setDragLineDragComplieCallBack(TouchObjectCallBack cb) {
        if (dl_ != null) {
            dl_.SetDragComplieCallBack(cb);
        }
    }

    private void setPointAtCallBack(TouchObjectCallBack cb) {
        if (pa_!=null) {
            pa_.SetPointUpCallBack(cb);
        }
    }

    private void drawLineWithPv() {
        if (pv_!= null&&sd_!= null) {
            if (data_.State == OutputPortData.PortState.E_Full)
                pv_.GenerateLineBetweenSceneNodeByGPAndID(listPoints_[listPoints_.Count-1], data_.SceneNodeID);
        }
    }

    private void showOptionTap()
    {//
        var dic = new Dictionary<string,TapButtonCallBack>();
        dic.Add("断开",()=> {
            if (pv_!=null) { pv_.BreakOutputToScene(data_); }
        });
        dic.Add("从这里开始", () =>
        {

        });
        if (pv_!=null) {
            pv_.ShowTapBtnsGroupByDicAndWorldPos(dic,transform.position);
        }
    }

    // ----- 对外接口 -----
    private PackageView pv_ = null;
    private SceneNodeData sd_ = null;
    private OutputPortData data_ = null;
    public void InitTextItemByData(OutputPortData data,int index,PackageView pv,SceneNodeData sd) {
        pv_ = pv;
        data_ = data;
        sd_ = sd;
        InitPoints();//data.
        var btnAdapt = GetComponent<BtnAdaptText>();
        btnAdapt.SetText("出口:"+index);
        updatePointsPos();

        if (data_.State == OutputPortData.PortState.E_Empty)
        {
            onDragLine();
        }
        else {
            offDragLine();
        }
        setDragLineDragTriggleSize(((RectTransform)this.transform).sizeDelta);
        setDragLineDragComplieCallBack((Vector2 wp)=> {
            if (pv_!= null) {
                SceneNodeData resultData = pv_.CheckSceneNodesContentWorldPos(wp);
                if (resultData != null)
                {
                    // todo: update output port then update
                    Debug.Log("hit scene Node");
                    pv_.LinkerOutputToScene(data_, resultData);
                }
                else {
                    Debug.Log("hit empty");
                }
            }
        });
        setPointAtCallBack((Vector2 wp) => {
            // 断开链接
            if (pv_ != null) {
                
                showOptionTap();
            }
        });
    }

    public void DrawLine() {
        drawLineWithPv();
    }
}
