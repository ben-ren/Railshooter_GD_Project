using Godot;
using Godot.NativeInterop;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class PathingTree : Node2D
{
	[Export] Godot.Collections.Array<Node2D> nodes;		//Using Godot Arrays to enable List like properties. Since Godot Arrays have inbuilt concatenation
	[Export] Godot.Collections.Dictionary<int, Godot.Collections.Array<int>> graph_ptr_dict = new();	//Define a relationship graph of all nodes
	[Export] Node2D Railshooter;
	[Export] float RangeLimit;
	[Export] ObjectDetection ray;
	float range;
	Texture obj = (Texture)GD.Load("res://Sprites/Railshooter_prototype/Railtrack_sprite_v2(tiled)_rot90.png");	//Used to load rail sprite texture
	private Vector2 mousePosition;
	private RenderRails render;
	public Node2D generatedNode;
	public bool graphUpdated = false;
	int railCount;

	public override void _Ready(){
		render = new();
		range = RangeLimit;
	}

	public override void _Process(double delta)
	{
		PositionLogic();
		//Shoots rail on left click. If ray ISN'T colliding with TargetNode && the railCount is greater than 0
		if(Input.IsActionJustPressed("left_click") && Railshooter.HasMethod("TrainController")){
			int num = (int)Railshooter.Call("GetRailCount");
			if(num > 0){
				ShootRail(range);
				num--;
				Railshooter.Call("SetRailCount",num);
			}
			
		//null-conditional operator ('?'). It returns an assigned default boolean value when the input variable is null. 
		//Example here uses 'ray.GetCollidedObject()?' if this returns null then the following section '?? false' will default that conditional to false.
		//Delete's rail on right click 
		} else if(Input.IsActionJustPressed("right_click") && (ray.GetCollidedObject()?.IsInGroup("rail") ?? false) ){
			RemoveRail(ray.GetCollidedObject());
		}
	}

	public Godot.Collections.Array<Node2D> GetNodes(){
		return nodes;
	}

	public Godot.Collections.Dictionary<int, Godot.Collections.Array<int>> GetGraphPtrDict(){
		return graph_ptr_dict;
	}

	public bool GetGraphUpdateState(){
		return graphUpdated;
	}

	public void SetGraphUpdateState(bool state){
		graphUpdated = state;
	}

	public Node2D GetGeneratedNode(){
		return generatedNode;
	}

	public Node2D GetOrigin(){
		return (Node2D)Railshooter.Call("GetOrigin");
	}

	public Node2D GetTarget(){
		return (Node2D)Railshooter.Call("GetTarget");
	}

	//Calculates the targetPointer position.
	void PositionLogic(){
		mousePosition = GetGlobalMousePosition();
		ray.DetectRailNode(nodes, GetTarget());		//Sets the collisionState variable.
		if(ray.GetCollisionState() > -2){
			range = ray.SetCollisionRange(Railshooter.Position);	//Set range to collision location. 
		}else{
			range = RangeLimit;		//If collisionState = -2 reset range to default.
		}
	}

    //Calculate's a Vector position between 2 points at a specified range
    static Vector2 CalculateRange(Vector2 pointA, Vector2 pointB, float range){
		//These 2 lines are used to calculate direction
		Vector2 pointAB = pointB - pointA;
		Vector2 uNormalizedAB = pointAB.Normalized();
		//These 2 lines are used to calculate the Vector position at designated range.
		Vector2 pointAQ = range * uNormalizedAB;
		Vector2 pointQ = pointA + pointAQ;
		return pointQ;
	}

	//Function that shoots a new rail at mouse position
	public void ShootRail(float dist){
		Vector2 TargetRangeLimit = CalculateRange(Railshooter.Position, mousePosition, dist); //Applies a range limit to the Railshooter's gun.
		if(ray.GetCollisionState() < 0){
			generatedNode = GenerateNode(TargetRangeLimit);
		}else{
			generatedNode = nodes[ray.GetCollisionState()];
		}
		UpdateGraph(GetTarget(), GetGeneratedNode());
		render.DrawRailLines(this, GetTarget(), GetGeneratedNode());	//renders rail sprites using Line2D nodes
	}

	//Create a new Node2D in scene at target location. https://www.youtube.com/watch?v=yxin2ScRnMU
	public Node2D GenerateNode(Vector2 pos){
		//Generate new Area2D node at position pos
		Area2D newArea =new(){Position = pos};
		AddChild(newArea);
		
		//Generate a new circle collider on the node
		CollisionShape2D newCol = new();
		newArea.AddChild(newCol);
		newCol.Shape = new CircleShape2D();

		nodes.Add(newArea);		//Adds the newly generated Line2D node to the 'nodes' Godot Array
		return newArea;
	}

	//Update the relation between 2 nodes in the dictionary
	public void UpdateGraph(Node2D startNode, Node2D endNode){
		int endIndex = nodes.IndexOf(endNode);        // Index of the endNode
		int startIndex = nodes.IndexOf(startNode);    // Index of the startNode
		
		// Get or create the relation array for the endIndex
		if(graph_ptr_dict.ContainsKey(endIndex)) {
			// If it exists, append startIndex to the existing relation array
			graph_ptr_dict[endIndex].Add(startIndex);
		} else {
			// If it doesn't exist, create a new relation array and add startIndex to it
			Godot.Collections.Array<int> newRelation = new Godot.Collections.Array<int>();
			if(ray.GetCollisionState() > 0){
				// If there's a collision, add the index of the startNode to the new relation array
				newRelation.Add(startIndex);
			}
			graph_ptr_dict.Add(endIndex, newRelation); // Add the new relation to the dictionary
		}

		// Update the relation array for the startIndex
		if (graph_ptr_dict.ContainsKey(startIndex)) {
			graph_ptr_dict[startIndex].Add(endIndex);
		} else {
			// If it doesn't exist, create a new relation array and add endIndex to it
			Godot.Collections.Array<int> newRelation = new Godot.Collections.Array<int>();
			newRelation.Add(endIndex);
			graph_ptr_dict.Add(startIndex, newRelation); // Add the new relation to the dictionary
		}

		SetGraphUpdateState(true);    // Update graph state to prevent rerunning procedure.
	}

	void RemoveRail(Node2D railArea){
		//Extract RailManager data to access startPoint and endPoint for graph & array editing.
		Node2D rail = (Node2D)railArea.GetParent();
		Node2D railStart = (Node2D)rail.Get("startPoint");	//Get's startPoint from RailManager.gd of selected object
		Node2D railEnd = (Node2D)rail.Get("endPoint");		//Get's endPoint from RailManager.gd of selected object
		
		rail.QueueFree();	//Remove Rail object (Line2D) from scene.
		RemoveFromGraph(railStart, railEnd);	//Update graph to remove relation between start and end points of selected rail

		//TODO: Delete the attached TargetNode(endPoint) IF after Line2D removal it becomes a loose node
	}

	//Remove's the relation of the input Node2D's from the graph_ptr_dict.
	//Also completely' remove's the index of the deleted targetNode if node is loose.
	public void RemoveFromGraph(Node2D start, Node2D end){
		int startInt = nodes.IndexOf(start);	//Find start index
		int endInt = nodes.IndexOf(end);		//Find end index
		//Find start key in graph and Remove end index from start key's values
		graph_ptr_dict[startInt].Remove(endInt);
	}
}
