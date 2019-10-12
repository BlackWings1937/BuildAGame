using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneView : BaseView
{
    // ----- 私有成员 -----
    [SerializeField]
    private Button btnEmpty1_ = null;

    [SerializeField]
    private Button btnEmpty2_ = null;

    [SerializeField]
    private ScrollRect scrollView_ = null;

    [SerializeField]
    private Button btnBack_ = null;

    [SerializeField]
    private Button btnChoseSceneNodeType_ = null;


    [SerializeField]
    private SVChoseOptionsBox myChoseOptionBox_ = null;


    [SerializeField]
    private PlaymentEditView myPlayMentEditView_ = null;

    [SerializeField]
    private PlotEditView myPlotEditView_ = null;

    private SceneNodeData data_;

    // debug
    [SerializeField]
    private NpcOtionsTrain testTrain_;


    // ----- 私有方法 -----

    private void showMyPlayMentEditView() {
        if (myPlayMentEditView_ != null) {
            myPlayMentEditView_.gameObject.SetActive(true);
        }
    }
    private void closeMyPlayMentEditView()
    {
        if (myPlayMentEditView_ != null) {
            myPlayMentEditView_.gameObject.SetActive(false);
        }
    }

    private void showMyPlotEditView() {
        if (myPlotEditView_ != null) {
            myPlotEditView_.gameObject.SetActive(true);
        }
    }
    private void closeMyPlotEditView() {
        if (myPlotEditView_ != null)
        {
            myPlotEditView_.gameObject.SetActive(false);
        }
    }



    private void showChoseOptionAddOptionType(RectTransform rt, string npcName) {

    }


    private void initViewAsNew() {
        myChoseOptionBox_.CloseChoseOptionBox();
        setSceneChoseBtn("", null);
    }


    private void setSceneChoseBtn(string name, TapButtonCallBack cb) {
        if (btnChoseSceneNodeType_ != null) {
            btnChoseSceneNodeType_.onClick.RemoveAllListeners();
            btnChoseSceneNodeType_.GetComponent<BtnAdaptText>().SetText(name);
            //btnChoseSceneNodeType_.transform.GetChild(0).GetComponent < Text >().text = name;
            btnChoseSceneNodeType_.onClick.AddListener(() => {
                if (cb != null) {
                    cb();
                }
            });
        }
    }



    // ----- 初始化 -----
    public override void dispose()
    {
        btnBack_.onClick.RemoveAllListeners();

        myChoseOptionBox_.CloseChoseOptionBox();
        closeMyPlayMentEditView();

        data_ = null;
    }
    protected override void registerViewEvent()
    {
        base.registerViewEvent();
        if (btnBack_ != null) {
            btnBack_.onClick.AddListener(() => {
                // todo back to package edit 
                GetController<SceneController>().BackToPackageEditView();
            });
        }
        if (btnEmpty1_ != null) {
            btnEmpty1_.onClick.AddListener(() => {
                myChoseOptionBox_.CloseChoseOptionBox();
            });
        }
        if (btnEmpty2_ != null)
        {
            btnEmpty2_.onClick.AddListener(() => {
                myChoseOptionBox_.CloseChoseOptionBox();
            });
        }
        if (scrollView_!=null) {
            scrollView_.onValueChanged.AddListener((Vector2 pos)=> {
                myChoseOptionBox_.CloseChoseOptionBox();
            });
        }
    }

    public override void init()
    {
        registerViewEvent();
        initViewAsNew();
        if (myPlayMentEditView_ != null) {
            myPlayMentEditView_.MySceneView = this;
        }
        if (myPlotEditView_ != null)
        {
            myPlotEditView_.MySceneView = this;
        }
    }

    // ----- 更新界面 -----

    private void updateViewAsEmpty(SceneNodeData data) {
        setSceneChoseBtn("选择场景类型", () => {
            if (myChoseOptionBox_ != null) {
                myChoseOptionBox_.ShowChoseOptionSceneType(
                    data,
                    btnChoseSceneNodeType_.transform as RectTransform,
                    this
                    );
            }
        });
    }
    private void disposeViewAsEmpty() {
        setSceneChoseBtn("", () => { });
    }

    private void updateViewAsPlayment(SceneNodeData data) {
        setSceneChoseBtn("玩法", () => { });
        if (myPlayMentEditView_ != null) {
            myPlayMentEditView_.UpdateByData(data.MySceneInfoData.PlayMentData);
        }
    }
    private void disposeViewAsPlayment() {

    }


    private void updateViewAsPlot(SceneNodeData data) {
        setSceneChoseBtn("剧情", () => { });
        if (myPlotEditView_ != null)
        {
            myPlotEditView_.UpdateByData(data.MySceneInfoData.PLotData);
        }
    }
    private void disposeViewAsPlot() { }


    public override void UpdateView(object info)
    {
        //closeMyPlayMentEditView showMyPlayMentEditView
        var data = info as SceneNodeData;
        data_ = data;
        var sData = data.MySceneInfoData;
        var state = sData.MyState;
        switch (state) {
            case SceneInfoData.State.E_None:
                closeMyPlayMentEditView();
                closeMyPlotEditView();
                updateViewAsEmpty(data);
                break;
            case SceneInfoData.State.E_Playment:
                showMyPlayMentEditView();
                closeMyPlotEditView();
                updateViewAsPlayment(data);
                break;
            case SceneInfoData.State.E_PLot:
                closeMyPlayMentEditView();
                showMyPlotEditView();
                updateViewAsPlot(data);
                break;
        }
    }

    // ----- 对外接口 -----

    public List<string> GetActionsList(string bgConfigName) { return GetController<SceneController>().GetActionsList(bgConfigName); }

    public void UpdateData() {
        GetController<SceneController>().UpdateData();
    }

    public void ShowInitBgConfgChoseBoxByEditBtn(Button btn) {
        if (btn != null && myChoseOptionBox_ != null) {
            myChoseOptionBox_.ShowInitBgConfgChoseBoxByEditBtn(data_, btn.transform as RectTransform, this);
        }
    }

    public void ShowOptionsByUserClickNpcItem(RectTransform rt, NpcOptions data) {
        if (rt != null && myChoseOptionBox_ != null) {
            myChoseOptionBox_.ShowOptionsByUserClickNpcItem(data_, rt, this, data);
        }
    }
    public void ShowOptionsByUserClickOptionItem(RectTransform rt, NpcOption nop) {
        if (rt != null && myChoseOptionBox_ != null) {
            myChoseOptionBox_.ShowOptionsByUserClickOptionItem(data_, rt, this, nop);
        }
    }

    public void ShowLuaScriptChoseBoxByEditBtn(Button btn) {
        if (btn != null && myChoseOptionBox_ != null) {
            myChoseOptionBox_.ShowLuaScriptChoseBoxByEditBtn(data_, btn.transform as RectTransform, this);
        }
    }
    public void ShowAnimationConfigChoseBoxByEditBtn(Button btn)
    {
        if (btn != null && myChoseOptionBox_ != null)
        {
            myChoseOptionBox_.ShowAnimationConfigChoseBoxByEditBtn(data_, btn.transform as RectTransform, this);
        }
    }
    public void ShowProductConfigBoxByEditBtn(Button btn)
    {
        if (btn != null && myChoseOptionBox_ != null)
        {
            myChoseOptionBox_.ShowProductConfigBoxByEditBtn(data_, btn.transform as RectTransform, this);
        }
    }

    public void OnAddChildNpcOptionNodeByNpcOptions(NpcOptions ndatas, RectTransform rt) {
        if (rt != null && myChoseOptionBox_ != null) {
            myChoseOptionBox_.ShowOptionsAddChildNpcOptionNode(data_, rt, this, ndatas);
        }
    }

    public void OnAddChildNpcOptionNodeByNpcOptionsAndNpcOption(NpcOptions ndatas, NpcOption nop, RectTransform rt)
    {

        if (rt != null && myChoseOptionBox_ != null)
        {
            myChoseOptionBox_.ShowOptionsAddChildNpcOptionNode(data_, rt, this, ndatas, nop);
        }
    }


    public void OnAddChildNpcOptionNodeJsonByNpcOptions(NpcOptions ndatas, RectTransform rt) {
        if (rt != null && myChoseOptionBox_ != null)
        {
            myChoseOptionBox_.ShowOptionsAddChildJsonNpcOptionNode(data_, rt, this, ndatas);
        }
    }


    public void OnAddChildNpcOptionNodeJsonByNpcOptions(NpcOptions ndatas, NpcOption nop, RectTransform rt)
    {
        if (rt != null && myChoseOptionBox_ != null)
        {
            myChoseOptionBox_.ShowOptionsAddChildJsonNpcOptionNode(data_, rt, this, ndatas, nop);
        }
    }

    public void OnAddChildNpcOptionNodeTriggleByNpcOptions(
        NpcOptions ndatas,
        RectTransform rt
        ) {
        if (rt != null && myChoseOptionBox_ != null) {
            myChoseOptionBox_.ShowOptionsAddChildTriggleByNpcOptions(rt,this,ndatas);
        }
    }


    public void OnAddChildNpcOptionNodeTriggleByNpcOptions(
        NpcOptions ndatas,
        NpcOption nop,
        RectTransform rt){
        if (rt != null && myChoseOptionBox_ != null)
        {
            myChoseOptionBox_.ShowOptionsAddChildTriggleByNpcOptions(rt, this, ndatas , nop);
        }
    }



    public void OnRemoveAllChildByNpcOptions(NpcOptions ndatas)
    {
        GetController<SceneController>().OnRemoveAllChildByNpcOptions(ndatas);
    }

    public void OnRemoveAllChildByNpcOptions(NpcOptions ndatas, NpcOption nop) {
        GetController<SceneController>().OnRemoveAllChildByNpcOptions(ndatas, nop);

    }


    /// <summary>
    /// 获取NpcOptions data 通过 npcName
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public NpcOptions GetOptionsByNpcName(string name)
    {
        return GetController<SceneController>().GetOptionsByNpcName(name);
    }


    public void AddJsonNpcOptionOnNpcOption(NpcOptions nops, string actionJsonName) {
        GetController<SceneController>().AddJsonNpcOptionOnNpcOption(nops, actionJsonName);
    }


    public void AddJsonNpcOptionOnNpcOptions(NpcOptions nops, string actionJsonName, NpcOption nop) {
        GetController<SceneController>().AddJsonNpcOptionOnNpcOptions(nops, actionJsonName, nop);
    }


    public void CopyANpcOption(NpcOption op) {
        GetController<SceneController>().CopyANpcOption(op);
    }

    public void PasteAtNpcOptions(NpcOptions nops) {
        GetController<SceneController>().PasteAtNpcOptions(nops);
    }

    public void PasteAtNpcOptions(NpcOptions nops,NpcOption nop) {
        GetController<SceneController>().PasteAtNpcOptions(nops, nop);
    }

    public bool IsCopyedNpcOption() { return GetController<SceneController>().IsCopyedNpcOption(); }
    public void DeleteNpcOption(NpcOptions nops, NpcOption nop) {
        GetController<SceneController>().DeleteNpcOption(nops, nop);
    }



    public void AddWaitPointNpcOption(NpcOptions nops) { GetController<SceneController>().AddWaitPointNpcOption(nops); }
    public void AddWaitPointNpcOption(NpcOptions nops, NpcOption nop) { GetController<SceneController>().AddWaitPointNpcOption(nops, nop); }

    public void AddWaitSoundNpcOption(NpcOptions nops) { GetController<SceneController>().AddWaitSoundNpcOption(nops); }
    public void AddWaitSoundNpcOption(NpcOptions nops, NpcOption nop) { GetController<SceneController>().AddWaitSoundNpcOption(nops, nop); }

    public void AddWaitShakeNpcOption(NpcOptions nops) { GetController<SceneController>().AddWaitShakeNpcOption(nops); }
    public void AddWaitShakeNpcOption(NpcOptions nops, NpcOption nop) { GetController<SceneController>().AddWaitShakeNpcOption(nops, nop); }


    public void AddOutputPort(NpcOptions nops)
    {
        GetController<SceneController>().AddOutputPort(nops);
    }
}



