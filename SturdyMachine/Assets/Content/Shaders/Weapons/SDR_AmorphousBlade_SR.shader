// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:3,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:True,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:2,rntp:3,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.1803922,fgcg:0.4117647,fgcb:0.0509804,fgca:1,fgde:0.01,fgrn:0,fgrf:10000,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:2865,x:33904,y:33987,varname:node_2865,prsc:2|diff-8186-OUT,spec-358-OUT,gloss-1813-OUT,emission-7992-OUT,clip-9613-OUT;n:type:ShaderForge.SFN_Color,id:6665,x:32147,y:32971,ptovrint:False,ptlb:ColorA,ptin:_ColorA,varname:_ColorA,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0.5372549,c3:0.909804,c4:1;n:type:ShaderForge.SFN_Tex2d,id:7736,x:31921,y:32621,varname:node_7736,prsc:2,tex:e1e61dc4cb4d36e4fbd90b06182ebbde,ntxv:0,isnm:False|UVIN-3517-OUT,TEX-1436-TEX;n:type:ShaderForge.SFN_Slider,id:358,x:33246,y:34049,ptovrint:False,ptlb:Metallic,ptin:_Metallic,varname:_Metallic,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Slider,id:1813,x:33246,y:34155,ptovrint:False,ptlb:Gloss,ptin:_Gloss,varname:_Gloss,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.8,max:1;n:type:ShaderForge.SFN_Panner,id:5685,x:31699,y:32621,varname:node_5685,prsc:2,spu:-0.3,spv:-1|UVIN-1538-OUT;n:type:ShaderForge.SFN_TexCoord,id:5739,x:30858,y:32487,varname:node_5739,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Tex2d,id:4720,x:31920,y:32988,varname:node_4720,prsc:2,tex:e1e61dc4cb4d36e4fbd90b06182ebbde,ntxv:0,isnm:False|UVIN-1031-OUT,TEX-1436-TEX;n:type:ShaderForge.SFN_Panner,id:4900,x:31698,y:32988,varname:node_4900,prsc:2,spu:0.3,spv:-1|UVIN-3274-OUT;n:type:ShaderForge.SFN_TexCoord,id:2241,x:30867,y:32984,varname:node_2241,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Tex2dAsset,id:1436,x:31399,y:32894,ptovrint:False,ptlb:Masks RGB,ptin:_MasksRGB,varname:_MasksRGB,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:e1e61dc4cb4d36e4fbd90b06182ebbde,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:1538,x:31406,y:32716,varname:node_1538,prsc:2|A-5739-UVOUT,B-7714-OUT;n:type:ShaderForge.SFN_ValueProperty,id:7714,x:31076,y:32845,ptovrint:False,ptlb:PaternSizeA,ptin:_PaternSizeA,varname:_PaternSizeA,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_Multiply,id:3274,x:31399,y:33104,varname:node_3274,prsc:2|A-2241-UVOUT,B-7925-OUT;n:type:ShaderForge.SFN_ValueProperty,id:7925,x:30867,y:33175,ptovrint:False,ptlb:PaternSizeB,ptin:_PaternSizeB,varname:_PaternSizeB,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1.5;n:type:ShaderForge.SFN_Blend,id:28,x:32159,y:32448,varname:node_28,prsc:2,blmd:18,clmp:False|SRC-7736-R,DST-4720-R;n:type:ShaderForge.SFN_TexCoord,id:3145,x:32159,y:32236,varname:node_3145,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Blend,id:6563,x:32451,y:32386,varname:node_6563,prsc:2,blmd:10,clmp:False|SRC-28-OUT,DST-3145-V;n:type:ShaderForge.SFN_Color,id:4488,x:32155,y:33281,ptovrint:False,ptlb:ColorB,ptin:_ColorB,varname:_ColorB,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.4470589,c2:0,c3:1,c4:1;n:type:ShaderForge.SFN_Slider,id:2579,x:31867,y:33187,ptovrint:False,ptlb:ColorsBloom,ptin:_ColorsBloom,varname:_ColorsBloom,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:1,cur:2,max:3;n:type:ShaderForge.SFN_Multiply,id:5813,x:32367,y:33027,varname:node_5813,prsc:2|A-6665-RGB,B-2579-OUT;n:type:ShaderForge.SFN_Multiply,id:1339,x:32367,y:33185,varname:node_1339,prsc:2|A-4488-RGB,B-2579-OUT;n:type:ShaderForge.SFN_Lerp,id:1449,x:32816,y:32397,varname:node_1449,prsc:2|A-5813-OUT,B-1339-OUT,T-6563-OUT;n:type:ShaderForge.SFN_Slider,id:7216,x:33104,y:33081,ptovrint:False,ptlb:Ignition,ptin:_Ignition,varname:_Ignition,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_TexCoord,id:1962,x:33248,y:33367,varname:node_1962,prsc:2,uv:3,uaff:False;n:type:ShaderForge.SFN_Tex2d,id:2755,x:33612,y:33467,varname:_node_2755,prsc:2,tex:e1e61dc4cb4d36e4fbd90b06182ebbde,ntxv:0,isnm:False|UVIN-1962-UVOUT,TEX-1436-TEX;n:type:ShaderForge.SFN_Subtract,id:5704,x:33958,y:32961,varname:node_5704,prsc:2|A-5444-OUT,B-1962-V;n:type:ShaderForge.SFN_Ceil,id:4588,x:34280,y:32960,varname:node_4588,prsc:2|IN-5704-OUT;n:type:ShaderForge.SFN_RemapRange,id:5444,x:33643,y:32883,varname:node_5444,prsc:2,frmn:0.3,frmx:1,tomn:0,tomx:1|IN-7216-OUT;n:type:ShaderForge.SFN_RemapRange,id:1122,x:33632,y:33241,varname:node_1122,prsc:2,frmn:0,frmx:0.35,tomn:0,tomx:1|IN-7216-OUT;n:type:ShaderForge.SFN_Subtract,id:3814,x:34129,y:33351,varname:node_3814,prsc:2|A-8728-OUT,B-9338-OUT;n:type:ShaderForge.SFN_Ceil,id:450,x:34304,y:33351,varname:node_450,prsc:2|IN-3814-OUT;n:type:ShaderForge.SFN_Clamp,id:8728,x:33893,y:33239,varname:node_8728,prsc:2|IN-1122-OUT,MIN-7775-OUT,MAX-6116-OUT;n:type:ShaderForge.SFN_OneMinus,id:9338,x:33795,y:33467,varname:node_9338,prsc:2|IN-2755-B;n:type:ShaderForge.SFN_Vector1,id:7775,x:33758,y:33330,varname:node_7775,prsc:2,v1:0;n:type:ShaderForge.SFN_Vector1,id:6116,x:33758,y:33388,varname:node_6116,prsc:2,v1:0.95;n:type:ShaderForge.SFN_Add,id:9613,x:34619,y:33452,varname:node_9613,prsc:2|A-4588-OUT,B-450-OUT;n:type:ShaderForge.SFN_Lerp,id:4653,x:33353,y:33723,varname:node_4653,prsc:2|A-1449-OUT,B-4806-OUT,T-9691-OUT;n:type:ShaderForge.SFN_OneMinus,id:9691,x:34525,y:32978,varname:node_9691,prsc:2|IN-4588-OUT;n:type:ShaderForge.SFN_Color,id:177,x:32627,y:33514,ptovrint:False,ptlb:IgnitionColor,ptin:_IgnitionColor,varname:_IgnitionColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.9411765,c2:0.5803922,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:1353,x:32880,y:33604,varname:node_1353,prsc:2|A-2755-B,B-177-RGB;n:type:ShaderForge.SFN_Multiply,id:4806,x:32939,y:33768,varname:node_4806,prsc:2|A-1353-OUT,B-3265-OUT;n:type:ShaderForge.SFN_RemapRange,id:105,x:32795,y:33353,varname:node_105,prsc:2,frmn:0,frmx:0.35,tomn:10,tomx:1|IN-7216-OUT;n:type:ShaderForge.SFN_ConstantClamp,id:3265,x:32980,y:33353,varname:node_3265,prsc:2,min:1,max:10|IN-105-OUT;n:type:ShaderForge.SFN_Desaturate,id:8186,x:33661,y:33781,varname:node_8186,prsc:2|COL-1449-OUT,DES-4108-OUT;n:type:ShaderForge.SFN_RemapRange,id:4108,x:31707,y:34103,varname:node_4108,prsc:2,frmn:0,frmx:1,tomn:0.5,tomx:0|IN-4293-OUT;n:type:ShaderForge.SFN_Multiply,id:7992,x:33661,y:33922,varname:node_7992,prsc:2|A-4653-OUT,B-4293-OUT;n:type:ShaderForge.SFN_Slider,id:4293,x:30887,y:32278,ptovrint:False,ptlb:FusionOutput,ptin:_FusionOutput,varname:_FusionOutput,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Lerp,id:3517,x:31699,y:32405,varname:node_3517,prsc:2|A-1538-OUT,B-5685-UVOUT,T-8807-OUT;n:type:ShaderForge.SFN_Sign,id:8807,x:31418,y:32423,varname:node_8807,prsc:2|IN-4293-OUT;n:type:ShaderForge.SFN_Lerp,id:1031,x:31698,y:33232,varname:node_1031,prsc:2|A-3274-OUT,B-4900-UVOUT,T-8807-OUT;proporder:1436-7216-177-6665-7714-4488-7925-2579-1813-358-4293;pass:END;sub:END;*/

