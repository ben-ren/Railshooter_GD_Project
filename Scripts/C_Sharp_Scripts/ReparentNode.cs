using Godot;
using System;

public partial class ReparentNode : Node
{
	[Export] Node2D target = new Node2D();
	[Export] Node2D newParent = new Node2D();

	// https://www.youtube.com/watch?v=nRriOMz0wH4
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		target.Reparent(newParent);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
