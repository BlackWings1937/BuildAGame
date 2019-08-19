/*
 * Unity 2019 Ecs 技术 helloworld  项目演示
 * 
 * 系统
 * 
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class RotationCubeSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        //传进去一个lamda 像一个闭包一样丢进去
        // 遍历交给foreach 每一个的计算交给我
        Entities.ForEach((ref RotationCubeComponent rotaSpeed,ref Rotation rotation)=>{
            rotation.Value = math.mul(math.normalize(rotation.Value),quaternion.AxisAngle(math.up(),rotaSpeed.Speed*Time.deltaTime));
        });
    }
}
