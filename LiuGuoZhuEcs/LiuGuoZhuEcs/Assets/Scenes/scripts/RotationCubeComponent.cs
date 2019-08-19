/*
 * Unity 2019 Ecs 技术 helloworld  项目演示
 * 
 * 组件
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Entities;

public struct RotationCubeComponent : IComponentData
{
    // 实体旋转速度
    public float Speed;
}
