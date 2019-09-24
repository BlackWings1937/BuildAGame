using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour
{
    void Start()
    {
        //Init();
    }

    private LineRenderer myLineRender_;
    private void Init() {
        myLineRender_ = this.gameObject.AddComponent<LineRenderer>();
        myLineRender_.positionCount = 2;
        myLineRender_.material.color = Color.blue;
        myLineRender_.SetWidth(0.1f,0.1f);
    }

    private GPoint p1_;
    private GPoint p2_;

    private void Update()
    {
        if (p1_!= null&&p2_!=null&&myLineRender_!=null) {
            var wpp1 = p1_.transform.position;
            var wpp2 = p2_.transform.position;

            myLineRender_.SetPosition(0, wpp1);
            myLineRender_.SetPosition(1, wpp2);
        }
    }

    // ----- 对外接口 -----
    public void SetPoints(GPoint p1,GPoint p2) {
        p1_ = p1;
        p2_ = p2;
    }
    public void SetColor(Color c) {
        myLineRender_.material.color = c;
    }
    public void Dispose() {
        SetPoints(null,null);
    }

    public static DrawLine Create(GPoint p1,GPoint p2,Color c) {
        var g = new GameObject();
        g.transform.localPosition = Vector3.zero;
        var d = g.AddComponent<DrawLine>();
        d.Init();
        d.SetPoints(p1, p2);
        d.SetColor(c);
  
        return d;
    }

}
