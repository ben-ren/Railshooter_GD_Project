using Godot;
using System;

public partial class UI_Controller : Control
{
	[Export] Node2D train;
	[Export] Camera2D cam;
	[Export] TextureProgressBar fuel_bar;
	[Export] HBoxContainer tiny_train;
	[Export] Label rail_count_display;
	[Export] Control guage_needle;
	[Export] Timer timer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//Dynamically updates Interface relative to Camera position
		if(cam!=null){
			SetInterfacePosition(cam.Position);
		}
		//Dynamically updates the needle of the speed guage relative to the train's current speed.
		if(train!=null){
            float speed = (float)train.Call("GetSpeed");
			SetGuageRotation(speed/2);
		}
		//TODO
		//Dynamically update fuel bar using a timer
		if(timer!=null){
			SetFuelBar(timer.TimeLeft);
		}
		//Dynamically update the tiny_train's position 
		//Dynamically update the rail count
	}

	//Updates the position of the Interface.
	public void SetInterfacePosition(Vector2 pos){
		Position = pos;
	}

	//Updates the fuel bar display to reflect the input value
	public void SetFuelBar(double val){
		fuel_bar.Value = val;
	}

	//Get's the current fuel bar value
	public double GetFuelBarValue(){
		return fuel_bar.Value;
	}

	//Performs an int-to-string conversion to display an input integer in the Text display
	public void SetRailCountDisplay(int count){
		string str = count.ToString();
		rail_count_display.Text = str;
	}

	//Performs a string-to-int conversion to return text display as integer
	public int GetRailCountDisplay(){
		return int.Parse(rail_count_display.Text);
	}

	public void SetGuageRotation(float degrees){
		//double radians = MathF.PI/180 * degrees;
		//double degrees = MathF.PI/180 * radians;
		guage_needle.RotationDegrees = degrees;
	}
	
	public float GetGuageRotation(){
		return guage_needle.RotationDegrees;
	}
}
