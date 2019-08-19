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

    private static readonly string STR_PROJ_CONFIG = "projConfig.json";

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
                nowdic.Add("name",d.name);
                nowdic.Add("path",d.path);
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

/*
    private void debugData() {//debug
        for (int i = 0;i<10;i++) {
            var d = new CellDataOfProj();
            d.name = "proj:" + i;
            d.path = "d:\\Proj" + i;
            m_ProjsData_.infos.Add(d);
        }
    }*/
