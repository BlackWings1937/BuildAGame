using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class PackageData : BaseData
{
    //常量声明
    public static readonly string STR_SCENELIST = "ScenesList";
    public static readonly string STR_ID = "ID";
    public static readonly string STR_NAME = "Name";
    public static readonly string STR_LINKERSINFO = "LinkersInfo";
    public static readonly string STR_SCENEINFO = "SceneInfo";
    public static readonly string STR_NONAME_CHINA = "未命名";
    public static readonly string STR_OUTPUT_CHINA = "出口";
    public static readonly string STR_EMPTY = "Empty";
    public static readonly string STR_OBJ = "Obj";


    public override void init()
    {
        base.init();
        UnityEngine.Debug.Log("packagedata init..");
        initPackageData();
    }

    public static readonly string STR_PACKAGE_DATA_FILE_NAME = "projData.json";

    private Dictionary<string, object> data_;
    private Dictionary<string, object> projData_;
    private void initPackageData() {
        UnityEngine.Debug.Log("packagedata initPackageData..");

        var appController = GetController<PackageController>().GetParentController() as AppController;
        projData_ = appController.GetTargetPackageInfo();//mark

        //加载数据
        var path = projData_[ProjData.STR_PATH] + STR_PACKAGE_DATA_FILE_NAME;
        if (File.Exists(path)) {
            var strData = File.ReadAllText(path);
            data_ = JsonConvert.DeserializeObject(strData) as Dictionary<string,object>; //JsonConvert.SerializeObject(dicall)
            UnityEngine.Debug.Log("packagedata initPackageData load data success..");
        }
        else {
            data_ = generateEmptyPackageData();
            saveData();
            UnityEngine.Debug.Log("packagedata initPackageData create empty data success..");
        }
        callUpdateEvent();
    }

    private void callUpdateEvent() {
        eventOfDataUpdates_(data_);
    }

    /// <summary>
    /// 存储packageData 到文件
    /// </summary>
    private void saveData() {
        var strData = JsonConvert.SerializeObject(data_);
        var path = projData_[ProjData.STR_PATH] + STR_PACKAGE_DATA_FILE_NAME;
        File.WriteAllText(path,strData);
    }


    /// <summary>
    /// 生成空场景包信息
    /// </summary>
    /// <returns></returns>
    private Dictionary<string,object> generateEmptyPackageData() {
        var dic = new Dictionary<string, object>();
        var scenesList = new List<Dictionary<string, object>>();
        dic.Add(STR_SCENELIST, scenesList);
        return dic;
    }

    /// <summary>
    /// 获取时间戳
    /// </summary>
    /// <returns></returns>
    public string GetTimeStamp()
    {
        TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalSeconds).ToString();
    }

    // ----- 对外接口 -----

    /// <summary>
    /// 生成空的sceneData
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, object> GenerateEmptySceneData() {
        var data = new Dictionary<string, object>();
        var timeStamp = GetTimeStamp();
        Debug.Log("timeStan:"+timeStamp);
        data.Add(STR_ID, timeStamp);
        data.Add(STR_NAME, STR_NONAME_CHINA);
        data.Add(STR_LINKERSINFO, new Dictionary<string,object>());
        data.Add(STR_SCENEINFO, new Dictionary<string,object>());
        return data;
    }



    /// <summary>
    /// 添加outputPort 到data
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    /// 
    public Dictionary<string, object> AddOutputPortToData(Dictionary<string,object> data) {
        var linkersInfo = data[STR_LINKERSINFO] as List<Dictionary<string,object>>;
        var count = linkersInfo.Count;
        var info = new Dictionary<string, object>();
        info.Add(STR_NAME, STR_OUTPUT_CHINA + count);
        info.Add(STR_OBJ, STR_EMPTY);
        linkersInfo.Add(info);
        callUpdateEvent();
        saveData();
        return data;
    }

    /// <summary>
    /// 移除出口的方法
    /// </summary>
    /// <param name="data"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public Dictionary<string, object> RemoveOutputPortForData(Dictionary<string,object> data,int index) {
        var linkersInfo = data[STR_LINKERSINFO] as List<Dictionary<string,object>>;
        linkersInfo.RemoveAt(index -1);
        for (int i = 0;i<linkersInfo.Count;++i) {
            linkersInfo[i][STR_NAME] = STR_OUTPUT_CHINA + (i + 1);
        }
        callUpdateEvent();
        saveData();
        return data;
    }

    /// <summary>
    /// 添加SceneData 到列表
    /// </summary>
    /// <param name="data"></param>
    public void AddSceneData(Dictionary<string,object> data) {
        var scenesList = data_[STR_SCENELIST] as List<Dictionary<string,object>>;
        scenesList.Add(data);
        callUpdateEvent();
        saveData();
    }

    public void RemoveSceneData(Dictionary<string,object> data) {
        var dataId = data[STR_ID] as string;
        var scenesList = data_[STR_SCENELIST] as List<Dictionary<string,object>>;
        for (int i = 0;i<scenesList.Count;++i) {
            var nowSceneData = scenesList[i];
            var nowId = nowSceneData[STR_ID] as string;
            if (dataId == nowId) {
                scenesList.RemoveAt(i);
                break;
            }
        }
        callUpdateEvent();
        saveData();
    }



    /// <summary>
    /// 获取目前的场景列表
    /// </summary>
    /// <returns></returns>
    public List<Dictionary<string, object>> GetScenesList() {
        if (data_!=null) {
            return data_[STR_SCENELIST] as List<Dictionary<string,object>>;
        }
        return new List<Dictionary<string, object>>();
    }
}
