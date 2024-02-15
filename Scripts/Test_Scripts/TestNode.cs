using Godot;
using System;

public partial class TestNode : Node2D
{
	Vector2 mousePosition;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("left_click")){
			mousePosition = GetGlobalMousePosition();
		}
		GD.Print("mouse: ", mousePosition);
	}
}
