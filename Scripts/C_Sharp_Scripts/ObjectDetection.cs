using Godot;
using System;

public partial class ObjectDetection : RayCast2D
{
	int collisionState;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		collisionState = -2;
	}

    public override void _Process(double delta)
    {
    }

    public int GetCollisionState(){
		return collisionState;
	}

	/*
	Uses the Raycast2D to detect the object collision type
	1. If collision is in 'nodes' array update the graph_ptr_dict
	2. If collision isn't in array set range to collision location
	*/
	public void DetectRailNode(Godot.Collections.Array<Node2D> nodeArray){
		if(IsColliding()){
			Variant obj = GetCollider();
			Node2D node = (Node2D)obj;
			//if detected object is in nodeArray set collisionState to index otherwise set to -1
			collisionState = nodeArray.IndexOf(node);
			return;
		}
		collisionState = -2;
	}

	public float SetCollisionRange(Vector2 startPos){
		return startPos.DistanceTo(GetCollisionPoint());
	}
}
