using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextItem : PointParent
{
    // Start is called before the first frame update
    protected override void InitPoints()
    {
        var rt = (RectTransform)transform;
        var p1 = generatePointByLocalPos(new Vector2(-rt.sizeDelta.x / 2, -rt.sizeDelta.y / 2));
        var p2 = generatePointByLocalPos(new Vector2(rt.sizeDelta.x / 2, -rt.sizeDelta.y / 2));
        listPoints_.Add(p1);
        listPoints_.Add(p2);

    }

    // ----- 对外接口 -----
    private OutputPortData data_;
    public void InitTextItemByData(OutputPortData data,int index) {
        data_ = data;
        InitPoints();//data.
        var btnAdapt = GetComponent<BtnAdaptText>();
        btnAdapt.SetText("出口:"+index);
    }
}
