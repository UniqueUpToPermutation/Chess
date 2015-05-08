#version 330 core

layout (location = 0) in vec3 VertexPosition;
layout (location = 1) in vec2 VertexUV;
layout (location = 2) in vec3 VertexNormal;

uniform mat4 World;
uniform mat4 ViewProjection;

out vec2 UV;

void main()
{
	gl_Position = ViewProjection * World * vec4(VertexPosition, 1);
	UV = VertexUV;
}