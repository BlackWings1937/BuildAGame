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
    public static readonly string STR_NONAME_CHINA = "未命名场景";
    public static readonly string STR_OUTPUT_CHINA = "出口";
    public static readonly string STR_EMPTY = "Empty";
    public static readonly string STR_OBJ = "Obj";
    public static readonly string STR_X = "X";
    public static readonly string STR_Y = "Y";
    public static readonly string STR_Z = "Z";
    public static readonly string STR_PACKAGE_DATA_FILE_NAME = "projData.json";  //内容包数据文件名称

    // ----- 初始化 -----
    /// <summary>
    /// 一个内容包的所有数据
    /// </summary>
    private PackageInfoData data_;
    /// <summary>
    /// 项目数据（保存着项目路径）
    /// </summary>
    private Dictionary<string, object> projData_;

    /// <summary>
    /// 初始化内容包数据
    ///     1.如果路径下没有内容包数据则创建空数据
    ///     2.如果路径下存在内容包数据则读取数据
    /// </summary>
    private void initPackageData()
    {
        //获取项目路径数据
        var appController = GetController<PackageController>().GetParentController() as AppController;
        projData_ = appController.GetTargetPackageInfo();//mark

        //加载 处理 数据
        var path = projData_[ProjData.STR_PATH] + "\\" + STR_PACKAGE_DATA_FILE_NAME;
        if (File.Exists(path))
        {
            var strData = File.ReadAllText(path);//as Dictionary<string,object>;
            data_ = JsonUtility.FromJson<PackageInfoData>(strData); //JsonConvert.SerializeObject(dicall)
        }
        else
        {
            data_ = generateEmptyPackageData();
            saveData();
        }

        //更新数据给绘制系统
        callUpdateEvent();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public override void init()
    {
        // 初始化基类
        base.init();
        // 初始化数据
        initPackageData();
    }

    /// <summary>
    /// 更新数据给监听者
    /// </summary>
    private void callUpdateEvent() {
        eventOfDataUpdates_(data_);
    }

    /// <summary>
    /// 存储packageData 到文件
    /// </summary>
    private void saveData() {

        var strData = JsonConvert.SerializeObject(data_);
        var path = projData_[ProjData.STR_PATH]+"\\" + STR_PACKAGE_DATA_FILE_NAME;
        File.WriteAllText(path,strData);
    }

    

    /// <summary>
    /// 生成空场景包信息
    /// </summary>
    /// <returns></returns>
    private PackageInfoData generateEmptyPackageData() {
        return new PackageInfoData();
    }

    // ----- 对外接口 -----

    /// <summary>
    /// 保存data数据到文件
    /// </summary>
    public void SaveData() { saveData(); }

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
        data.ID = TimeUtils.GetTimeStamp();
        data.Name = STR_NONAME_CHINA;
        data.X = worldPos.x;
        data.Y = worldPos.y;
        data.Z = worldPos.z;
        return data;
    }

    public SceneNodeData GenerateTwoPortSceneDataByWorldPos(Vector3 worldPos) {
        SceneNodeData d = this.GenerateEmptySceneDataByWorldPos(worldPos);
        Debug.Log("gen Data local pos:"+worldPos.x+"y:"+worldPos.y);
        this.AddOutputPortToData(d);
        this.AddOutputPortToData(d);
        return d;
    }

    /// <summary>
    /// 链接一个输出端到另一个场景节点
    /// </summary>
    /// <param name="o1"></param>
    /// <param name="d2"></param>
    public void LinkerOutputToScene(OutputPortData o1, SceneNodeData d2)
    {
        o1.SceneNodeID = d2.ID;
        o1.State = OutputPortData.PortState.E_Full;
        callUpdateEvent();
        saveData();
    }

    /// <summary>
    /// 断开一个链接
    /// </summary>
    /// <param name="o1"></param>
    /// <param name="d2"></param>
    public void BreakOutputToScene(OutputPortData o1, SceneNodeData d2) {
        o1.SceneNodeID = "-1";
        o1.State = OutputPortData.PortState.E_Empty;
        callUpdateEvent();
        saveData();
    }




    /// <summary>
    /// 添加outputPort 到data
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
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
}
