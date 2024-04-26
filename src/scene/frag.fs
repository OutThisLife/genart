precision highp float;

// ---------------------------------------------------

#define R vUvRes
#define Rpx vResolution
#define PI 3.14159265359
#define TWOPI 6.28318530718
#define PHI 2.61803398875
#define TAU 1.618033988749895

#define saturate(a) clamp(a, 0., 1.)
#define S(a, b) step(a, b)
#define SM(a, b, v) smoothstep(a, b, v)
#define SME(v, r) SM(0., r / Rpx.x, v)
#define dot2(v) dot(v, v)
#define hue(v) (.6 + .6 * cos(6.3 * (v) + vec3(0, 23, 21)))
#define rot(a) mat2(cos(a), sin(a), -sin(a), cos(a))
#define rot3d(a) mat3(cos(a), sin(a), 0, -sin(a), cos(a), 0, 0, 0, 1)
#define mapLinear(x, a1, a2, b1, b2) b1 + (x - a1) * (b2 - b1) / (a2 - a1)

// ---------------------------------------------------

uniform float uTime;
uniform vec2 uMouse;
uniform sampler2D uChannel0;

in vec2 vUv;
in vec3 vUvRes;
in vec3 vResolution;
in vec4 vPos;

out vec4 fragColor;

// --------------------------------------------------

float rand(float n) { return fract(sin(n) * 43758.5453123); }
float rand(vec2 n) {
  return fract(sin(dot(n, vec2(12.9898, 4.1414))) * 43758.5453);
}

float noise(float p) {
  float fl = floor(p);
  float fc = fract(p);
  return mix(rand(fl), rand(fl + 1.0), fc);
}

float noise(vec2 n) {
  const vec2 d = vec2(0.0, 1.0);
  vec2 b = floor(n), f = smoothstep(vec2(0.0), vec2(1.0), fract(n));
  return mix(mix(rand(b), rand(b + d.yx), f.x),
             mix(rand(b + d.xy), rand(b + d.yy), f.x), f.y);
}

const mat2 m2 = mat2(0.8, -0.6, 0.6, 0.8);

float noise(vec2 p, float freq) {
  float unit = R.x / freq;
  vec2 ij = floor(p / unit);
  vec2 xy = mod(p, unit) / unit;
  // xy = 3.*xy*xy-2.*xy*xy*xy;
  xy = .5 * (1. - cos(PI * xy));
  float a = rand((ij + vec2(0., 0.)));
  float b = rand((ij + vec2(1., 0.)));
  float c = rand((ij + vec2(0., 1.)));
  float d = rand((ij + vec2(1., 1.)));
  float x1 = mix(a, b, xy.x);
  float x2 = mix(c, d, xy.x);
  return mix(x1, x2, xy.y);
}

float fbm(in vec2 p) {
  float f = 0.0;
  f += 0.5000 * noise(p);
  p = m2 * p * 2.02;
  f += 0.2500 * noise(p);
  p = m2 * p * 2.03;
  f += 0.1250 * noise(p);
  p = m2 * p * 2.01;
  f += 0.0625 * noise(p);

  return f / 0.9375;
}

float pnoise(vec2 p, int res) {
  float persistance = .5;
  float n = 0.;
  float normK = 0.;
  float f = 4.;
  float amp = 1.;
  int iCount = 0;
  for (int i = 0; i < 50; i++) {
    n += amp * noise(p, f);
    f *= 2.;
    normK += amp;
    amp *= persistance;
    if (iCount == res)
      break;
    iCount++;
  }
  float nf = n / normK;
  return nf * nf * nf * nf;
}

void main() {
  vec2 st = (vUv * 2. - 1.) / R.xy;
  vec2 uv = gl_FragCoord.xy / Rpx.xy;

  float t = uTime;
  float mo = distance(uMouse, vUv);

  vec4 col;
  vec4 fb = texture(uChannel0, uv);

  {
    vec2 p = vUv - .5;
    p.x *= R.x;

    t *= .5;
    vec2 o = vec2(cos(t * .5), sin(t * .5));
    o *= rot(t);

    // float d = .1 / pnoise(((p * 8.) / 1. - fbm((p * rot(t * .2)) * 3.)) -
    //                       vec2(cos(t * .3 + pnoise(p * 100.)),
    //                            sin(t * .3 + pnoise(p * 30. * pnoise(p)))));
    float n = rand(p.y);
    float d = .2 / pnoise(((p * rot(-t * .3) * 4.) *
                           SM(0., ceil(t), .1 / fbm(p * rot(t * .5)))) -
                              o,
                          int(n));
    d += .1 / tan(d);
    d = saturate(d);
    d = mapLinear(d, 0., 1.001, 0., 1.);

    vec3 c0 = vec3(1);
    c0 = mix(hue(d + t + p.y), vec3(10), p.x * p.y);
    c0 = pow(c0, vec3(p.x - .3, p.y - .3, p.x + p.y));

    // col = mix(col, vec4(hue(d + t - p.y), 1), 1. - d);
    col = mix(col, vec4(c0, 1), 1. - d);
    col = mix(col, fb, .99);
  }

  fragColor = saturate(col);
}