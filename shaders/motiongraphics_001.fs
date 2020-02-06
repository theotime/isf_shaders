// @lsdlive
// CC-BY-NC-SA

// www.moduloprime.com

// Checkout this on shadertoy: https://www.shadertoy.com/view/tlcXD8

// With the help of https://thebookofshaders.com/examples/?chapter=motionToolKit
// With the help of Flopine.
// With the help of FabriceNeyret2.


/*{
	"DESCRIPTION": "Motion Graphics #001",
	"CREDIT": "www.moduloprime.com",
	"CATEGORIES": [
		"Generator"
	],
	"INPUTS": [
    {
      "NAME": "bpm",
      "LABEL": "BPM",
      "TYPE": "float",
      "DEFAULT": 120,
      "MIN": 0.
    },
    {
      "NAME": "ring_base_sz",
      "LABEL": "Ring size",
      "TYPE": "float",
      "DEFAULT": 0.025,
      "MIN": 0.
    },
    {
      "NAME": "ring_base_width",
      "LABEL": "Ring Width",
      "TYPE": "float",
      "DEFAULT": 0.05,
      "MIN": 0.
    },
    {
      "NAME": "speed",
      "LABEL": "Speed",
      "TYPE": "float",
      "DEFAULT": 0.5,
      "MIN": 0.
    }
	]
}*/


// https://lospec.com/palette-list/1bit-monitor-glow
//vec3 col1 = vec3(.133, .137, .137);
//vec3 col2 = vec3(.941, .965, .941);


const float pi = 3.141592654;

#define g_time (speed*(bpm/60.)*TIME)

mat2 r2d(float a) {
    float c = cos(a), s = sin(a);
    return mat2(c, s, -s, c);
}

// https://thebookofshaders.com/edit.php?log=160909064320
float easeInOutExpo(float t) {
    if (t == 0. || t == 1.) {
        return t;
    }
    if ((t *= 2.) < 1.) {
        return .5 * exp2(10. * (t - 1.));
    } else {
        return .5 * (-exp2(-10. * (t - 1.)) + 2.);
    }
}

// not used, but can be
float easeInOutQuad(float t) {
    if ((t *= 2.) < 1.) {
        return .5 * t * t;
    } else {
        return -.5 * ((t - 1.) * (t - 3.) - 1.);
    }
}

// not used, but can be
float easeInOutCubic(float t) {
    if ((t *= 2.) < 1.) {
        return .5 * t * t * t;
    } else {
        return .5 * ((t -= 2.) * t * t + 2.);
    }
}

// https://thebookofshaders.com/edit.php?log=160909064528
float ring(vec2 p, float radius, float width) {
  	return abs(length(p) - radius * 0.5) - width;
}

float smoothedge(float v, float f) {
    return smoothstep(0., f / RENDERSIZE.x, v);
}

void main(void) {
    vec2 uv = (gl_FragCoord.xy - .5 * RENDERSIZE.xy) / RENDERSIZE.y;

    // rotation animation
    float t = easeInOutExpo(fract(g_time));
    uv *= r2d((pi / 2.) * (floor(g_time) + t));

    // ring size animation
    float offs = .5;
	  t = easeInOutExpo(fract(g_time + offs));
    float anim_sz = .25 + .25 * sin(pi * .75 + pi * (floor(g_time + offs) + t));

    float ring = ring(uv, ring_base_sz + anim_sz, ring_base_width);
    float eps = abs(ring); // sharpen around the ring

    float sdf = (2. * smoothstep(-eps, eps, uv.x) - 1.) * ring;
    float mask = smoothedge(sdf, 4.); // cut sdf + AA

    //vec3 col = mix(col1, col2, mask);
    vec3 col = vec3(mask); // black & white mask for VJ tool
    gl_FragColor = vec4(col, 1.);
}
