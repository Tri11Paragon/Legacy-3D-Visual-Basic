#version 400 core

in vec2 pass_textureCoordinates;
in vec3 surfaceNormal;
in vec3 toCameraVector;

out vec4 out_Color;

uniform sampler2D modelTexture;

void main(void){
	vec3 unitVectorToCamera = normalize(toCameraVector);
	vec3 unitNormal = normalize(surfaceNormal);
	
	vec3 totalDiffuse = vec3(0.0);
	vec3 totalSpecular = vec3(0.0);

	totalDiffuse = max(totalDiffuse, 0.2);
	
	out_Color =  vec4(totalDiffuse,1.0) * ((texture(modelTexture,pass_textureCoordinates) + vec4(totalSpecular,1.0)));
}