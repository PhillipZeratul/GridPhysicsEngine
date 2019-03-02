using Unity.Entities;
using UnityEngine;


public class BootStrap : MonoBehaviour
{
    [Range(5, 30)]
    public int mapRow;
    [Range(5, 30)]
    public int mapCol;

    private EntityManager entityManager;


    private void Start()
    {
        entityManager = World.Active.GetOrCreateManager<EntityManager>();
        // Create Map
        CreateMap();
    }

    private void CreateMap()
    {
        EntityArchetype mapArchetype = entityManager.CreateArchetype(
            typeof(Map),
            typeof(Initializer)
        );

        Entity mapEntity = entityManager.CreateEntity(mapArchetype);

        entityManager.SetSharedComponentData<Map>(mapEntity, new Map
        {
            rows = mapRow,
            cols = mapCol
        });
    }
}
