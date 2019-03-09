using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;


public class ControlSystem : ComponentSystem
{
    private ComponentGroup controlGroup;


    protected override void OnCreateManager()
    {
        controlGroup = GetComponentGroup(typeof(Control));
    }

    protected override void OnUpdate()
    {
        if (controlGroup.CalculateLength() != 1)
            throw new System.Exception("Number of Control Entity is not 1!" );

        using (var controlEntities = controlGroup.ToEntityArray(Allocator.TempJob))
        {
            var control = EntityManager.GetComponentData<Control>(controlEntities[0]);

            control.move = new float2(Input.GetAxis(Constants.Contorl.Horizontal), Input.GetAxis(Constants.Contorl.Vertial));

            EntityManager.SetComponentData<Control>(controlEntities[0], control);
        }
    }
}
