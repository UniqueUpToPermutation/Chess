#version 330 core

uniform sampler2D Texture;

in vec2 uv;

out vec3 color;

void main()
{
    color = texture(Texture, uv);
}