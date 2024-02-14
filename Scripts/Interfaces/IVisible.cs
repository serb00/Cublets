using Godot;

public interface IVisible
{
    EntityData GetEntityData();
}

public struct EntityData
{
    public EntityType entityType;
    public float size;
    public Vector3 angle;

}