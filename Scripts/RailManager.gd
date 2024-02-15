@tool
extends Line2D

@export var startPoint : Node2D
@export var endPoint : Node2D

var area : Area2D
var col : CollisionShape2D
var segment_shape : SegmentShape2D

func _ready():
	generateColliderChildren()

func _process(_delta):
	synchronize_points()

func get_start_node():
	return startPoint

func get_end_node():
	return endPoint

# Generate Area2D & ColliderShape2D with SegmentShape2D
func generateColliderChildren():
	area = Area2D.new()
	add_child(area)
	
	col = CollisionShape2D.new()
	area.add_child(col)
	
	segment_shape = SegmentShape2D.new()
	col.shape = segment_shape

func synchronize_points():
	if (points.size() == 2):
		segment_shape.a = points[0]
		segment_shape.b = points[1]
