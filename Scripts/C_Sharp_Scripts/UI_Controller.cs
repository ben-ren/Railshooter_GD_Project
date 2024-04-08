using Godot;
using System;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

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
	[Export] HBoxContainer arrow;
	Vector2 endPoint;
	Vector2 startPoint;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		endPoint = goal.Position;
		startPoint = train.Position;
		TotalFuelCapacity(timer.WaitTime);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		OutOfFuelCheck();
		bool crashed = (bool)train.Call("GetCrashed");
		if(!crashed){
			SetDisplayData();
		}else{
			SetGuageRotation(0);
		}
		bool refuel = (bool)train.Call("GetFuelState");
		if(refuel){
			double percent = (double)train.Call("GetFuelSize");
			RefuelTrain(percent);
		}
		if((bool)train.Call("GetTargetSwitched") && !crashed){
			//Change arrow sprite rotation
			RotateDirectionalArrow(arrow);
		}
	}

	void SetDisplayData(){
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

	//Set crashed state if timer.TimeLeft = 0
	void OutOfFuelCheck(){
		if(timer.TimeLeft == 0.0d){
			train.Call("SetCrashed", true);
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

	//Increases the timer value by the input amount
	public void RefuelTrain(double percent){
		double val = timer.TimeLeft + timer.WaitTime * (percent/100);
		timer.WaitTime = Math.Clamp(val, fuel_bar.MinValue, fuel_bar.MaxValue);
		timer.Start();
		train.Call("SetFuelState", false);
	}

	public void TotalFuelCapacity(double val){
		fuel_bar.MaxValue = val;
		fuel_bar.Step = val/1000;
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

	//Set's the rotation of the guage needle in degrees
	public void SetGuageRotation(float degrees){
		//double radians = MathF.PI/180 * degrees;
		//double degrees = MathF.PI/180 * radians;
		guage_needle.RotationDegrees = degrees;
	}
	
	//returns the current rotation degrees of the guage needle
	public float GetGuageRotation(){
		return guage_needle.RotationDegrees;
	}

	//Rotate's the directional arrow node to point parallel to the selected rail
	public void RotateDirectionalArrow(HBoxContainer arrow){
		//Get selected origin and target from train node 
		Node2D nextNode = (Node2D)train.Call("GetNextNode");
		Node2D target = (Node2D)train.Call("GetTarget");
		GD.Print(nextNode);
		if(nextNode != null){
			arrow.Visible = true;
			//calculate directional vector
			Vector2 direction = nextNode.Position - target.Position;
			//Set arrow rotation
			float angle = Mathf.Atan2(direction.Y, direction.X);
			float degrees = Mathf.RadToDeg(angle);
			arrow.RotationDegrees = degrees + 90;
		}
		//Reset TargetSwitched flag
		train.Call("SetTargetSwitched", false);
	}
}
