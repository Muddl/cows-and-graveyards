shader_type spatial;

uniform vec4 base_color : source_color = vec4(0.2, 0.55, 0.15, 1.0);
uniform vec4 tip_color : source_color = vec4(0.35, 0.7, 0.25, 1.0);
uniform float noise_scale : hint_range(1.0, 50.0) = 8.0;
uniform float color_variation : hint_range(0.0, 0.3) = 0.1;
uniform float roughness : hint_range(0.0, 1.0) = 0.9;

float hash(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453);
}

float value_noise(vec2 p) {
    vec2 i = floor(p);
    vec2 f = fract(p);
    f = f * f * (3.0 - 2.0 * f);

    float a = hash(i);
    float b = hash(i + vec2(1.0, 0.0));
    float c = hash(i + vec2(0.0, 1.0));
    float d = hash(i + vec2(1.0, 1.0));

    return mix(mix(a, b, f.x), mix(c, d, f.x), f.y);
}

void fragment() {
    vec2 uv = UV * noise_scale;
    float n = value_noise(uv);

    // Blend between base and tip color with noise
    vec3 color = mix(base_color.rgb, tip_color.rgb, n * 0.5 + 0.25);

    // Add random darker/lighter patches
    float patch = value_noise(uv * 0.3);
    color += (patch - 0.5) * color_variation * 2.0;

    ALBEDO = color;
    ROUGHNESS = roughness;
}
