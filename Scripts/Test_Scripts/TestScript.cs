using Godot;
using System;
using System.Diagnostics;

public partial class TestScript : Node2D
{
	//Whenever adding or editing an [Export] variable, remember to reBUILD the scene. 
	//Apparently this is a C# specific issue
	[Export] int i = 0;
	[Export] PathFollow2D pathFollow;
	[Export] public float speed = 5f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	//Runs calculations at a fixed rate (60 times a second). Instead of per frame
	public override void _PhysicsProcess(double delta)
	{
		if(pathFollow != null){
			pathFollow.Progress += speed;
		}
	}
}
