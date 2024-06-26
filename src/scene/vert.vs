precision highp float;

// ---------------------------------------------------

uniform mat3 normalMatrix;
uniform mat4 modelMatrix;
uniform mat4 modelViewMatrix;
uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;
uniform vec4 uResolution;

in vec3 normal;
in vec2 uv;
in vec4 position;

out vec2 vUv;
out vec3 vNormal;
out vec4 vPos;
out vec3 vUvRes;
out vec3 vResolution;

// ---------------------------------------------------

void main() {
  vUv = uv;
  vPos = position;
  vNormal = normalMatrix * normal;
  vUvRes = vec3(normalize(uResolution.xy), uResolution.z);
  vResolution = uResolution.xyz;

  gl_Position = projectionMatrix * modelViewMatrix * position;
}