using Godot;
using System;

public partial class UIControl : Control
{
	[Export] CameraController cam;
	Vector2 offset;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		offset = cam.Position + Position;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		SetPos(cam.Position + offset);
	}

	//Set's the position of the Control node
	void SetPos(Vector2 pos){
		Position = pos;
	}

	//alligns the anchor's & dimensions of this Control node to the Camera dimensions
	void CameraSizeMatch(){

	}
}
