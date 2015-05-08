#version 330 core

uniform vec3 Color;
uniform vec3 LightDirection = vec3(0, -1, 1);

uniform sampler2D ShadowMap;

out vec3 OutputColor;

in vec4 Normal;
in float Depth;
in 

void main()
{
    float visibility;

    vec4 lightSpacePosition = LightViewProjection * WorldPosition;
    vec2 shadowMapUV = vec2((lightSpacePosition.x + 1) * .5, (lightSpacePosition + 1f) * .5);

    float depth = texture(ShadowMap, shadowMapUV).z;

	OutputColor = Color * (-1 * dot(vec4(LightDirection, 0), normalize(Normal)));
}