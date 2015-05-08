#version 330 core

// Color of the ambient light
uniform vec3 AmbientColor = vec3(1, 1, 1);
// Strength of the ambient light
uniform float AmbientStrength = 0;
// Color of the diffuse light
uniform vec3 DiffuseColor = vec3(1, 1, 1);
// Color of the specular light
uniform vec3 SpecularColor = vec3(1, 1, 1);
// The weight of the specular component of the light
uniform float Specularity = 1;
// How shiny the specular light is
uniform float Shininess = 1;

// Direction of the light (must be normalized)
uniform vec3 LightDirection;
// Position of the camera
uniform vec3 CameraPosition;

out vec3 OutputColor;

in vec4 Position;
in vec3 Normal;

void main()
{
	vec3 NormalUnit = normalize(Normal);
	vec3 LightReflect = reflect(LightDirection, NormalUnit);
	vec3 PixelToCamera = normalize(CameraPosition - Position.xyz);
	vec3 SpecularComponent = SpecularColor * pow(max(dot(PixelToCamera, LightReflect), 0), Shininess);
	vec3 DiffuseComponent = clamp(DiffuseColor * (-1 * dot(LightDirection, NormalUnit)), 0, 1);

	OutputColor = AmbientColor * AmbientStrength + DiffuseComponent + Specularity * SpecularComponent;
}