/*
var list = new List<NpcOption>();
for (int i = 0;i<2;++i) {
    var o = new NpcOption();
    o.MyState = NpcOption.State.E_PlayJson;
    o.ExData ="testJson"+i+ ".json";
    list.Add(o);
}
for (int i = 0;i<3;++i) {
    var o = new NpcOption();
    o.MyState = NpcOption.State.E_Listen;
    list.Add(o);
}
testTrain_.UpdateByNpcNameAndOptionLIst("XBL",list);
*/

// debug
/*
var dic = new Dictionary<string, TapButtonCallBack>();
var cout = 10;
for (int i = 0;i<cout;++i) {
    dic.Add("剧情场景"+i, () => {
        Debug.Log("call back i:"+i);
    });
}
showChoseOptionBoxByDicAndWorldPos(dic,transform.position);*/

// todo plotEditView here
/*
private void showChoseOptionBoxByNameAndDicAndWorldPos(
    string name,
    Dictionary<string, TapButtonCallBack> dic,
    Vector2 pos) {
    myChoseOptionBox_.gameObject.SetActive(true);
    var localPos = TransformUtils.WorldPosToNodePos(pos, transform);
    var rtBox = myChoseOptionBox_.transform as RectTransform;
    var sizeOfBox = rtBox.sizeDelta;
    myChoseOptionBox_.transform.localPosition = new Vector3(localPos.x+sizeOfBox.x/2,localPos.y-sizeOfBox.y/2,0);
    myChoseOptionBox_.UpdateBoxName(name);
    myChoseOptionBox_.UpdateOptionsByDic(dic);
}

private void showChoseOptionBoxNameAndDicAndNearObj(
    string name,
    Dictionary<string, TapButtonCallBack> dic,
    RectTransform rt
    ) {
    var pos = rt.position;
    var size = rt.sizeDelta;
    size = rt.parent.transform.TransformVector(size);
    showChoseOptionBoxByNameAndDicAndWorldPos(name, dic,new Vector2(pos.x + size.x/2, pos.y));
}
private void closeChoseOptionBox() {
    if (myChoseOptionBox_.gameObject.activeSelf)
        myChoseOptionBox_.gameObject.SetActive(false);
}*/
/*
  private void showChoseOptionSceneType() {
      var pos = btnChoseSceneNodeType_.transform.position;
      var dic = new Dictionary<string, TapButtonCallBack>();
      dic.Add("剧情类型",()=> {
          if (data_ != null) {
              data_.MySceneInfoData.MyState = SceneInfoData.State.E_PLot;
              UpdateData();
          }
      });
      dic.Add("玩法类型",()=> {
          if (data_ != null) {
              data_.MySceneInfoData.MyState = SceneInfoData.State.E_Playment;
              UpdateData();
          }
      });
      myChoseOptionBox_.ShowChoseOptionBoxByNameAnidDicAndNearObj(
          "选择场景类型",
          dic,
          (RectTransform)btnChoseSceneNodeType_.transform
          );
  }*/
