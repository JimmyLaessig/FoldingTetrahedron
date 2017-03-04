// Upgrade NOTE: replaced 'UNITY_INSTANCE_ID' with 'UNITY_VERTEX_INPUT_INSTANCE_ID'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'



Shader "Custom/Tetrahedron" 
{

	SubShader 
	{
		Pass
		{
			Tags { "RenderType"="Opaque" }
			LOD 200
		
			CGPROGRAM
				#pragma target 5.0
				#pragma vertex VS_Main
				#pragma fragment FS_Main
				#pragma geometry GS_Main
				#pragma multi_compile_instancing
				#include "UnityCG.cginc" 
				#include "UnityLightingCommon.cginc"


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
					float4 clipPos	: SV_POSITION;
					float3 normal	: NORMAL;
					float4 color	: COLOR;
					UNITY_VERTEX_INPUT_INSTANCE_ID
				};


				// **************************************************************
				// Shader Programs												*
				// **************************************************************

				// Vertex Shader ------------------------------------------------
				GS_INPUT VS_Main(appdata_base v)
				{
					GS_INPUT output = (GS_INPUT)0;
					
					UNITY_SETUP_INSTANCE_ID (v);
					UNITY_TRANSFER_INSTANCE_ID (v, output);

					output.clipPos	= UnityObjectToClipPos(v.vertex);
					output.localPos = v.vertex;
					output.worldPos = float4(UnityObjectToWorldDir(v.vertex.xyz), 1);
					return output;
				}




				// Geometry Shader -----------------------------------------------------
				[maxvertexcount(12)]
				void GS_Main(triangle GS_INPUT input[3], inout TriangleStream<FS_INPUT> vertices)
				{
					UNITY_SETUP_INSTANCE_ID (input[0]);
					

					float4 p00 = input[0].localPos;
					float4 p11 = input[1].localPos;
					float4 p22 = input[2].localPos;
					
					float4 p01 = (p00 + p11)/2.0f;	
					float4 p02 = (p00 + p22)/2.0f;	
					float4 p12 = (p11 + p22)/2.0f;
					

					// Calculate Normal
					float3 v01 = normalize(p11.xyz - p00.xyz);
					float3 v02 = normalize(p22.xyz - p00.xyz);	
					float3 localNormal = normalize(cross(v01, v02));	
					float3 worldNormal = UnityObjectToWorldNormal(localNormal);


					FS_INPUT output	= (FS_INPUT)0;
					UNITY_TRANSFER_INSTANCE_ID (input[0], output);

					output.normal		= worldNormal;	
					output.color		= float4(1, 0, 0, 1);	



					// Face 1					
					output.clipPos = UnityObjectToClipPos( p00 );											
					vertices.Append(output);	
					UNITY_TRANSFER_INSTANCE_ID (input[0], output);		   
					output.clipPos = UnityObjectToClipPos( p01 );	
					UNITY_TRANSFER_INSTANCE_ID (input[0], output);				
					vertices.Append(output);				 	
					output.clipPos = UnityObjectToClipPos( p02 );	
					UNITY_TRANSFER_INSTANCE_ID (input[0], output);						
					vertices.Append(output);			   
					vertices.RestartStrip();			   
														   
					// Face 2							   
					output.clipPos = UnityObjectToClipPos( p01 );					
					vertices.Append(output);			   														   
					output.clipPos = UnityObjectToClipPos( p11 );									   
					vertices.Append(output);			   									   
					output.clipPos = UnityObjectToClipPos( p12 );			
					vertices.Append(output);
					vertices.RestartStrip();

					// Face 3
					output.clipPos = UnityObjectToClipPos( p02 );					
					vertices.Append(output);			   											   
					output.clipPos = UnityObjectToClipPos( p12 );
					vertices.Append(output);			   														   
					output.clipPos = UnityObjectToClipPos( p22 );		
					vertices.Append(output);			   
					vertices.RestartStrip();			   
														   
					// Inner faces(Animated)			   
					output.color = float4(0, 1, 0, 1);		 
														   
					output.clipPos = UnityObjectToClipPos( p01 );	
					vertices.Append(output);
					output.clipPos = UnityObjectToClipPos( p12 );	
					vertices.Append(output);
					output.clipPos = UnityObjectToClipPos( p02 );				
					vertices.Append(output);					
				}



				// Fragment Shader -----------------------------------------------
				float4 FS_Main(FS_INPUT input) : SV_Target
				{
					UNITY_SETUP_INSTANCE_ID (input);

					half nl = max(0, dot(input.normal, _WorldSpaceLightPos0.xyz));
					// factor in the light color
					
					
					return input.color * nl * _LightColor0;
					//return float4(1, 0, 0, 1)* nl * _LightColor0;
				}
			ENDCG
		}
	} 
}
