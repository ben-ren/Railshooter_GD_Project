extends Object

func RoundToSignificantFigure(value, decimals):
	var multiplier = pow(10, decimals)
	return round(value * multiplier) / multiplier

func rotate_towards(obj: Node2D, target_pos: Vector2, speed: float, delta: float):
	var direction = obj.global_position.direction_to(target_pos)
	var angle_rad = atan2(direction.y, direction.x)
	var angle_deg = rad_to_deg(angle_rad)
	var target_rotation = angle_deg + 90
	obj.rotation_degrees = lerp(obj.rotation_degrees, target_rotation, speed * delta)
