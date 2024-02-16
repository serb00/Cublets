using Godot;

public interface IVisible
{
    VisibleEntityData GetEntityData();
}

public struct VisibleEntityData
{
    public EntityType entityType;
    public float size;
    public Vector3 angle;
    public float distance;
    public float differenceToMe;

}