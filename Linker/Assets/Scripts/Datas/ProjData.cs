using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
[Serializable]
public class CellDataOfProj {
    public string path;
    public string name;
    public string configFilePath;
}

[Serializable]
public class ProjsConfig {
    public List<CellDataOfProj> infos = new List<CellDataOfProj>();
}

public class ProjData : BaseData {
    // 创建项目文件夹，项目记录文件，并返回文件路径
    public static readonly string STR_PROJ_CONFIG = "projConfig.json";

    public static readonly string STR_FLODER_ANIM = "anim";
    public static readonly string STR_FLODER_JSON = "json";
    public static readonly string STR_FLODER_LUASCRIPT = "luascript";
    public static readonly string STR_FLODER_ANIMCONFIG = "animconfig";
    public static readonly string STR_FLODER_PRODUCTCONFIG = "productconfig";
    public static readonly string STR_FILE_CONFIGPROJ = "configProj.json";
    public static readonly string STR_NAME = "name";
    public static readonly string STR_PATH = "path";
    public static readonly string STR_CONFIG_FILE_PATH = "configFilePath";

    private ProjsConfig m_ProjsData_;
    

    private void initFromFile() {

        if (File.Exists(STR_PROJ_CONFIG))
        {
            var str = File.ReadAllText(STR_PROJ_CONFIG);
            var info = JsonUtility.FromJson<ProjsConfig>(str);
            m_ProjsData_ = info;
        }
        else {
            m_ProjsData_ = new ProjsConfig();
            save();
        }
        callUpdateEvent();
    }
    private void callUpdateEvent() {
        if (eventOfDataUpdates_!= null) {
            var dic = new Dictionary<string, object>();
            var count = m_ProjsData_.infos.Count;
            for (int i = count -1; 0<=i; --i)
            {
                var d = m_ProjsData_.infos[i];
                var nowdic = new Dictionary<string, object>();
                nowdic.Add(STR_NAME, d.name);
                nowdic.Add(STR_PATH, d.path);
                nowdic.Add(STR_CONFIG_FILE_PATH, d.configFilePath);
                dic.Add(d.name,nowdic);
            }
            eventOfDataUpdates_(dic);
        }
    }
    public void save() {
        var str = JsonUtility.ToJson(m_ProjsData_);
        File.WriteAllText(STR_PROJ_CONFIG,str);
    }

    public void addProjByNameAndPath(string name,string path,string configFIlePath)
    {
        removeProjByNameAndPath(name,path);
        var d = new CellDataOfProj();
        d.name = name;
        d.path = path;
        d.configFilePath = configFIlePath;
        m_ProjsData_.infos.Add(d);
        save();
        callUpdateEvent();
    }
    public void removeProjByNameAndPath(string name,string path) {
        var count = m_ProjsData_.infos.Count;
        for (int i = 0; i < count; ++i)
        {
            var d = m_ProjsData_.infos[i];
            if (d.name == name && d.path == path)
            {
                m_ProjsData_.infos.RemoveAt(i);
            }
        }
        save();
        callUpdateEvent();
    }

    public override void init()
    {
        base.init();
        initFromFile();
    }
}
