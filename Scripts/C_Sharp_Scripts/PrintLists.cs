using Godot;
using System;


public partial class PrintLists : Object{
    //Print all Node2D's stored in nodes Array
    public void PrintNodes(PathingTree pathing_tree){
        Godot.Collections.Array<Node2D> nodeList = pathing_tree.GetNodes();
		String output = "";
		for(int i=0; i<nodeList.Count; i++){
			output += (i + ", " + nodeList[i].Position + " | ");
		}
		GD.Print(output);
	}

    //print all index adjacencies stores in graphPtrDict dictionary
	public void PrintGraphDict(PathingTree pathing_tree){
        Godot.Collections.Dictionary<int, Godot.Collections.Array<int>> graphPtrDict = pathing_tree.GetGraphPtrDict();
		foreach(var kvp in graphPtrDict){
			int key = kvp.Key;
			Godot.Collections.Array<int> value = kvp.Value;
			GD.Print("Key: ", key, ", Value: ", value);
		}
	}
}