using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneView : BaseView
{
    // ----- 私有成员 -----
    [SerializeField]
    private Button btnEmpty_ = null;

    [SerializeField]
    private Button btnBack_ = null;

    [SerializeField]
    private Button btnChoseSceneNodeType_ = null;


    [SerializeField]
    private ChoseOptionsBox myChoseOptionBox_ = null;


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
        if (myPlayMentEditView_!= null) {
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

    // todo plotEditView here

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
    }

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
        showChoseOptionBoxNameAndDicAndNearObj(
            "选择场景类型",
            dic,
            (RectTransform)btnChoseSceneNodeType_.transform
            );
    }


    private void initViewAsNew() {
        closeChoseOptionBox();
        setSceneChoseBtn("",null);


    }


    private void setSceneChoseBtn(string name,TapButtonCallBack cb) {
        if (btnChoseSceneNodeType_!=null) {
            btnChoseSceneNodeType_.onClick.RemoveAllListeners();
            btnChoseSceneNodeType_.GetComponent<BtnAdaptText>().SetText(name);
            //btnChoseSceneNodeType_.transform.GetChild(0).GetComponent < Text >().text = name;
            btnChoseSceneNodeType_.onClick.AddListener(()=> {
                if (cb!=null) {
                    cb();
                }
            });
        }
    }

    

    // ----- 初始化 -----
    public override void dispose()
    {
        btnBack_.onClick.RemoveAllListeners();

        closeChoseOptionBox();
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
        if (btnEmpty_ !=null) {
            btnEmpty_.onClick.AddListener(()=>{
                closeChoseOptionBox();
            });
        }
    }

    public override void init()
    {
        registerViewEvent();
        initViewAsNew();
        if (myPlayMentEditView_!= null) {
            myPlayMentEditView_.MySceneView = this;
        }
        if (myPlotEditView_ != null)
        {
            myPlotEditView_.MySceneView = this;
        }
    }

    // ----- 更新界面 -----

    private void updateViewAsEmpty(SceneNodeData data) {
        setSceneChoseBtn("选择场景类型",()=> {
            showChoseOptionSceneType();
        });
    }
    private void disposeViewAsEmpty() {
        setSceneChoseBtn("",()=> { });
    }

    private void updateViewAsPlayment(SceneNodeData data) {
        setSceneChoseBtn("玩法", () => { });
        if (myPlayMentEditView_!= null) {
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
    public void UpdateData() {
        GetController<SceneController>().UpdateData();
    }

    public void ShowInitBgConfgChoseBoxByEditBtn(Button btn) {
        if (btn!= null) {
            var dic = new Dictionary<string, TapButtonCallBack>();
            var listOfData = GetController<SceneController>().GetBgConfigsList();
            var count = listOfData.Count;
            for (int i = 0; i < count; ++i)
            {
                var str = listOfData[i];
                dic.Add(str, () => {
                    if (data_ != null)
                    {
                        data_.MySceneInfoData.PLotData.BgConfigName = str;
                        GetController<SceneController>().ParseBgConfigToData(data_.MySceneInfoData.PLotData.BgConfigName, data_);
                        GetController<SceneController>().UpdateData();
                    }
                });
            }
            showChoseOptionBoxNameAndDicAndNearObj("选择用来初始化的BgConfig", dic, btn.transform as RectTransform);
        }
    }

    public void ShowLuaScriptChoseBoxByEditBtn(Button btn) {
        if (btn!= null) {
            var dic = new Dictionary<string,TapButtonCallBack>();
            var listOfData = GetController<SceneController>().GetLuaScriptsResList();
            var count = listOfData.Count;
            for (int i = 0;i<count;++i) {
                var str = listOfData[i];
                dic.Add(str,()=> {
                    if (data_!= null) {
                        data_.MySceneInfoData.PlayMentData.LuaScriptName = str;
                        GetController<SceneController>().UpdateData();
                    }
                });
            }
            showChoseOptionBoxNameAndDicAndNearObj("选择luaScript",dic,btn.transform as RectTransform);
        }
    }
    public void ShowAnimationConfigChoseBoxByEditBtn(Button btn)
    {
        if (btn != null)
        {
            var dic = new Dictionary<string, TapButtonCallBack>();
            var listOfData = GetController<SceneController>().GetAnimationConfigsList();
            var count = listOfData.Count;
            for (int i = 0; i < count; ++i)
            {
                var str = listOfData[i];
                dic.Add(str, () => {
                    if (data_ != null)
                    {
                        data_.MySceneInfoData.PlayMentData.AnimationConfigName = str;
                        GetController<SceneController>().UpdateData();
                    }
                });
            }
            showChoseOptionBoxNameAndDicAndNearObj("选择AnimationConfig", dic, btn.transform as RectTransform);
        }
    }
    public void ShowProductConfigBoxByEditBtn(Button btn)
    {
        if (btn != null)
        {
            var dic = new Dictionary<string, TapButtonCallBack>();
            var listOfData = GetController<SceneController>().GetProductConfigsList();
            var count = listOfData.Count;
            for (int i = 0; i < count; ++i)
            {
                var str = listOfData[i];
                dic.Add(str, () => {
                    if (data_ != null)
                    {
                        data_.MySceneInfoData.PlayMentData.ProductConfigName = str;
                        GetController<SceneController>().UpdateData();
                    }
                });
            }
            showChoseOptionBoxNameAndDicAndNearObj("选择ProductConfig", dic, btn.transform as RectTransform);
        }
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
