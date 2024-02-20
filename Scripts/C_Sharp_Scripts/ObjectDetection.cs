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

	//Uses the Raycast2D to detect the object collision type and set CollisionState	
	public void DetectRailNode(Godot.Collections.Array<Node2D> nodeArray, Node2D exemption){
		//If ray is colliding
		if(IsColliding()){
			Variant obj = GetCollider();
			Node2D node = (Node2D)obj;
			//Check if node is exempted from check
			if(node != exemption){
				collisionState = nodeArray.IndexOf(node);		//if detected object is in nodeArray set collisionState to index otherwise set to -1
				return;
			}
		}
		collisionState = -2;
	}

	//Return's the node the ray has collided with
	public Node2D GetCollidedObject(){
		if(IsColliding()){
			Variant obj = GetCollider();
			Node2D node = (Node2D)obj;
			return node;
		}
		return null;
	}

	//Return's the distance between a start position and the collision location.
	public float SetCollisionRange(Vector2 startPos){
		return startPos.DistanceTo(GetCollisionPoint());
	}
}
