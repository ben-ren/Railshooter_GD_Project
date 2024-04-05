extends Node2D

const MathHelper = preload("res://Scripts/MathHelper.gd")
var MathH = MathHelper.new()

@export var origin : Node2D;
@export var target : Node2D;
@export var speed : float;
@export var accel : float;
@export var railCount : int;
var selectedTrackIndex
var csharp_script
var nodes
var graph_ptr_dict
var crashed : bool;
var refuel: bool;
var fuel_size;
@export var ray : Node2D;

# Called when the node enters the scene tree for the first time.
func _ready():
	selectedTrackIndex = 0 # Resets the track select to default left most track
	csharp_script = get_node("../PathingTree")
	nodes = csharp_script.GetNodes()
	graph_ptr_dict = csharp_script.GetGraphPtrDict()

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _physics_process(delta):
	if(!crashed):
		UpdateRailLists()
		MoveTo(target, delta)
		TrainController()
		MathH.rotate_towards(self, target.position, speed, get_process_delta_time() * .05)
	
	# Check's for train object's collision state
	
	# Sets after reaching target sets next target node
	if(position.distance_to(target.position) < 1.0):
		NextTarget()
	pass

func GetRailCount():
	return railCount;

func SetRailCount(num):
	railCount = num

func SetCrashed(state):
	crashed = state

func GetCrashed():
	return crashed

func GetSpeed():
	return speed

func GetOrigin():
	return origin

func GetTarget():
	return target

func GetRay():
	return ray

func GetFuelState():
	return refuel

func SetFuelState(state):
	refuel = state

# Updates the nodes & graph_ptr_dicts lists when changes are detected in original data.
func UpdateRailLists():
	if(csharp_script.GetGraphUpdateState()):
		nodes = csharp_script.GetNodes()
		graph_ptr_dict = csharp_script.GetGraphPtrDict()
		csharp_script.SetGraphUpdateState(false)

# Move's this object to the targetNode's position.
func MoveTo(targetNode, delta):
	position = position.move_toward(targetNode.position, delta*speed)
	pass

# create function to set target position to adjacent nodes in graph_ptr_dict
func NextTarget():
	# check if nodes contains the target node
	if( nodes.has(target) ):
		var index = nodes.find( target, 0 ) # Get object index in list
		var tempArr = graph_ptr_dict[index] # search through graph_ptr_dict for adjacent nodes
		# Ensure temporary Array isn't null OR empty
		if(tempArr != null && !tempArr.is_empty()):
			print("temp Array NOT null")
			# Store selected track from adjacency list. Use a modulo to prevent out of bounds exception
			var newIndex = tempArr[selectedTrackIndex%tempArr.size()]
			origin = target # set origin as old target
			target = nodes[newIndex] # set new target
			selectedTrackIndex = 0
		else:
			# temporary array is either empty or null
			print("temp Array empty or null")
			SetCrashed(true)
	else:
		print("current target not in node list")

# Updates Train movement based on player inputs
func TrainController():
	# Pressing up/down increases/decreases speed var
	speed += Input.get_axis("Axis_Y+", "Axis_Y-")*accel
	# Pressing left/right increases/decreases selectedTrackIndex
	if(Input.is_action_just_pressed("Axis_X+")):
		selectedTrackIndex += 1
	elif( Input.is_action_just_pressed("Axis_X-") ):
		selectedTrackIndex -= 1
	pass

func _on_area_2d_area_entered(area):
	if area.is_in_group("fuel"):
		SetFuelState(true)
		fuel_size = area.get_fuel_size();
		area.remove_from_group("fuel")
	elif area.is_in_group("obstacles"):
		SetCrashed(true)
	else:
		print("")
	pass # Replace with function body.

func GetFuelSize():
	return fuel_size
