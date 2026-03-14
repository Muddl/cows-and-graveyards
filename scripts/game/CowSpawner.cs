namespace CowsGraveyards.Game;

using Godot;

public class CowSpawner
{
    private const float SideOffsetX = 5f;
    private const float HorizonZ = -80f;
    private const float EndZ = 10f;

    public int SpawnCount { get; private set; }

    public Vector3 GetSpawnPosition(TapSide side)
    {
        float x = side == TapSide.Left ? -SideOffsetX : SideOffsetX;
        return new Vector3(x, 0f, HorizonZ);
    }

    public Vector3 GetEndPosition(TapSide side)
    {
        float x = side == TapSide.Left ? -SideOffsetX : SideOffsetX;
        return new Vector3(x, 0f, EndZ);
    }

    public void RecordSpawn()
    {
        SpawnCount++;
    }
}
