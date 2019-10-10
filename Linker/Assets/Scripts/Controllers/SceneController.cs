using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : AppChildController {

    // ----- 对外接口 -----
    public void BackToPackageEditView() {
        DisposeController();
    }
    public void UpdateData() {
        getData<SceneData>().UpdateData();
    }
    public void ParseBgConfigToData(string bgConfigName, SceneNodeData data)
    {
        getData<SceneData>().ParseBgConfigToData(bgConfigName, data);
    }

    public List<string> GetLuaScriptsResList() {
        return getData<SceneData>().GetLuaScriptsResList();
    }
    public List<string> GetProductConfigsList() { return getData<SceneData>().GetProductConfigsList(); }
    public List<string> GetAnimationConfigsList() { return getData<SceneData>().GetAnimationConfigsList(); }
    public List<string> GetBgConfigsList() { return getData<SceneData>().GetBgConfigsList(); }

}
 