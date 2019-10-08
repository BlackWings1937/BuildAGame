using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackageController : AppChildController {


    // ----- 对外接口 -----
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
            Debug.Log("创建场景 方法回调");//GenerateTwoPortSceneDataByWorldPos
            //var data = getData<PackageData>().GenerateEmptySceneDataByWorldPos(posLocal);//debug
            var data = getData<PackageData>().GenerateTwoPortSceneDataByWorldPos(posLocal);
            getData<PackageData>().AddSceneData(data);
        });
        //debug
        dic.Add("粘贴",()=> { Debug.Log("粘贴节点  方法回调"); getView<PackageView>().CloseBtnsGroup(); });
        dic.Add(TapButtonsGroup.STR_KEY_CANCLE,()=> { Debug.Log("取消");  getView<PackageView>().CloseBtnsGroup(); });
        getView<PackageView>().ShowBtnsGroupByDic(touchWorldPos, dic);
    }

    public void ShowTapBtnsGroupByDicAndWorldPos(Dictionary<string, TapButtonCallBack> dic,Vector2 wp) {
        getView<PackageView>().ShowBtnsGroupByDic(wp, dic);
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
}
