using Godot;
using System;
using System.Drawing;

public partial class Body : CollisionShape3D
{
	[Export] BodyData _bodyData;
	Vector3 _scale;

	public void Initialize(Creature creature, BodyData bodyData)
	{
		_bodyData = bodyData;

		// GD.Print("Body Initialize");
		creature._energyManager.AdjustMaxEnergy(_bodyData.Size * 500);

		// collision shape is centered, therefore we need to set the position to half the size of the body
		Position = new Vector3(0, _bodyData.Size, 0);
		Scale = new Vector3(_bodyData.Size, _bodyData.Size, _bodyData.Size);

		Node3D bodyModel = _bodyData.Scene.Instantiate() as Node3D;
		_scale = new Vector3(_bodyData.Size, _bodyData.Size, _bodyData.Size);
		// TODO: fix later, current cube is 2x2x2, but the body is 1x1x1
		bodyModel.Scale = _scale;
		bodyModel.Position = new Vector3(0, -_bodyData.Size, 0);
		bodyModel.Name = "BodyModel";

		AddChild(bodyModel);
	}

	public float GetSize()
	{
		return _bodyData.Size;
	}

	public Vector3 GetScale()
	{
		return _scale;
	}

	public Shape3D GetShape()
	{
		return Shape;
	}


}
