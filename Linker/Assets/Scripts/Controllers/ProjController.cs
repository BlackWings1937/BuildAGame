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

    public void OnEnterProjByName(string name,string path) {

    }

    public void OnEnterOldProj() {
        var strDir = OpenDialogUtils.OpenFile();
        if (strDir!= "") {
            if (File.Exists(strDir)) {
                var str = File.ReadAllText(strDir);
                var projConfigData = JsonUtility.FromJson<ProjConfigData>(str);
                enterProjAtPath(projConfigData.name,projConfigData.path);
            }
        }
    }


    private static readonly string STR_FLODER_ANIM = "anim";
    private static readonly string STR_FLODER_JSON = "json";
    private static readonly string STR_FLODER_LUASCRIPT = "luascript";
    private static readonly string STR_FLODER_ANIMCONFIG = "animconfig";
    private static readonly string STR_FLODER_PRODUCTCONFIG = "productconfig";
    private static readonly string STR_FILE_CONFIGPROJ = "configProj.json";
    private static readonly string STR_NAME = "name";
    private static readonly string STR_PATH = "path";
    private string  createDri(string path) {
        path = path + "\\";
        Directory.CreateDirectory(path+STR_FLODER_ANIM);
        Directory.CreateDirectory(path+ STR_FLODER_JSON);
        Directory.CreateDirectory(path+ STR_FLODER_LUASCRIPT);
        Directory.CreateDirectory(path+ STR_FLODER_ANIMCONFIG);
        Directory.CreateDirectory(path+ STR_FLODER_PRODUCTCONFIG);
        var p = new ProjConfigData();
        p.name = path;
        p.path = path;
        var str = JsonUtility.ToJson(p);
        var configFilePath = path + STR_FILE_CONFIGPROJ;
        File.WriteAllText(configFilePath,str);
        return configFilePath;
    }

    private void createProjAtPath(string name,string path) {
        if (Directory.Exists(path)) {
            var configfilePath = createDri(path);
            getData<ProjData>().addProjByNameAndPath(name,path, configfilePath);
            enterProjAtPath(name,path);
        }
    }

    private void enterProjAtPath(string name,string path) {
        
        var dic = new Dictionary<string,object>();
        dic.Add("name",name);
        dic.Add("path",path);
        GetParentController().SetTargetPackageInfo(dic);
        DispearController();
    }

}
