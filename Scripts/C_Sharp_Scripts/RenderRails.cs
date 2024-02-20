using Godot;
using System;
using System.Dynamic;

public partial class RenderRails : Object{

	Texture2D rail_texture = (Texture2D)GD.Load("res://Sprites/Railshooter_prototype/Railtrack_sprite_v2(tiled)_rot90.png");
	Script railManagerScript = (Script)GD.Load("res://Scripts/RailManager.gd");

	//Generates a Line2D node attaches the rail texture and draws it between the 2 input nodes.
	public void DrawRailLines(Node2D pathingTree, Node2D targetNode, Node2D newNode){
		bool dataPresent = pathingTree!= null && targetNode != null && newNode != null && rail_texture != null && railManagerScript != null;
		if(dataPresent){
			//Configure Line2D settings
			Line2D line = new();	//generate new Line2D node
			line.Position -= targetNode.Position;

			//Attach Script
			line.SetScript(railManagerScript);
			line.CallDeferred("set_start_point", targetNode);		//Sets the Node2D startPoint variable as the input targetNode. 
			line.CallDeferred("set_end_point", newNode);		//Sets the Node2D startPoint variable as the input targetNode. 

			line.Texture = rail_texture;					//Assign rail_texture to line
			line.TextureMode = Line2D.LineTextureMode.Tile;	//Set Texture Mode to 'Tiled'
			line.Width = 30;								//Set Line texture pixel Width
			
			//Add 2 points to Line2D
			Vector2 targetNodeGlobalPos = pathingTree.GlobalPosition + targetNode.Position;
			Vector2 newNodeGlobalPos = pathingTree.GlobalPosition + newNode.Position;
			line.AddPoint(targetNodeGlobalPos);	//Set point[0] to global targetNode position
			line.AddPoint(newNodeGlobalPos);		//Set point[1] to global newNode Position

			targetNode.AddChild(line);				//Set to child of the targetNode
		}
	}
}