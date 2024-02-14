using Godot;

public partial class MouseRaycaster : Marker3D
{
	[Export] Camera3D _camera;
	[Export] BrainVisualizer visualizer;
	[Export] GameManager gameManager;
	private Vector3 rayStart;
	private Vector3 rayEnd;



	public override void _Input(InputEvent inputEvent)
	{

		if (inputEvent is InputEventMouseButton eventMouseButton && eventMouseButton.ButtonIndex == MouseButton.Left && eventMouseButton.Pressed)
		{
			var space_state = GetWorld3D().DirectSpaceState;
			rayStart = _camera.ProjectRayOrigin(eventMouseButton.Position);
			rayEnd = rayStart + _camera.ProjectRayNormal(eventMouseButton.Position) * 1000;
			var query = PhysicsRayQueryParameters3D.Create(rayStart, rayEnd);
			query.CollideWithAreas = false;
			query.CollideWithBodies = true;
			// query.CollisionMask = 1 << 1; // Only collide with layer 2


			var result = space_state.IntersectRay(query);

			// Check if it hits a creature
			if (result.Count > 0 && result["collider"].Obj is CharacterBody3D body)
			{
				var creature = Utils.GetFirstParentOfType<Creature>(body);
				visualizer.SetBrainInstance(creature.GetBrain());
				gameManager.SetSelectedCreature(creature);
			}
		}
	}

}
