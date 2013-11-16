Shader "Mobile/Additive" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _TintColor ("Tint color (RGB)", Color) = (1,1,1,1)
    }
    
    Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend SrcAlpha One
	Cull Off Lighting Off ZWrite Off Fog { Color (0,0,0,0) }
	
	BindChannels {
		Bind "Color", color
		Bind "Vertex", vertex
		Bind "TexCoord", texcoord
	}
	
    SubShader {
        Pass {
            Lighting Off
            SetTexture [_MainTex] {
                // Pull the color property into this blender
                constantColor [_TintColor]
                // And use the texture's alpha to blend between it and
                // vertex color
                combine constant lerp(texture) previous
            }
            // Multiply in texture
            SetTexture [_MainTex] {
                combine previous * texture
            }
        }
    }
    }
} 