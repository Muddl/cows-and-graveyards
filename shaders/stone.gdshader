shader_type spatial;

uniform vec4 base_color : source_color = vec4(0.45, 0.43, 0.47, 1.0);
uniform float roughness : hint_range(0.0, 1.0) = 0.85;
uniform float noise_scale : hint_range(1.0, 50.0) = 12.0;
uniform float normal_strength : hint_range(0.0, 2.0) = 0.3;
uniform float color_variation : hint_range(0.0, 0.5) = 0.08;

// Simple hash-based noise (no texture needed)
float hash(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453);
}

float value_noise(vec2 p) {
    vec2 i = floor(p);
    vec2 f = fract(p);
    f = f * f * (3.0 - 2.0 * f); // smoothstep

    float a = hash(i);
    float b = hash(i + vec2(1.0, 0.0));
    float c = hash(i + vec2(0.0, 1.0));
    float d = hash(i + vec2(1.0, 1.0));

    return mix(mix(a, b, f.x), mix(c, d, f.x), f.y);
}

float fbm(vec2 p) {
    float v = 0.0;
    float a = 0.5;
    for (int i = 0; i < 3; i++) {
        v += a * value_noise(p);
        p *= 2.0;
        a *= 0.5;
    }
    return v;
}

void fragment() {
    vec2 uv = UV * noise_scale;

    // Color variation from noise
    float n = fbm(uv);
    vec3 color = base_color.rgb + (n - 0.5) * color_variation * 2.0;
    ALBEDO = color;

    // Roughness with slight variation
    ROUGHNESS = roughness + (n - 0.5) * 0.1;

    // Normal variation from noise for subtle stone texture
    float eps = 0.01;
    float nx = fbm(uv + vec2(eps, 0.0)) - fbm(uv - vec2(eps, 0.0));
    float ny = fbm(uv + vec2(0.0, eps)) - fbm(uv - vec2(0.0, eps));
    NORMAL_MAP = vec3(0.5 + nx * normal_strength, 0.5 + ny * normal_strength, 1.0);
    NORMAL_MAP_DEPTH = 1.0;
}
