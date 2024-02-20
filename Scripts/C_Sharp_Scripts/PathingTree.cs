using Godot;
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

	public override void _Ready(){
		render = new();
		range = RangeLimit;
	}

	public override void _Process(double delta)
	{
		GD.Print(ray.GetCollidedObject());
		PositionLogic();
		//Shoots rail on left click. If ray ISN'T colliding with TargetNode
		if(Input.IsActionJustPressed("left_click") && Railshooter.HasMethod("TrainController")){
			ShootRail(range);
		//null-conditional operator ('?'). It returns an assigned default boolean value when the input variable is null. 
		//Example here uses 'ray.GetCollidedObject()?' if this returns null then the following section '?? false' will default that conditional to false.
		} else if(Input.IsActionJustPressed("right_click") && (ray.GetCollidedObject()?.IsInGroup("rail") ?? false) ){
			/*TODO: 
			* 1. Extract RailManager data to access startPoint and endPoint for graph & array editing.
			* 2. Write DeleteNode function to remove Line2D from scene.
			* 3. Delete the attached TargetNode IF after Line2D removal it becomes a loose node
			* 4. Update the graph to remove the index of the removed node from all associated nodes using RemoveFromGraph() function.
			*/
			DeleteNode(ray.GetCollidedObject());
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

	//Delete's the selected Node and it's children from the scene.
	public void DeleteNode(Node2D node){
		RemoveFromGraph(node);
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

	//Remove's the input Node2D from the graph_ptr_dict
	public void RemoveFromGraph(Node2D badNode){
		
	}
}
