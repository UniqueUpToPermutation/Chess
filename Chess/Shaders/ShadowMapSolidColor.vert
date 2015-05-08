#version 330 core

layout (location = 0) in vec3 VertexPosition;
layout (location = 1) in vec2 VertexUV;
layout (location = 2) in vec3 VertexNormal;

uniform mat4 World;
uniform mat4 ViewProjection;
uniform 

out vec4 Normal;
out vec4 WorldPosition;

void main()
{
	WorldPosition = World * vec4(VertexPosition, 1);
	gl_Position = ViewProjection * WorldPosition;

	Normal = World * vec4(VertexNormal, 0);
}