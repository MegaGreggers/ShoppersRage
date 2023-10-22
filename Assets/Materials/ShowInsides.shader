Shader "Unlit Cull Front" {
    SubShader {
        Pass {
            Material {
                Diffuse (255,255,255,1)
            }
            Lighting On
            Cull Front
        }
    }
}