Shader "Shader Forge/SDR_AmorphousBlade_SR" {
    Properties {
        _MasksRGB ("Masks RGB", 2D) = "white" {}
        _Ignition ("Ignition", Range(0, 1)) = 0
        _IgnitionColor ("IgnitionColor", Color) = (0.9411765,0.5803922,1,1)
        _ColorA ("ColorA", Color) = (1,0.5372549,0.909804,1)
        _PaternSizeA ("PaternSizeA", Float ) = 2
        _ColorB ("ColorB", Color) = (0.4470589,0,1,1)
        _PaternSizeB ("PaternSizeB", Float ) = 1.5
        _ColorsBloom ("ColorsBloom", Range(1, 3)) = 2
        _Gloss ("Gloss", Range(0, 1)) = 0.8
        _Metallic ("Metallic", Range(0, 1)) = 0
        _FusionOutput ("FusionOutput", Range(0, 1)) = 0
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "Queue"="AlphaTest"
            "RenderType"="TransparentCutout"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _ColorA;
            uniform float _Metallic;
            uniform float _Gloss;
            uniform sampler2D _MasksRGB; uniform float4 _MasksRGB_ST;
            uniform float _PaternSizeA;
            uniform float _PaternSizeB;
            uniform float4 _ColorB;
            uniform float _ColorsBloom;
            uniform float _Ignition;
            uniform float4 _IgnitionColor;
            uniform float _FusionOutput;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
                float2 texcoord3 : TEXCOORD3;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float2 uv3 : TEXCOORD3;
                float4 posWorld : TEXCOORD4;
                float3 normalDir : TEXCOORD5;
                float3 tangentDir : TEXCOORD6;
                float3 bitangentDir : TEXCOORD7;
                LIGHTING_COORDS(8,9)
                UNITY_FOG_COORDS(10)
                #if defined(LIGHTMAP_ON) || defined(UNITY_SHOULD_SAMPLE_SH)
                    float4 ambientOrLightmapUV : TEXCOORD11;
                #endif
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.uv3 = v.texcoord3;
                #ifdef LIGHTMAP_ON
                    o.ambientOrLightmapUV.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                    o.ambientOrLightmapUV.zw = 0;
                #elif UNITY_SHOULD_SAMPLE_SH
                #endif
                #ifdef DYNAMICLIGHTMAP_ON
                    o.ambientOrLightmapUV.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
                #endif
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float node_4588 = ceil(((_Ignition*1.428571+-0.4285715)-i.uv3.g));
                float4 _node_2755 = tex2D(_MasksRGB,TRANSFORM_TEX(i.uv3, _MasksRGB));
                clip((node_4588+ceil((clamp((_Ignition*2.857143+0.0),0.0,0.95)-(1.0 - _node_2755.b)))) - 0.5);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                UNITY_LIGHT_ATTENUATION(attenuation,i, i.posWorld.xyz);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float gloss = _Gloss;
                float perceptualRoughness = 1.0 - _Gloss;
                float roughness = perceptualRoughness * perceptualRoughness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
/////// GI Data:
                UnityLight light;
                #ifdef LIGHTMAP_OFF
                    light.color = lightColor;
                    light.dir = lightDirection;
                    light.ndotl = LambertTerm (normalDirection, light.dir);
                #else
                    light.color = half3(0.f, 0.f, 0.f);
                    light.ndotl = 0.0f;
                    light.dir = half3(0.f, 0.f, 0.f);
                #endif
                UnityGIInput d;
                d.light = light;
                d.worldPos = i.posWorld.xyz;
                d.worldViewDir = viewDirection;
                d.atten = attenuation;
                #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
                    d.ambient = 0;
                    d.lightmapUV = i.ambientOrLightmapUV;
                #else
                    d.ambient = i.ambientOrLightmapUV;
                #endif
                #if UNITY_SPECCUBE_BLENDING || UNITY_SPECCUBE_BOX_PROJECTION
                    d.boxMin[0] = unity_SpecCube0_BoxMin;
                    d.boxMin[1] = unity_SpecCube1_BoxMin;
                #endif
                #if UNITY_SPECCUBE_BOX_PROJECTION
                    d.boxMax[0] = unity_SpecCube0_BoxMax;
                    d.boxMax[1] = unity_SpecCube1_BoxMax;
                    d.probePosition[0] = unity_SpecCube0_ProbePosition;
                    d.probePosition[1] = unity_SpecCube1_ProbePosition;
                #endif
                d.probeHDR[0] = unity_SpecCube0_HDR;
                d.probeHDR[1] = unity_SpecCube1_HDR;
                Unity_GlossyEnvironmentData ugls_en_data;
                ugls_en_data.roughness = 1.0 - gloss;
                ugls_en_data.reflUVW = viewReflectDirection;
                UnityGI gi = UnityGlobalIllumination(d, 1, normalDirection, ugls_en_data );
                lightDirection = gi.light.dir;
                lightColor = gi.light.color;
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float3 specularColor = _Metallic;
                float specularMonochrome;
                float2 node_1538 = (i.uv0*_PaternSizeA);
                float4 node_9515 = _Time;
                float node_8807 = sign(_FusionOutput);
                float2 node_3517 = lerp(node_1538,(node_1538+node_9515.g*float2(-0.3,-1)),node_8807);
                float4 node_7736 = tex2D(_MasksRGB,TRANSFORM_TEX(node_3517, _MasksRGB));
                float2 node_3274 = (i.uv0*_PaternSizeB);
                float2 node_1031 = lerp(node_3274,(node_3274+node_9515.g*float2(0.3,-1)),node_8807);
                float4 node_4720 = tex2D(_MasksRGB,TRANSFORM_TEX(node_1031, _MasksRGB));
                float3 node_1449 = lerp((_ColorA.rgb*_ColorsBloom),(_ColorB.rgb*_ColorsBloom),( i.uv0.g > 0.5 ? (1.0-(1.0-2.0*(i.uv0.g-0.5))*(1.0-(0.5 - 2.0*(node_7736.r-0.5)*(node_4720.r-0.5)))) : (2.0*i.uv0.g*(0.5 - 2.0*(node_7736.r-0.5)*(node_4720.r-0.5))) ));
                float3 diffuseColor = lerp(node_1449,dot(node_1449,float3(0.3,0.59,0.11)),(_FusionOutput*-0.5+0.5)); // Need this for specular when using metallic
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, specularColor, specularColor, specularMonochrome );
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = abs(dot( normalDirection, viewDirection ));
                float NdotH = saturate(dot( normalDirection, halfDirection ));
                float VdotH = saturate(dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, roughness );
                float normTerm = GGXTerm(NdotH, roughness);
                float specularPBL = (visTerm*normTerm) * UNITY_PI;
                #ifdef UNITY_COLORSPACE_GAMMA
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                #endif
                specularPBL = max(0, specularPBL * NdotL);
                #if defined(_SPECULARHIGHLIGHTS_OFF)
                    specularPBL = 0.0;
                #endif
                half surfaceReduction;
                #ifdef UNITY_COLORSPACE_GAMMA
                    surfaceReduction = 1.0-0.28*roughness*perceptualRoughness;
                #else
                    surfaceReduction = 1.0/(roughness*roughness + 1.0);
                #endif
                specularPBL *= any(specularColor) ? 1.0 : 0.0;
                float3 directSpecular = attenColor*specularPBL*FresnelTerm(specularColor, LdotH);
                half grazingTerm = saturate( gloss + specularMonochrome );
                float3 indirectSpecular = (gi.indirect.specular);
                indirectSpecular *= FresnelLerp (specularColor, grazingTerm, NdotV);
                indirectSpecular *= surfaceReduction;
                float3 specular = (directSpecular + indirectSpecular);
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += gi.indirect.diffuse;
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
////// Emissive:
                float3 emissive = (lerp(node_1449,((_node_2755.b*_IgnitionColor.rgb)*clamp((_Ignition*-25.71429+10.0),1,10)),(1.0 - node_4588))*_FusionOutput);
/// Final Color:
                float3 finalColor = diffuse + specular + emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _ColorA;
            uniform float _Metallic;
            uniform float _Gloss;
            uniform sampler2D _MasksRGB; uniform float4 _MasksRGB_ST;
            uniform float _PaternSizeA;
            uniform float _PaternSizeB;
            uniform float4 _ColorB;
            uniform float _ColorsBloom;
            uniform float _Ignition;
            uniform float4 _IgnitionColor;
            uniform float _FusionOutput;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
                float2 texcoord3 : TEXCOORD3;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float2 uv3 : TEXCOORD3;
                float4 posWorld : TEXCOORD4;
                float3 normalDir : TEXCOORD5;
                float3 tangentDir : TEXCOORD6;
                float3 bitangentDir : TEXCOORD7;
                LIGHTING_COORDS(8,9)
                UNITY_FOG_COORDS(10)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.uv3 = v.texcoord3;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float node_4588 = ceil(((_Ignition*1.428571+-0.4285715)-i.uv3.g));
                float4 _node_2755 = tex2D(_MasksRGB,TRANSFORM_TEX(i.uv3, _MasksRGB));
                clip((node_4588+ceil((clamp((_Ignition*2.857143+0.0),0.0,0.95)-(1.0 - _node_2755.b)))) - 0.5);
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                UNITY_LIGHT_ATTENUATION(attenuation,i, i.posWorld.xyz);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float gloss = _Gloss;
                float perceptualRoughness = 1.0 - _Gloss;
                float roughness = perceptualRoughness * perceptualRoughness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float3 specularColor = _Metallic;
                float specularMonochrome;
                float2 node_1538 = (i.uv0*_PaternSizeA);
                float4 node_5698 = _Time;
                float node_8807 = sign(_FusionOutput);
                float2 node_3517 = lerp(node_1538,(node_1538+node_5698.g*float2(-0.3,-1)),node_8807);
                float4 node_7736 = tex2D(_MasksRGB,TRANSFORM_TEX(node_3517, _MasksRGB));
                float2 node_3274 = (i.uv0*_PaternSizeB);
                float2 node_1031 = lerp(node_3274,(node_3274+node_5698.g*float2(0.3,-1)),node_8807);
                float4 node_4720 = tex2D(_MasksRGB,TRANSFORM_TEX(node_1031, _MasksRGB));
                float3 node_1449 = lerp((_ColorA.rgb*_ColorsBloom),(_ColorB.rgb*_ColorsBloom),( i.uv0.g > 0.5 ? (1.0-(1.0-2.0*(i.uv0.g-0.5))*(1.0-(0.5 - 2.0*(node_7736.r-0.5)*(node_4720.r-0.5)))) : (2.0*i.uv0.g*(0.5 - 2.0*(node_7736.r-0.5)*(node_4720.r-0.5))) ));
                float3 diffuseColor = lerp(node_1449,dot(node_1449,float3(0.3,0.59,0.11)),(_FusionOutput*-0.5+0.5)); // Need this for specular when using metallic
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, specularColor, specularColor, specularMonochrome );
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = abs(dot( normalDirection, viewDirection ));
                float NdotH = saturate(dot( normalDirection, halfDirection ));
                float VdotH = saturate(dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, roughness );
                float normTerm = GGXTerm(NdotH, roughness);
                float specularPBL = (visTerm*normTerm) * UNITY_PI;
                #ifdef UNITY_COLORSPACE_GAMMA
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                #endif
                specularPBL = max(0, specularPBL * NdotL);
                #if defined(_SPECULARHIGHLIGHTS_OFF)
                    specularPBL = 0.0;
                #endif
                specularPBL *= any(specularColor) ? 1.0 : 0.0;
                float3 directSpecular = attenColor*specularPBL*FresnelTerm(specularColor, LdotH);
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor * 1,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            Cull Back
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _MasksRGB; uniform float4 _MasksRGB_ST;
            uniform float _Ignition;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
                float2 texcoord3 : TEXCOORD3;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float2 uv3 : TEXCOORD3;
                float4 posWorld : TEXCOORD4;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.uv3 = v.texcoord3;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float node_4588 = ceil(((_Ignition*1.428571+-0.4285715)-i.uv3.g));
                float4 _node_2755 = tex2D(_MasksRGB,TRANSFORM_TEX(i.uv3, _MasksRGB));
                clip((node_4588+ceil((clamp((_Ignition*2.857143+0.0),0.0,0.95)-(1.0 - _node_2755.b)))) - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
        Pass {
            Name "Meta"
            Tags {
                "LightMode"="Meta"
            }
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #include "UnityMetaPass.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _ColorA;
            uniform float _Metallic;
            uniform float _Gloss;
            uniform sampler2D _MasksRGB; uniform float4 _MasksRGB_ST;
            uniform float _PaternSizeA;
            uniform float _PaternSizeB;
            uniform float4 _ColorB;
            uniform float _ColorsBloom;
            uniform float _Ignition;
            uniform float4 _IgnitionColor;
            uniform float _FusionOutput;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
                float2 texcoord3 : TEXCOORD3;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float2 uv3 : TEXCOORD3;
                float4 posWorld : TEXCOORD4;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.uv3 = v.texcoord3;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST );
                return o;
            }
            float4 frag(VertexOutput i) : SV_Target {
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                UnityMetaInput o;
                UNITY_INITIALIZE_OUTPUT( UnityMetaInput, o );
                
                float2 node_1538 = (i.uv0*_PaternSizeA);
                float4 node_3818 = _Time;
                float node_8807 = sign(_FusionOutput);
                float2 node_3517 = lerp(node_1538,(node_1538+node_3818.g*float2(-0.3,-1)),node_8807);
                float4 node_7736 = tex2D(_MasksRGB,TRANSFORM_TEX(node_3517, _MasksRGB));
                float2 node_3274 = (i.uv0*_PaternSizeB);
                float2 node_1031 = lerp(node_3274,(node_3274+node_3818.g*float2(0.3,-1)),node_8807);
                float4 node_4720 = tex2D(_MasksRGB,TRANSFORM_TEX(node_1031, _MasksRGB));
                float3 node_1449 = lerp((_ColorA.rgb*_ColorsBloom),(_ColorB.rgb*_ColorsBloom),( i.uv0.g > 0.5 ? (1.0-(1.0-2.0*(i.uv0.g-0.5))*(1.0-(0.5 - 2.0*(node_7736.r-0.5)*(node_4720.r-0.5)))) : (2.0*i.uv0.g*(0.5 - 2.0*(node_7736.r-0.5)*(node_4720.r-0.5))) ));
                float4 _node_2755 = tex2D(_MasksRGB,TRANSFORM_TEX(i.uv3, _MasksRGB));
                float node_4588 = ceil(((_Ignition*1.428571+-0.4285715)-i.uv3.g));
                o.Emission = (lerp(node_1449,((_node_2755.b*_IgnitionColor.rgb)*clamp((_Ignition*-25.71429+10.0),1,10)),(1.0 - node_4588))*_FusionOutput);
                
                float3 diffColor = lerp(node_1449,dot(node_1449,float3(0.3,0.59,0.11)),(_FusionOutput*-0.5+0.5));
                float specularMonochrome;
                float3 specColor;
                diffColor = DiffuseAndSpecularFromMetallic( diffColor, _Metallic, specColor, specularMonochrome );
                float roughness = 1.0 - _Gloss;
                o.Albedo = diffColor + specColor * roughness * roughness * 0.5;
                
                return UnityMetaFragment( o );
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
