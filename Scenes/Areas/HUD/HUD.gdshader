shader_type canvas_item;

uniform float dropOffPoint : hint_range(0, 1) = 0.1f;

void fragment() {
	
	float percentage = min((UV.y - 1. + dropOffPoint) / dropOffPoint, 1.f);
	
	COLOR *= percentage;
}
