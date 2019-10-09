using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppData : BaseData {

    // ----- 私有成员 -----
    private Dictionary<string, object> cachePackageInfo_;
    private SceneNodeData cacheSceneInfo_;

    // ----- 对外接口 -----
    public void SetTargetPackageInfo(Dictionary<string, object> info) { cachePackageInfo_ = info; }
    public void SetTargetSceneInfo(SceneNodeData data) { cacheSceneInfo_ = data; }
    public Dictionary<string, object> GetTargetPackageInfo() { return cachePackageInfo_; }
    public SceneNodeData GetTargetSceneInfo() { return cacheSceneInfo_; }
}
