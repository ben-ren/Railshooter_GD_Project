using Godot;
using System;
using System.ComponentModel.DataAnnotations;

public partial class UI_Controller : Control
{
	[Export] Node2D train;
	[Export] Camera2D cam;
	[Export] TextureProgressBar fuel_bar;
	[Export] VSlider Progression;
	[Export] Label rail_count_display;
	[Export] Control guage_needle;
	[Export] Timer timer;
	[Export] Node2D goal;
	Vector2 endPoint;
	Vector2 startPoint;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		endPoint = goal.Position;
		startPoint = train.Position;
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
		//Dynamically update fuel bar using a timer
		if(timer!=null){
			SetFuelBar(timer.TimeLeft);
		}
		//Dynamically update the tiny_train's position 
		if(goal != null){
			Vector2 distance = endPoint - startPoint;
			float progress = train.Position.Y / distance.Y * 100;	//Calculate train.Position.Y as a percentage of endPoint.Y
			Progression.Value = progress;					//Set Progression.Value to percentage value
		}
		//Dynamically update the rail count
		if(rail_count_display != null){
			int num = (int)train.Call("GetRailCount");
			SetRailCountDisplay(num);	//TODO: Replace '0' with dynamic value
		}
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
