using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class AppConfigData {
    public string Win32ProjPath = "";
}

public class AppData : BaseData {

    // ----- 私有成员 -----
    private Dictionary<string, object> cachePackageInfo_;
    private SceneNodeData cacheSceneInfo_;


    private const string STR_FILE_NAME_APP_CONFIG = "appConfig.json";
    private AppConfigData appData_;
    private void initData() {
        var currPath = System.Environment.CurrentDirectory;
        var path = currPath + "\\" + STR_FILE_NAME_APP_CONFIG;
        if (File.Exists(path))
        {
            var str = File.ReadAllText(path);
            appData_ = JsonUtility.FromJson<AppConfigData>(str);
        }
        else {
            appData_ = new AppConfigData();
        }
    }

    private void saveData() {
        var str = JsonUtility.ToJson(appData_);
        var currPath = System.Environment.CurrentDirectory;
        var path = currPath + "\\" + STR_FILE_NAME_APP_CONFIG;
        File.WriteAllText(path, str);
    }


    public override void init()
    {
        initData();
        base.init();
    }

    // ----- 对外接口 -----

    public void SetWin32ProjPath(string p) {
        appData_.Win32ProjPath = p;
        saveData();
    }
    public string GetWin32ProjPath() {
        return appData_.Win32ProjPath;
    }
    public void SetTargetPackageInfo(Dictionary<string, object> info) { cachePackageInfo_ = info; }
    public void SetTargetSceneInfo(SceneNodeData data) { cacheSceneInfo_ = data; }
    public Dictionary<string, object> GetTargetPackageInfo() { return cachePackageInfo_; }
    public SceneNodeData GetTargetSceneInfo() { return cacheSceneInfo_; }
}
