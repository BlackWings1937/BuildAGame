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
    public static readonly string STR_X = "X";
    public static readonly string STR_Y = "Y";
    public static readonly string STR_Z = "Z";


    public override void init()
    {
        base.init();
        UnityEngine.Debug.Log("packagedata init..");
        initPackageData();
    }

    public static readonly string STR_PACKAGE_DATA_FILE_NAME = "projData.json";


    private PackageInfoData data_;
    private Dictionary<string, object> projData_;
    private void initPackageData() {
        UnityEngine.Debug.Log("packagedata initPackageData..");

        var appController = GetController<PackageController>().GetParentController() as AppController;
        projData_ = appController.GetTargetPackageInfo();//mark

        //加载数据
        var path = projData_[ProjData.STR_PATH]+"\\" + STR_PACKAGE_DATA_FILE_NAME;
        if (File.Exists(path)) {
            var strData = File.ReadAllText(path);//as Dictionary<string,object>;
            data_ = JsonUtility.FromJson<PackageInfoData>(strData); //JsonConvert.SerializeObject(dicall)

        }
        else {
            data_ = generateEmptyPackageData();
            saveData();
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
        Debug.Log("save data:"+strData);
        var path = projData_[ProjData.STR_PATH]+"\\" + STR_PACKAGE_DATA_FILE_NAME;
        File.WriteAllText(path,strData);
    }

    public void SaveData() { saveData(); }

    /// <summary>
    /// 生成空场景包信息
    /// </summary>
    /// <returns></returns>
    private PackageInfoData generateEmptyPackageData() {
        return new PackageInfoData();
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
    public SceneNodeData GenerateEmptySceneData() {
        return GenerateEmptySceneDataByWorldPos(Vector3.zero);
    }

    /// <summary>
    /// 创建节点数据通过世界坐标
    /// </summary>
    /// <param name="worldPos"></param>
    /// <returns></returns>
    public SceneNodeData GenerateEmptySceneDataByWorldPos(Vector3 worldPos) {
        var data = new SceneNodeData(); //new Dictionary<string, object>();
        data.ID = GetTimeStamp();
        data.Name = STR_NONAME_CHINA;
        data.X = worldPos.x;
        data.Y = worldPos.y;
        data.Z = worldPos.z;
        return data;
    }



    /// <summary>
    /// 添加outputPort 到data
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    /// 
    public SceneNodeData AddOutputPortToData(SceneNodeData data) {
        var linkersInfo = data.LinkersInfo;
        var newOutputPort = new OutputPortData();
        Debug.Log("addPortInfo count:"+linkersInfo.Count);
        linkersInfo.Add(newOutputPort);
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
    public SceneNodeData RemoveOutputPortForData(SceneNodeData data, int index) {
        data.LinkersInfo.RemoveAt(index);
        callUpdateEvent();
        saveData();
        return data;
    }

    /// <summary>
    /// 添加SceneData 到列表
    /// </summary>
    /// <param name="data"></param>
    public void AddSceneData(SceneNodeData data) {
        var scenesList = data_.ScenesList;
        scenesList.Add(data);
        callUpdateEvent();
        saveData();
    }

    /// <summary>
    /// 移除一个SceneNode节点
    /// </summary>
    /// <param name="data"></param>
    public void RemoveSceneData(SceneNodeData data) {
        var dataId = data.ID;
        var scenesList = data_.ScenesList;
        for (int i =0;i<scenesList.Count;++i) {
            var nowSceneData = scenesList[i];
            var nowId = nowSceneData.ID;
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
    /*
    public List<Dictionary<string, object>> GetScenesList() {
        if (data_!=null) {
            return data_[STR_SCENELIST] as List<Dictionary<string,object>>;
        }
        return new List<Dictionary<string, object>>();
    }*/
}
