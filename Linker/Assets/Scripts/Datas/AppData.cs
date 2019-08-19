using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppData : BaseData {

    // ----- 私有成员 -----
    private Dictionary<string, object> cachePackageInfo_;
    private Dictionary<string, object> cacheSceneInfo_;

    // ----- 对外接口 -----
    public void SetTargetPackageInfo(Dictionary<string, object> info) { cachePackageInfo_ = info; }
    public void SetTargetSceneInfo(Dictionary<string, object> info) { cacheSceneInfo_ = info; }
    public Dictionary<string, object> GetTargetPackageInfo() { return cachePackageInfo_; }
    public Dictionary<string, object> GetTargetSceneInfo() { return cacheSceneInfo_; }
}
