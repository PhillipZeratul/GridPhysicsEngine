using Unity.Entities;


public struct Map : ISharedComponentData
{
    // int array of length rows x cols
    // 0 for road
    // 1 for wall
    
    public int rows;
    public int cols;
    public int[] mapArray;
}