#version 330 core

layout (location = 0) in vec3 position;

out vec2 uv;

void main()
{
    gl_Position = vec4(position, 1);
	uv = vec2((position.x + 1) * .5, (position.y + 1) * .5);
}