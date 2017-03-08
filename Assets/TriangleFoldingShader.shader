
Shader "Custom/TriangleFolding" 
{

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
					UNITY_VERTEX_INPUT_INSTANCE_ID
				};

				struct FS_INPUT
				{
					UNITY_VERTEX_INPUT_INSTANCE_ID
					float4 clipPos	: SV_POSITION;
					float3 normal	: NORMAL;
					float4 color	: COLOR;
				};
				


				// **************************************************************
				// Shader Programs												*
				// **************************************************************

				// Vertex Shader ------------------------------------------------
				GS_INPUT VS_Main(appdata_full v)
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


				uniform float4 colorBottom;
				uniform float4 colorFront;
				uniform float4 colorLeft;
				uniform float4 colorRight;


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
					output.color		= colorBottom;
						
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
					output.color = lerp(colorBottom, colorRight, animationProgress);				   						   
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
					output.color = lerp(colorBottom, colorLeft, animationProgress);						   						   
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
					output.color = lerp(colorBottom, colorFront, animationProgress);					   						   
					output.clipPos = UnityObjectToClipPos( p0_out );											
					vertices.Append(output);	   
					output.clipPos = UnityObjectToClipPos( p1_out );			
					vertices.Append(output);				 	
					output.clipPos = UnityObjectToClipPos( p2_out );					
					vertices.Append(output);			   
					//vertices.RestartStrip();												
				}

				

				uniform bool lightingEnabled;
				uniform float3 lightDir;
				uniform float4 lightColor;

				// Fragment Shader -----------------------------------------------
				float4 FS_Main(FS_INPUT input, bool isFrontFace : SV_IsFrontFace) : SV_Target
				{
					
					if(lightingEnabled)
					{
						float3 n = (isFrontFace) ? input.normal : -input.normal;
						half nl = max(0, dot(n, -lightDir));										
						return input.color * (nl + 0.2) * lightColor;
					}
					
					return input.color;				
				}
			ENDCG
		}
	} 
}
