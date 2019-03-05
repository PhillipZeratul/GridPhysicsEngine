using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;
using System;


public class RandomMoveSystem : JobComponentSystem
{
    Unity.Mathematics.Random rnd;


    [BurstCompile]
    struct RandomMoveJob : IJobProcessComponentData<PhysicsObject, Obstacle>
    {
        public float dT;
        public Unity.Mathematics.Random rnd;


        public void Execute(ref PhysicsObject physicsObject, [ReadOnly] ref Obstacle obstacle)
        {
            float moveX = rnd.NextFloat(-obstacle.maxRndMoveSpeed, obstacle.maxRndMoveSpeed) * dT;
            float moveY = rnd.NextFloat(-obstacle.maxRndMoveSpeed, obstacle.maxRndMoveSpeed) * dT;
           
            physicsObject.dx = moveX;
            physicsObject.dy = moveY;
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        rnd.InitState((uint)Guid.NewGuid().GetHashCode());

        var job = new RandomMoveJob
        {
            dT = Time.deltaTime,
            rnd = rnd
        };

        return job.Schedule(this, inputDeps);
    }
}
