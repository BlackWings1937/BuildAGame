/*
 * 
 * 实体管理器（混合ecs模式）
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class EntitiesManager : MonoBehaviour,IConvertGameObjectToEntity
{

    public float CubeSpeed = 10;

    /// <summary>
    /// 把GameObject 转为Entity
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="dstManager"></param>
    /// <param name="conversionSystem"></param>
    void IConvertGameObjectToEntity.Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        //  创建一个组件
        var data = new RotationCubeComponent { Speed = CubeSpeed };
        // 加入entity 管理器 让unity 实体管理器进行管理
        dstManager.AddComponentData(entity,data);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
