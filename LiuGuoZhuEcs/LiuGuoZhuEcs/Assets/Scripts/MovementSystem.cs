/*
 * Ecs hybird 第二课
 * 
 * movement 系统
 * 
 * title: 创建海量cube
 *        定义组件（实体）运动的速度与方法
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*system 需要的命名空间*/
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class MovementSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        //                    
        Entities.ForEach((ref Translation translation,ref Movement movement)=> {
            translation.Value.y += movement.MoveSpeed * Time.deltaTime;
            //上下移动边界值控制
            var limite = 1F;
            if (translation.Value.y > limite)
            {
                movement.MoveSpeed = -Mathf.Abs(movement.MoveSpeed);
            }
            else if(translation.Value.y<-limite) {
                movement.MoveSpeed = Mathf.Abs(movement.MoveSpeed);
            }
        });//传入一个lamda
    }
}
