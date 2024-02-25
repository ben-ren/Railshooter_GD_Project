using Godot;
using System;

public partial class GunController : Node2D
{
	Vector2 mousePos;
	Vector2 direction;
	float angleRad;
	float angleDeg;

	Node2D parentNode;

    public override void _Ready()
    {
        parentNode = (Node2D)GetParent();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
		mousePos = GetGlobalMousePosition();
		RotateTowards(mousePos);
	}

	void RotateTowards(Vector2 targetPos){
		direction = GlobalPosition.DirectionTo(targetPos);
		angleRad = Mathf.Atan2(direction.Y, direction.X);
		angleDeg = Mathf.RadToDeg(angleRad);
		RotationDegrees = angleDeg - parentNode.RotationDegrees;
	}
}
