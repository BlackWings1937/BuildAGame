using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackageController : AppChildController {


    // ----- 私有方法 -----
    public void showTapBtns(Vector2 wp, Dictionary<string, TapButtonCallBack> dic) {
        if (gameObject.activeSelf) {
            getView<PackageView>().ShowBtnsGroupByDic(wp, dic);
        }
    }


    // ----- 对外接口 -----
    public void SetLoadResLayerActive(bool v) { getView<PackageView>().SetLoadResLayerActive(v); }

    public void GenerateFormatProjFile() {
        getData<PackageData>().GenerateFormatProjFile();
    }
    public void PlayScene(string sceneId) {
    }
    public void StopSceneByID(string sceneId) {
    }

    public void EnterEditSceneSys(SceneNodeData data) {
        getParentController<AppController>().SetTargetSceneInfo(data);
        getView<PackageView>().PrepareToOtherView();
        DispearController();
    }

    public void CopySceneData(SceneNodeData data) {
        getData<PackageData>().CopySceneData(data);
    }
    public void RemoveSceneData(SceneNodeData data) {
        getData<PackageData>().RemoveSceneData(data);
    }
    public void LinkerOutputToScene(OutputPortData o1,SceneNodeData d2) {
        getData<PackageData>().LinkerOutputToScene(o1,d2);
    }
    public void BreakOutputToScene(OutputPortData o1, SceneNodeData d2)
    {
        getData<PackageData>().BreakOutputToScene(o1, d2);
    }

    public void ShowTapBtnsGroup(Vector2 touchWorldPos,Vector2 posLocal) {
        var dic = new Dictionary<string, TapButtonCallBack>();
        dic.Add("创建场景",()=> {
            var data = getData<PackageData>().GenerateEmptySceneDataByWorldPos(posLocal);
            getData<PackageData>().AddSceneData(data);
        });
        dic.Add(TapButtonsGroup.STR_KEY_CANCLE,()=> { Debug.Log("取消");  getView<PackageView>().CloseBtnsGroup(); });
       // getView<PackageView>().ShowBtnsGroupByDic(touchWorldPos, dic);
        showTapBtns(touchWorldPos, dic);
    }

    public void ShowTapBtnsGroupByDicAndWorldPos(Dictionary<string, TapButtonCallBack> dic,Vector2 wp) {
        showTapBtns(wp, dic);
    }

    public SceneNodeData FindSceneNodeDataByID(string id) {
        return getData<PackageData>().FindSceneNodeDataByID(id);
    }
    public void UpdateView() {
        getData<PackageData>().UpdateView();
    }

    /// <summary>
    /// 保存当前packageInfo 信息到文件
    /// </summary>
    public void SaveData() {
        getData<PackageData>().SaveData();
    }

    public void AddPortToSceneNodeBySceneData(SceneNodeData data) {
        getData<PackageData>().AddOutputPortToData(data);
    }
    public void ReloadLuaScript() {
        getData<PackageData>().ReloadLuaScript();
    }

    /// <summary>
    /// 设置场景是否正在运行中
    /// </summary>
    /// <param name="sceneId"></param>
    /// <param name="statue"></param>
    public void SetSceneRunningStatue(string sceneId, bool statue) {
        getData<PackageData>().SetSceneRunningStatue(sceneId,statue);
    }

    public PackageData.EnumLinkerDeviceStatue GetDeviceStatue() { return getData<PackageData>().MyDeviceStatue; }


    public bool IsSetWin32ProjPath() {
        var p = GetParentController().GetWin32ProjPath();
        return p != "";
    }

    public void StopWin32() {
        this.GetParentController().StopWin32Exe();
        getData<PackageData>().MyDeviceStatue = PackageData.EnumLinkerDeviceStatue.E_None;
        getView<PackageView>().UpdateUIView();
    }
    public void StartWin32() {
        this.GetParentController().PrepareWin32();
        this.GetParentController().StartWin32Exe();
        getData<PackageData>().MyDeviceStatue = PackageData.EnumLinkerDeviceStatue.E_Win32;
        getView<PackageView>().UpdateUIView();
    }

    public void StartCell() {
        this.getView<PackageView>().ShowConnectLayer();
        this.StartCellController();

    }
    public void StopCell() {
        this.getView<PackageView>().CloseConnectLayer();
        this.StopCellController();
        getData<PackageData>().MyDeviceStatue = PackageData.EnumLinkerDeviceStatue.E_None;
        getView<PackageView>().UpdateUIView();
    }
    public string GetHostIP() {
        return GetParentController().GetHostIP();
    }

    public void OnUserCloseConnectLayer() {
        this.StopCell();
    }

    public void StartCellController()
    {
        GetParentController().StartCellController();
    }
    public void StopCellController()
    {
        GetParentController().StopCellController();
    }

    public void OnUserCellConnect(string brand) {
        this.getView<PackageView>().CloseConnectLayer();
        getData<PackageData>().MyDeviceBrand = brand;
        if (brand == "Win32")
        {
            getData<PackageData>().MyDeviceStatue = PackageData.EnumLinkerDeviceStatue.E_Win32;

        }
        else {
            getData<PackageData>().MyDeviceStatue = PackageData.EnumLinkerDeviceStatue.E_Cell;
        }
        getView<PackageView>().UpdateUIView();
        
    }

    public string GetBrandName() {
        return getData<PackageData>().MyDeviceBrand;
    }
    public void SetStartScene(string sceneId)
    {
        getData<PackageData>().SetStartScene(sceneId);
    }

    private void updateResToServerByList(List<string> l) {
        GenerateFormatProjFile();
        GetParentController().UpdateResByAimFloder(l);
    }
    public void UpdateResLuaScript() {
        var l = new List<string>();
        l.Add("LuaScripts");
        updateResToServerByList(l);
    }
    public void UpdateResXMLAndJson() {
        var l = new List<string>();
        l.Add("Jsons");
        l.Add("DragonBoneDatas");
        updateResToServerByList(l);
    }
    public void updateResConfigs() {
        var l = new List<string>();
        l.Add("Others");
        updateResToServerByList(l);
    }
    public void UpdateResAll() {
        GenerateFormatProjFile();
        GetParentController().UpdateRes();
    }
    public void StartSceneLuaScript() {
        GetParentController().ShowLoadingView();

        UpdateResLuaScript();
        startScene(false);
    }
    public void StartSceneXMLAndJson() {
        GetParentController().ShowLoadingView();

        UpdateResXMLAndJson();
        startScene(true);
    }  
    public void StartSceneConfig() {
        GetParentController().ShowLoadingView();

        updateResConfigs();
        startScene(false);
    }
    public void StartSceneAll() {
        GetParentController().ShowLoadingView();
        UpdateResAll();
        startScene(true);
    }
    private void startScene(bool v) {
        
        GetParentController().PlayScene(v);
    }
    public void StopScene() {
        GetParentController().StopScene();
    }
    public bool IsSetStartScene() {
       return  getData<PackageData>().IsSetStartScene();
    }

    public PackageData.EnumLinkerDeviceStatue GetDeviceState() {
        return this.getData<PackageData>().MyDeviceStatue;
    }
}
