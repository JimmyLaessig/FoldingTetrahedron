
Shader "Custom/TetrahedronFolding" 
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
				#pragma multi_compile_instancing
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
					bool isFrontFace: SV_IsFrontFace;
					
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



				uniform float cosPhi;
				uniform float sinPhi; 
				uniform float animationProgress;
				uniform bool animationForward;

				float4 _color;
				float4 _animatedColor;

				

				// Geometry Shader -----------------------------------------------------
				[maxvertexcount(18)]
				void GS_Main(triangle GS_INPUT input[3], inout TriangleStream<FS_INPUT> vertices)
				{
					UNITY_SETUP_INSTANCE_ID (input[0]);
					

					float3 p00 = input[0].localPos.xyz;
					float3 p11 = input[1].localPos.xyz;
					float3 p22 = input[2].localPos.xyz;
						 
					float3 p01 = (p00 + p11)/2.0f;					
					float3 p12 = (p11 + p22)/2.0f;
					float3 p20 = (p00 + p22)/2.0f;	

					float3 s = (p00 + p11 + p22) / 3.0f;					


					FS_INPUT output	= (FS_INPUT)0;
					UNITY_TRANSFER_INSTANCE_ID (input[0], output);

					// Normal and color for outer faces (static)
					output.normal		= UnityObjectToWorldNormal(CalculateNormal(p00, p11, p22));
					output.color		= _color;	


					// Face 1					
					output.clipPos = UnityObjectToClipPos( p00 );											
					vertices.Append(output);	   
					output.clipPos = UnityObjectToClipPos( p01 );			
					vertices.Append(output);				 	
					output.clipPos = UnityObjectToClipPos( p20 );					
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
					output.clipPos = UnityObjectToClipPos( p20 );					
					vertices.Append(output);			   											   
					output.clipPos = UnityObjectToClipPos( p12 );
					vertices.Append(output);			   														   
					output.clipPos = UnityObjectToClipPos( p22 );		
					vertices.Append(output);			   
					vertices.RestartStrip();			   
								
														   
					// Inner faces(Animated)	
					float delay		= 0.2;
					float i			= ( animationProgress < delay) ? 0 : ( animationProgress - delay)/ (1-delay);
					output.color	=  lerp(_color, _animatedColor, i);

					// Face 1		
					float3 p0_out	= p01;
					float3 p1_out	= p12;
					float3 p2_out	= RotateAround(p01, p12, s, cosPhi, sinPhi);
					float3 n_out	= UnityObjectToWorldNormal(CalculateNormal(p0_out, p1_out, p2_out));					
								
					output.normal = n_out;																						   
					output.clipPos = UnityObjectToClipPos( p0_out );	
					vertices.Append(output);
					output.clipPos = UnityObjectToClipPos( p1_out );	
					vertices.Append(output);
					output.clipPos = UnityObjectToClipPos( p2_out );				
					vertices.Append(output);	
					vertices.RestartStrip();	


					// Face 2
					p0_out	= p01;
					p1_out	= RotateAround(p20, p01, s, cosPhi, sinPhi);
					p2_out	= p20;
					n_out	= UnityObjectToWorldNormal(CalculateNormal(p0_out, p1_out, p2_out));

					output.normal = n_out;		
					output.clipPos = UnityObjectToClipPos( p0_out  );	
					vertices.Append(output);
					output.clipPos = UnityObjectToClipPos( p1_out  );	
					vertices.Append(output);				
					output.clipPos = UnityObjectToClipPos( p2_out  );				
					vertices.Append(output);	
					vertices.RestartStrip();	
					
					// Face3	
					p0_out	= RotateAround(p12, p20, s, cosPhi, sinPhi);;
					p1_out	= p12;
					p2_out	= p20;
					n_out	= UnityObjectToWorldNormal(CalculateNormal(p0_out, p1_out, p2_out));

			
					output.normal = n_out;													   
					output.clipPos = UnityObjectToClipPos( p0_out  );	
					vertices.Append(output);
					output.clipPos = UnityObjectToClipPos( p1_out  );	
					vertices.Append(output);
					output.clipPos = UnityObjectToClipPos( p2_out  );				
					vertices.Append(output);					
				}

				

				// Fragment Shader -----------------------------------------------
				float4 FS_Main(FS_INPUT input) : SV_Target
				{
					UNITY_SETUP_INSTANCE_ID (input);
					
					float3 n = (input.isFrontFace) ? input.normal : -input.normal;
					//float3 n = input.normal;
					half nl = max(0, dot(n, _WorldSpaceLightPos0.xyz));										
					return input.color * nl * _LightColor0;
				}

			ENDCG
		}
	} 
}
