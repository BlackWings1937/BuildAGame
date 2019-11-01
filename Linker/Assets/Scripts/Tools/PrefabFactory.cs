/*
 * 用于复用prefab实例的管理器
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabFactory 
{
    private string prefabName_;             //预制体名称
    private int preCount_;                  // 预制体大体数量
    private List<GameObject> usingList_;     // 正在使用中的预制体实例数量
    private List<GameObject> idleList_;      // 闲置的预制体实例数量
    private GameObject prefabInstance_;      // 预制体实例
    private GameObject idleInstancesParent_; // 限制预制体实例母节点
    // 加载预制体模板实例
    private bool loadPrefabInstance(string prefabName) {
        prefabInstance_ = Resources.Load(PrefabsFactoryManager.STR_INSTANCE_PREFAB_PATH + prefabName) as GameObject;
        return checkFactoryIsInit();
    }
    // 初始化列表
    private void initListByCount(int count) {
        usingList_ = new List<GameObject>(count);// is already create this match gameObject?
        idleList_ = new List<GameObject>(count);
    }
    // 批量创建预定数量的实例到列表
    private void preCreatePrefabInstanceToList() {
        for (int i = 0;i<preCount_;++i) {
            var g = GameObject.Instantiate(prefabInstance_) as GameObject;
            addToIdleList(g);
        }
    }
    // 检查工厂是否初始化
    private bool checkFactoryIsInit() {
        return (prefabInstance_ != null && idleInstancesParent_!=null);
    }

    private void addToIdleList(GameObject g) {
        g.SetActive(false);
        g.transform.SetParent(idleInstancesParent_.transform,false);
        idleList_.Add(g);

    }
    private void addToUsingList(GameObject g) {
        g.SetActive(true);
        usingList_.Add(g);
    }
    // ----- 对外接口 -----
    /// <summary>
    /// 设置闲置实力母节点
    /// </summary>
    /// <param name="g">闲置实例母节点</param>
    public void SetIdleInstanceParent(GameObject g) {
        if (g!= null) {
            idleInstancesParent_ = g;
        }
    }

    /// <summary>
    /// 初始化factory ，预制体需要放到项目路径下的 Resource/Prefabs 目录下才能使用这个工厂
    /// </summary>
    /// <param name="prefabName">预制体的名称</param>
    /// <param name="count">预制体实例大概需要的数量</param>
    /// <returns></returns>
    public bool InitByPrefabNameAndCount(string prefabName,int count) {
        if (loadPrefabInstance(prefabName) && count>=0) {
            prefabName_ = prefabName;
            preCount_ = count;
            initListByCount(preCount_);
            preCreatePrefabInstanceToList();
            return true;
        }
        return false;
    }

    public bool InitByInstancesParentPrefabNameCount(GameObject instancesParent, string prefabName, int count = 0) {
        this.SetIdleInstanceParent(instancesParent);
        if (InitByPrefabNameAndCount(prefabName,count)) {
            return true;
        }
        return false;
    }

    public static PrefabFactory Create(GameObject instancesParent,string prefabName,int count = 0) {
        PrefabFactory pf = new PrefabFactory();
        if (pf.InitByInstancesParentPrefabNameCount(instancesParent, prefabName,count)) {
            return pf;
        }
        Debug.LogError("PrefabFactory create fail.. check prefab is exist?");
        return null;
    }

    /// <summary>
    /// 撤销这类预制体工厂的方法（将删除所有闲置的预制体，需要确保所有预制体都被回收）
    /// </summary>
    public void Dispose() {
        if (!checkFactoryIsInit()) { return; }
        idleInstancesParent_ = null;
        //GameObject.Destroy(prefabInstance_); // should delete?
        prefabInstance_ = null;
        prefabName_ = "";
        preCount_ = 0;
        //for (int i = 0;i<usingList_.Count;++i) { // should delete?
        //    GameObject.Destroy(usingList_[i]);
        //}
        usingList_.Clear();
        for (int i = 0;i<idleList_.Count;++i) {
            GameObject.Destroy(idleList_[i]);
        }
        idleList_.Clear();
        usingList_ = null;
        idleList_ = null;
    }

    /// <summary>
    /// 获取预制体实例
    /// </summary>
    /// <returns>预制体实例</returns>
    public GameObject Get() {
        if (!checkFactoryIsInit()) { return null; }
        GameObject g = null;
        if (idleList_.Count>0) {
            int rearIndex = idleList_.Count - 1;
            g = idleList_[rearIndex];
            idleList_.RemoveAt(rearIndex);
        } else {
            g = GameObject.Instantiate(prefabInstance_) as GameObject;
        }
        addToUsingList(g);
        return g;
    }

    public GameObject GetFalseActive()
    {
        if (!checkFactoryIsInit()) { return null; }
        GameObject g = null;
        if (idleList_.Count > 0)
        {
            int rearIndex = idleList_.Count - 1;
            g = idleList_[rearIndex];
            idleList_.RemoveAt(rearIndex);
        }
        else
        {
            g = GameObject.Instantiate(prefabInstance_) as GameObject;
        }
        usingList_.Add(g);
        return g;
    }

    /// <summary>
    /// 回收预制体实例
    /// </summary>
    /// <param name="g">预制体实例</param>
    public void Recycle(GameObject g) {
        if (!checkFactoryIsInit()) { return ; }
        if (usingList_.Contains(g)) {
            usingList_.Remove(g);
            addToIdleList(g);
        }
    }
}
