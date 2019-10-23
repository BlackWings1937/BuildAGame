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

    public const string STR_DRAGONBONE_DATA = "DragonBoneDatas";
    public const string STR_HD = "Hd";
    public const string STR_LD = "Ld";
    public const string STR_LUASCRIPT = "LuaScripts";
    public const string STR_JSONS = "Jsons";
    public const string STR_AUDIOS = "Audios";
    public const string STR_BGMS = "Bgms";
    public const string STR_PNGS = "Pngs";
    public const string STR_OTHERS = "Others";
    public const string STR_PRODUCTCONFIGS = "ProductConfigs";
    public const string STR_ANIMATECONFIGS = "AnimatesConfigs";
    


    private static readonly int MAX_OLD_ITEM = 10;//最大显示已经创建的项目数

    private ProjsConfig m_ProjsData_;


    // 检查项目列表中的项目是否还存在，不存在就将条目删除
    private void checkProjExist() {
        for (int i = m_ProjsData_.infos.Count -1; i >=0 ; --i) {
            var configfilepath = m_ProjsData_.infos[i].configFilePath;
            if (!File.Exists(configfilepath))
            {
                m_ProjsData_.infos.RemoveAt(i);
            }
        }
    }
    public void CheckProjExist() { checkProjExist(); }

    // 检查项目是否超过最大保存项目值，超过则删除
    private void checkIsProjOutMaxNum() {
        if (m_ProjsData_.infos.Count> MAX_OLD_ITEM) {
            m_ProjsData_.infos.RemoveRange(0,m_ProjsData_.infos.Count - MAX_OLD_ITEM);
        }
    }

    private void initFromFile() {

        if (File.Exists(STR_PROJ_CONFIG))
        {
            var str = File.ReadAllText(STR_PROJ_CONFIG);
            var info = JsonUtility.FromJson<ProjsConfig>(str);
            m_ProjsData_ = info;
            checkProjExist();
            checkIsProjOutMaxNum();
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
