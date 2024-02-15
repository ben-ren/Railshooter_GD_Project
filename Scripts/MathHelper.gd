extends Object

func RoundToSignificantFigure(value, decimals):
	var multiplier = pow(10, decimals)
	return round(value * multiplier) / multiplier
