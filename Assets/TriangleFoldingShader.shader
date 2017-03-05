
Shader "Custom/TriangleFolding" 
{
	Properties
	{
	_color("color", Color) = (1, 0, 0, 1)
	_animatedColor("animatedColor", Color) = (0, 1, 0, 1)
	}
	SubShader 
	{
		Pass
		{
			Tags { "RenderType"="Opaque" }
			LOD 200
			Cull off
			CGPROGRAM
				#pragma target 5.0
				#pragma vertex VS_Main
				#pragma fragment FS_Main
				#pragma geometry GS_Main
				#include "UnityCG.cginc" 
				#include "UnityLightingCommon.cginc"
				#include "fun.cginc"

				// **************************************************************
				// Data structures												*
				// **************************************************************
				struct GS_INPUT
				{
					float4 clipPos	: POSITION;
					float4 localPos	: TEXCOORD0;
					float4 worldPos	: TEXCOORD1;
					float4 color	: COLOR;
					//UNITY_VERTEX_INPUT_INSTANCE_ID
				};

				struct FS_INPUT
				{
					float4 clipPos	: SV_POSITION;
					float3 normal	: NORMAL;
					float4 color	: COLOR;
					bool isFrontFace: SV_IsFrontFace;
					//UNITY_VERTEX_INPUT_INSTANCE_ID
				};


				// **************************************************************
				// Shader Programs												*
				// **************************************************************

				// Vertex Shader ------------------------------------------------
				GS_INPUT VS_Main(appdata_base v)
				{
					GS_INPUT output = (GS_INPUT)0;					

					output.clipPos	= UnityObjectToClipPos(v.vertex);
					output.localPos = v.vertex;
					output.worldPos = float4(UnityObjectToWorldDir(v.vertex.xyz), 1);
					return output;
				}



				uniform float cosPhi;
				uniform float sinPhi; 
				uniform float animationProgress;


				float4 _color;
				float4 _animatedColor;

				


				// Geometry Shader -----------------------------------------------------
				[maxvertexcount(12)]
				void GS_Main(triangle GS_INPUT input[3], inout TriangleStream<FS_INPUT> vertices)
				{
					

					float3 p00 = input[0].localPos.xyz;
					float3 p11 = input[1].localPos.xyz;
					float3 p22 = input[2].localPos.xyz;
						 
					float3 p01 = (p00 + p11)/2.0f;					
					float3 p12 = (p11 + p22)/2.0f;
					float3 p20 = (p00 + p22)/2.0f;	


					FS_INPUT output	= (FS_INPUT)0;
					float3 p0_out, p1_out, p2_out, n_out;
					// Inner face		
					p0_out	= p01;
					p1_out	= p12;
					p2_out	= p20;
					n_out	= UnityObjectToWorldNormal(CalculateNormal(p0_out, p1_out, p2_out));	

					// Normal and color for outer faces (static)
					output.normal		= n_out;
					output.color		= _color;
						
					// Inner face					
					output.clipPos = UnityObjectToClipPos( p0_out );											
					vertices.Append(output);	   
					output.clipPos = UnityObjectToClipPos( p1_out );			
					vertices.Append(output);				 	
					output.clipPos = UnityObjectToClipPos( p2_out );					
					vertices.Append(output);			   
					vertices.RestartStrip();			   
					
					// Face 1		
					p0_out	= p01;
					p1_out	= p20;
					p2_out	= RotateAround(p01, p20, p00, cosPhi, sinPhi);
					n_out	= UnityObjectToWorldNormal(CalculateNormal(p0_out, p1_out, p2_out));	
					
					output.normal = n_out;					   						   
					output.clipPos = UnityObjectToClipPos( p0_out );											
					vertices.Append(output);	   
					output.clipPos = UnityObjectToClipPos( p1_out );			
					vertices.Append(output);				 	
					output.clipPos = UnityObjectToClipPos( p2_out );					
					vertices.Append(output);			   
					vertices.RestartStrip();		


					// Face 2		
					p0_out	= p12;
					p1_out	= p01;
					p2_out	= RotateAround(p12, p01, p11, cosPhi, sinPhi);
					n_out	= UnityObjectToWorldNormal(CalculateNormal(p0_out, p1_out, p2_out));	
							
					output.normal = n_out;						   						   
					output.clipPos = UnityObjectToClipPos( p0_out );											
					vertices.Append(output);	   
					output.clipPos = UnityObjectToClipPos( p1_out );			
					vertices.Append(output);				 	
					output.clipPos = UnityObjectToClipPos( p2_out );					
					vertices.Append(output);			   
					vertices.RestartStrip();	


					// Face 3		
					p0_out	= p20;
					p1_out	= p12;
					p2_out	= RotateAround(p20, p12, p22, cosPhi, sinPhi);
					n_out	= UnityObjectToWorldNormal(CalculateNormal(p0_out, p1_out, p2_out));	
							
					output.normal = n_out;						   						   
					output.clipPos = UnityObjectToClipPos( p0_out );											
					vertices.Append(output);	   
					output.clipPos = UnityObjectToClipPos( p1_out );			
					vertices.Append(output);				 	
					output.clipPos = UnityObjectToClipPos( p2_out );					
					vertices.Append(output);			   
					vertices.RestartStrip();												
				}

				

				// Fragment Shader -----------------------------------------------
				float4 FS_Main(FS_INPUT input) : SV_Target
				{
					float3 n = (input.isFrontFace) ? input.normal : -input.normal;
					half nl = max(0, dot(n, _WorldSpaceLightPos0.xyz));										
					return input.color * nl * _LightColor0;
				}
			ENDCG
		}
	} 
}
