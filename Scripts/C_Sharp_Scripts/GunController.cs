using Godot;
using System;

public partial class GunController : Node2D
{
	Vector2 mousePos;
	Vector2 direction;
	float angleRad;
	float angleDeg;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		mousePos = GetGlobalMousePosition();
		RotateTowards(mousePos);
	}

	void RotateTowards(Vector2 targetPos){
		direction = GlobalPosition.DirectionTo(mousePos);
		angleRad = Mathf.Atan2(direction.Y, direction.X);
		angleDeg = Mathf.RadToDeg(angleRad);
		RotationDegrees = angleDeg;
	}
}
