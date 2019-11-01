using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour
{
    void Start()
    {
    }

    private LineRenderer myLineRender_;
    private List<Vector3> listOfPoes_ = new List<Vector3>() { new Vector3(0, 0, 0), new Vector3(0, 25, 0) };
    private void Init() {
        myLineRender_ = gameObject.GetComponent<LineRenderer>();
        myLineRender_.material = new Material(Shader.Find("Sprites/Default"));
        myLineRender_.widthMultiplier = 0.05f;
        myLineRender_.positionCount = 2; //listOfPoes_.Count;
    }

    private GPoint p1_;
    private GPoint p2_;
    private int  warnIndex_ = 0;
    private int warnTime = 2;

    void Update()
    {
        if (warnIndex_<warnTime) {
            myLineRender_.SetPosition(0, new Vector3(0, 0, 0));//
            myLineRender_.SetPosition(1, new Vector3(0,0, 0));//
            warnIndex_++; return; }
        if (p1_!= null&&p2_!=null&&myLineRender_!=null) {
            var wpp1 = p1_.transform.position;
            var wpp2 = p2_.transform.position;
         
            myLineRender_.SetPosition(0, new Vector3(wpp1.x, wpp1.y, 0));//
            myLineRender_.SetPosition(1, new Vector3(wpp2.x, wpp2.y, 0));//
        }
       // if (gameObject.activeSelf == false) { gameObject.SetActive(true); }
    }

    // ----- 对外接口 -----
    public void ClearWarnIndex() { warnIndex_ = 0; }
    public void SetWidth(float w) { myLineRender_.widthMultiplier = w;  }
    public void SetPoints(GPoint p1,GPoint p2) {
        p1_ = p1;
        p2_ = p2;
    }
    public void SetColor(Color c) {
        myLineRender_.material.color = c;
    }
    public void Dispose() {
        myLineRender_.SetPosition(0, new Vector3(0, 0, 0));//
        myLineRender_.SetPosition(1, new Vector3(0, 0, 0));//
        SetPoints(null,null);
    }

    private static GameObject lineParentInstance_ = null;
    public static DrawLine Create(GPoint p1,GPoint p2,Color c,float w) {
        if (lineParentInstance_ == null) {
            lineParentInstance_ = new GameObject("lineParentInstance_");
        }
        var g = PrefabsFactoryManager.GetInstance().GetFactoryByPrefabName("line").GetFalseActive(); //GameObject.Instantiate(Resources.Load("prefab/line")) as GameObject; // new GameObject();
        //g.SetActive(false);
        g.transform.SetParent(lineParentInstance_.transform);
        g.transform.localPosition = Vector3.zero;
        var d = g.GetComponent<DrawLine>();
        d.Init();
        d.SetPoints(p1, p2);
        d.SetColor(c);
        d.SetWidth(w);
        d.ClearWarnIndex();
       g.SetActive(true);
        return d;
    }
    public static void Recycle(DrawLine l) {
        l.gameObject.SetActive(false);
        l.Dispose();
        l.SetPoints(null,null);
        var f = PrefabsFactoryManager.GetInstance().GetFactoryByPrefabName("line");
        f.Recycle(l.gameObject);

    }

}
