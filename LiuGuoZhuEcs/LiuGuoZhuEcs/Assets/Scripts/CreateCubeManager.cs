
/*
 * Ecs hybird 第二课
 * 
 * 实体管理器
 * 
 * title: 创建海量cube
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class CreateCubeManager : MonoBehaviour
{
    //游戏对象预设
    public GameObject CubePrefab;

    //对象克隆的数量
    public int XNum = 10;
    public int YNum = 10;

    //实体对象
    Entity entitiy;
    //实体管理器对象
    EntityManager entityMgr;


    private void Start()
    {
        // 游戏对象转为实体(entity)           转化Gameobject -> entity
        entitiy = GameObjectConversionUtility.ConvertGameObjectHierarchy(CubePrefab, World.Active);// 表示活跃的游戏预设
        // 得到实体管理器        对标 GameObject
        entityMgr = World.Active.EntityManager;// 拿去一个实体出来，然后enity 做大量复制
        // 经行大量的克隆，且指定初始位置
        for (int x = 0;x<XNum;x++) { for (int y = 0;y<YNum;y++) {
                if (CubePrefab == null) { Debug.Log(GetType() + "/start()/游戏预设没有指定"); }

                // 从实体预设，大量克隆实体
                var newEntity = entityMgr.Instantiate(entitiy);
                // 对于克隆实体，定义其初始的位置
                var position = transform.TransformPoint(new float3(x-XNum/2,noise.cnoise(new float2(x,y) *0.21f),y-YNum/2));
                // 把定义的组件加入到实体管理器中 设置其中的组件参数
                entityMgr.SetComponentData(newEntity, new Translation() { Value = position });
                entityMgr.AddComponentData(newEntity,new Movement { MoveSpeed = 1F });//需要更改
            } }
    }

}
