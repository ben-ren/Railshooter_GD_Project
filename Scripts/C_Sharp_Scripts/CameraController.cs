using Godot;
using System;

public partial class CameraController : Camera2D
{
	[Export] Node2D targetObject;
	[Export] double panSpeed;
	Vector2 startPos;
	Vector2 offset;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		startPos = Position;
		offset = startPos - targetObject.Position;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		RotationDegrees = 0;	//Block Camera Rotation
		Position = Position.Lerp(targetObject.GlobalPosition + offset, (float)(panSpeed*GetProcessDeltaTime()));
	}
}
