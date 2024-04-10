using Godot;
using System;

public partial class MenuControl : Control
{
	[Export] Node2D train;
	[Export] Control control;
	bool complete;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		complete = (bool)train.Call("GetLevelCompleteState");
		if(complete){
			control.Visible = true;
		}else{
			control.Visible = false;
		}
	}
}
