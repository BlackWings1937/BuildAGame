
/*
 * Ecs hybird 第二课
 * 
 * 组件
 * 
 * title: 创建海量cube
 *        定义实体运动的速度
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public struct Movement : IComponentData
{
    //运动(组件)
    public float MoveSpeed;// 不要在这里赋初始值
}
