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
            var data = getData<PackageData>().GenerateTwoPortSceneDataByWorldPos(posLocal);
            getData<PackageData>().AddSceneData(data);
        });
        dic.Add("粘贴",()=> { getView<PackageView>().CloseBtnsGroup(); });
        dic.Add(TapButtonsGroup.STR_KEY_CANCLE,()=> { Debug.Log("取消");  getView<PackageView>().CloseBtnsGroup(); });
       // getView<PackageView>().ShowBtnsGroupByDic(touchWorldPos, dic);
        showTapBtns(touchWorldPos, dic);
    }

    public void ShowTapBtnsGroupByDicAndWorldPos(Dictionary<string, TapButtonCallBack> dic,Vector2 wp) {
        showTapBtns(wp, dic);
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
