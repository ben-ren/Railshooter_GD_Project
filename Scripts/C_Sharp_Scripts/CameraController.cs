using Godot;
using System;

public partial class CameraController : Camera2D
{
	[Export] Node2D targetObject;
	[Export] double panSpeed;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GD.Print(Position);
		RotationDegrees = 0;	//Block Camera Rotation
		Position = Position.Lerp(targetObject.GlobalPosition, (float)(panSpeed*GetProcessDeltaTime()));
	}
}
