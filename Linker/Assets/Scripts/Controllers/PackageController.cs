using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackageController : AppChildController {


    // ----- 对外接口 -----
    public void ShowTapBtnsGroup(Vector2 touchWorldPos) {
        var dic = new Dictionary<string, TapButtonCallBack>();
        dic.Add("创建场景",()=> {
            Debug.Log("创建场景 方法回调");
            var data = getData<PackageData>().GenerateEmptySceneDataByWorldPos(touchWorldPos);
            getData<PackageData>().AddSceneData(data);
        });
        //debug
        dic.Add("粘贴",()=> { Debug.Log("粘贴节点  方法回调"); getView<PackageView>().CloseBtnsGroup(); });
        dic.Add(TapButtonsGroup.STR_KEY_CANCLE,()=> { Debug.Log("取消");  getView<PackageView>().CloseBtnsGroup(); });
        getView<PackageView>().ShowBtnsGroupByDic(touchWorldPos, dic);
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
