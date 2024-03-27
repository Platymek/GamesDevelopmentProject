shader_type canvas_item;

void fragment() {
	
	vec4 color = COLOR;
	
	color.a = color.a * (sin(TIME * 3.) + 1.) * .75;
	
	COLOR = color;
}
