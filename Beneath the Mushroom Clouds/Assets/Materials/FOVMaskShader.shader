 Shader "Custom/FOVMaskShader" {
     SubShader{
         Tags { "RenderType" = "Opaque" "Queue" = "Geometry-1" }
         Pass {
             Blend Zero One
             ZWrite On
         }
     }
 }
