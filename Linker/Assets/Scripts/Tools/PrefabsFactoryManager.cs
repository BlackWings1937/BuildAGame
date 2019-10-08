/*
 * 预制体工厂单例，代码调用后就会自动创建自身实例的单例
 * 用于管理所有的预制体工厂
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabsFactoryManager : MonoBehaviour
{
    /// <summary>
    /// 单例声明
    /// </summary>
    public static readonly string STR_INSTANCE_PREFAB_PATH = "prefab/";                        // 单例预制体的路径
    private static readonly string STR_INSTANCE_PREFAB_NAME = "PrefabsFactoryManager";           // 单例预制体的名称
    private static PrefabsFactoryManager instance_ = null;
    public static PrefabsFactoryManager GetInstance() {
        if (instance_ == null) {
            var prefab = Resources.Load(STR_INSTANCE_PREFAB_PATH+STR_INSTANCE_PREFAB_NAME);
            var g = GameObject.Instantiate(prefab) as GameObject;
            instance_ = g.GetComponent<PrefabsFactoryManager>();
        }
        return instance_;
    }
    // ----- 私有成员 -----
    // 存储所有工厂的字典
    private Dictionary<string, PrefabFactory> dicOfPrefabFactorys_ = new Dictionary<string, PrefabFactory>();


    // ----- 私有方法 -----
    private void OnDestroy()
    {
        dispose();
    }
    /// <summary>
    /// 清理每一个工厂，工厂中idle 的 对象
    /// </summary>
    private void dispose() {
        // todo 
        // 1: dispose all prefabFactory
        foreach (var pair in dicOfPrefabFactorys_) {
            pair.Value.Dispose();
        }
        dicOfPrefabFactorys_.Clear();
    }

    /// <summary>
    /// 获取idle 对象母节点
    /// </summary>
    /// <param name="prefabName"></param>
    /// <returns></returns>
    private GameObject getPrefabInstancesParentByPrefabName(string prefabName) {
        var p = transform.Find(prefabName);
        if (p == null) {
            var g = new GameObject(prefabName);
            p = g.transform;
            p.SetParent(this.transform,false);
        }
        return p.gameObject;
    }

    // ----- 对外接口 -----
    /// <summary>
    /// 撤销所有工厂
    /// </summary>
    public void Dispose() {
        dispose();
    }

    /// <summary>
    /// 根据预制体的名字获取某一个工厂
    /// </summary>
    /// <param name="prefabName">预制体的名称</param>
    /// <returns></returns>
    public PrefabFactory GetFactoryByPrefabName(string prefabName) {
        var prefabInstancesParent = getPrefabInstancesParentByPrefabName(prefabName);
        PrefabFactory f = null;
        if (dicOfPrefabFactorys_.ContainsKey(prefabName))
        {
            f = dicOfPrefabFactorys_[prefabName];
            if (f == null) {
                f = PrefabFactory.Create(prefabInstancesParent,prefabName);
                dicOfPrefabFactorys_[prefabName] = f;
            }
        }
        else {
            f = PrefabFactory.Create(prefabInstancesParent, prefabName);
            dicOfPrefabFactorys_.Add(prefabName,f);
        }
        return f;
    }
}
