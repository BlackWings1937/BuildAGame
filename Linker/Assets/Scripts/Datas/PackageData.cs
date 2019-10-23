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

        //读取玩法脚本为场景添加出口 
        reloadLuaScript();
        //GetPortsDataByLuaScript(projData_[ProjData.STR_PATH] + "\\LuaScripts\\TestPlayMent.lua");

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
    public override void wakeup()
    {
        callUpdateEvent();
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

    /// <summary>
    /// 断开scene和其他scene的连接
    /// </summary>
    /// <param name="data"></param>
    private void removeAllLinkerSceneNode(SceneNodeData data) {
        var id = data.ID;
        var count = data_.ScenesList.Count;
        for (int i = 0;i<count;++i) {
            var ns = data_.ScenesList[i];
            if (ns != data)
            {
                var pCount = ns.LinkersInfo.Count;
                for (int z = 0; z < pCount; ++z)
                {
                    var op = ns.LinkersInfo[z];
                    if (op.SceneNodeID == id && op.State == OutputPortData.PortState.E_Full)
                    {
                        op.SceneNodeID = "-1";
                        op.State = OutputPortData.PortState.E_Empty;
                    }
                }
            }
            else {
                var pCount = ns.LinkersInfo.Count;
                for (int z = 0; z < pCount; ++z)
                {
                    var op = ns.LinkersInfo[z];
                    op.SceneNodeID = "-1";
                    op.State = OutputPortData.PortState.E_Empty;
                }
            }
        }
    }

    private List<OutputPortData> getPortsDataByLuaScript(string path) {
        var l = new List<OutputPortData>();
        if (File.Exists(path)) {
            var str = File.ReadAllText(path);
            Debug.Log(str);
            int index = 0;
            do
            {
                index = str.IndexOf("PROT");//new char[] { 'P', 'R', 'O', 'T' }s
                Debug.Log("Index:" + index);
                if (index == -1) { break; }
                int protNum = 0;
                bool result = int.TryParse(str[index + 4].ToString(), out protNum);
                if (result)
                {
                    Debug.Log("ProtNum:" + protNum);
                    var pd = new OutputPortData();
                    pd.readNum_ = protNum;
                    l.Add(pd);
                }
                else {
                    break;
                }
                str = str.Remove(index, 4);
            } while (index != -1);
            
        }
        /*
        l.Sort((OutputPortData p1,OutputPortData p2)=> {
            if (p1.readNum_>p2.readNum_) {
                return 1;
            }
            else{
                return -1;
            }
            //return 1;
        });
        */
        return l;
    }

    private void reloadLuaScript() {
        var sceneList = data_.ScenesList;
        var count = sceneList.Count;
        for (int i = 0;i<count;++i) {
            var s = sceneList[i];
            if (s.MySceneInfoData.MyState == SceneInfoData.State.E_Playment) {
                var luaScriptName = s.MySceneInfoData.PlayMentData.LuaScriptName;
                var origOutputPortData = s.LinkersInfo;
                var newOutputPortData = getPortsDataByLuaScript(projData_[ProjData.STR_PATH] + "\\LuaScripts\\"+luaScriptName);
                var countOrig = origOutputPortData.Count;
                for (int z = 0;z<countOrig;++z) {
                    var oriPortData = origOutputPortData[z];
                    for (int x = 0;x< newOutputPortData.Count;++x) {
                        var newPortData = newOutputPortData[x];
                        if (newPortData.readNum_ == oriPortData.readNum_) {
                            newPortData.State = oriPortData.State;
                            newPortData.SceneNodeID = oriPortData.SceneNodeID;
                        }
                    }
                }
                s.LinkersInfo = newOutputPortData;
            }
        }
    }

    private SceneNodeData findSceneDataBySceneID(string sceneId) {

        var sceneList = data_.ScenesList;
        var count = sceneList.Count;
        for (int i = 0; i < count; ++i)
        {
            var sceneData = sceneList[i];
            if (sceneData.ID == sceneId) {
                return sceneData;
            }
        }
        return null;
    }

    // ----- 对外接口 -----

    /// <summary>
    /// 设置场景运行状态
    /// </summary>
    /// <param name="sceneId"></param>
    /// <param name="statue"></param>
    public void SetSceneRunningStatue(string sceneId, bool statue) {
        var sceneData = findSceneDataBySceneID(sceneId);
        sceneData.IsPlaying = statue;
        callUpdateEvent();
    }


    /// <summary>
    /// 重新加载lua代码
    /// </summary>
    public void ReloadLuaScript() {
        this.reloadLuaScript();
        this.callUpdateEvent();
    }

    /// <summary>
    /// 读取lua代码获取代码中的出口数量
    /// </summary>
    /// <param name="path"></param>
    public List<OutputPortData> GetPortsDataByLuaScript(string path) {
        Debug.Log("path:"+path);
        return this.getPortsDataByLuaScript(path);
    }

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
        data.MySceneInfoData = new SceneInfoData();
        data.MySceneInfoData.SceneNodeID = data.ID;
        return data;
    }

    public SceneNodeData GenerateTwoPortSceneDataByWorldPos(Vector3 worldPos) {
        SceneNodeData d = this.GenerateEmptySceneDataByWorldPos(worldPos);
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
    /// 复制一个节点
    /// </summary>
    /// <param name="data"></param>
    public void CopySceneData(SceneNodeData data) {
        var copy = SceneNodeData.Copy(data);
        data_.ScenesList.Add(copy);
        callUpdateEvent();
        saveData();
    }

    /// <summary>
    /// 移除一个SceneNode节点
    /// </summary>
    /// <param name="data"></param>
    public void RemoveSceneData(SceneNodeData data) {
        removeAllLinkerSceneNode(data);
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
