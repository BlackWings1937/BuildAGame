using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

[Serializable]
public class ProjConfigData { public string name;public string path; }

public class ProjController : AppChildController {
    protected override bool init()
    {
        var result = base.init();
        return result;
    }


    public void OnCreateNewProj() {
        var strDir = OpenDialogUtils.OpenDir();
        if (strDir != "") {
            createProjAtPath(strDir, strDir);
        }
    }

    public void OnEnterProjByFilePath(string filePath) {
        enterOldProjByPath(filePath);
    }

    //选择旧项目进入
    public void OnEnterOldProj() {
        var strDir = OpenDialogUtils.OpenFile();
        enterOldProjByPath(strDir);
    }



    private void enterOldProjByPath(string path) {
        if (path!= "") {
            if (File.Exists(path)) {
                var str = File.ReadAllText(path);
                var projConfigData = JsonUtility.FromJson<ProjConfigData>(str);
                enterProjAtPath(projConfigData.name, projConfigData.path);
            }
        }
    }



    private string  createDri(string path) {
        path = path + "\\";
        Directory.CreateDirectory(path+ ProjData.STR_FLODER_ANIM);
        Directory.CreateDirectory(path+ ProjData.STR_FLODER_JSON);
        Directory.CreateDirectory(path+ ProjData.STR_FLODER_LUASCRIPT);
        Directory.CreateDirectory(path+ ProjData.STR_FLODER_ANIMCONFIG);
        Directory.CreateDirectory(path+ ProjData.STR_FLODER_PRODUCTCONFIG);
        var p = new ProjConfigData();
        p.name = path;
        p.path = path;
        var str = JsonUtility.ToJson(p);
        var configFilePath = path + ProjData.STR_FILE_CONFIGPROJ;
        File.WriteAllText(configFilePath,str);
        return configFilePath;
    }

    private void createProjAtPath(string name,string path) {
        if (Directory.Exists(path)) {
            var configfilePath = createDri(path);
            //保存项目到data
            getData<ProjData>().addProjByNameAndPath(name,path, configfilePath);
            enterProjAtPath(name,path);
        }
    }

    private void enterProjAtPath(string name,string path) {
        var dic = new Dictionary<string,object>();
        dic.Add(ProjData.STR_NAME, name);
        dic.Add(ProjData.STR_PATH, path);
        GetParentController().SetTargetPackageInfo(dic);
        DispearController();
    }

}
