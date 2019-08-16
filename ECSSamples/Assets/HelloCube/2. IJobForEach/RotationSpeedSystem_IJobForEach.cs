using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

// This system updates all entities in the scene with both a RotationSpeed_IJobForEach and Rotation component.
// 使用每一个实体的RS和R 跟新每一个实体

// ReSharper disable once InconsistentNaming
public class RotationSpeedSystem_IJobForEach : JobComponentSystem
{
    // Use the [BurstCompile] attribute to compile a job with Burst. You may see significant speed ups, so try it!
    [BurstCompile]//                      在这里设置系统要操作的组件
    struct RotationSpeedJob : IJobForEach<Rotation, RotationSpeed_IJobForEach>
    {
        public float DeltaTime;

        // 这个方法只修改R组件，只读RS组件
        // The [ReadOnly] attribute tells the job scheduler that this job will not write to rotSpeedIJobForEach
        public void Execute(ref Rotation rotation, [ReadOnly] ref RotationSpeed_IJobForEach rotSpeedIJobForEach)
        {
            // Rotate something about its up vector at the speed given by RotationSpeed_IJobForEach.
            rotation.Value = math.mul(math.normalize(rotation.Value), quaternion.AxisAngle(math.up(), rotSpeedIJobForEach.RadiansPerSecond * DeltaTime));
        }
    }

    // OnUpdate runs on the main thread.
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        //相比起来应该叫做封了一包逻辑的数据，只是操作位置在这个文件而已
        var job = new RotationSpeedJob
        {
            DeltaTime = Time.deltaTime
        };

        return job.Schedule(this, inputDependencies);
    }
}
