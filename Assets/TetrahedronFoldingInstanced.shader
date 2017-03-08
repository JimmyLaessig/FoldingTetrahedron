
Shader "Custom/TetrahedronFolding" 
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
				};


				// **************************************************************
				// Shader Programs												*
				// **************************************************************

				// Vertex Shader ------------------------------------------------
				GS_INPUT VS_Main(appdata_full v)
				{
					GS_INPUT output = (GS_INPUT)0;
					
					UNITY_SETUP_INSTANCE_ID (v);
					UNITY_TRANSFER_INSTANCE_ID (v, output);

					output.clipPos	= UnityObjectToClipPos(v.vertex);
					output.localPos = v.vertex;
					output.worldPos = float4(UnityObjectToWorldDir(v.vertex.xyz), 1);
					output.color	= v.color;
					return output;
				}


				uniform float cosPhi;
				uniform float sinPhi; 
				uniform float animationProgress;

				
				uniform float4 colorBottom;
				uniform float4 colorFront;
				uniform float4 colorLeft;
				uniform float4 colorRight;

				// Returns the correct colors for the new created triangles based on the faces current color
				void GetAnimatedColors(in float4 currentColor, out float4 animatedColor0, out float4 animatedColor1, out float4 animatedColor2)
				{
					// Yellow => bottom
					if(Equals(currentColor, colorBottom))
					{
						animatedColor0 = colorLeft;
						animatedColor1 = colorRight;
						animatedColor2 = colorFront;
					}
					// Green => Front
					else if(Equals(currentColor, colorFront))
					{
						animatedColor0 = colorRight;
						animatedColor1 = colorLeft;
						animatedColor2 = colorBottom;
					}
					// Red => Right
					else if(Equals(currentColor, colorRight))
					{
						animatedColor0 = colorLeft;
						animatedColor1 = colorFront;
						animatedColor2 = colorBottom;
					}
					// Blue => Left
					else if(Equals(currentColor, colorLeft))
					{
						animatedColor0 = colorFront;
						animatedColor1 = colorRight;
						animatedColor2 = colorBottom;
					}
				}


				FS_INPUT CreateVertex(float4 clipPos, float3 normal, float4 color)
				{
					FS_INPUT vertex	= (FS_INPUT)0;
					vertex.clipPos	= clipPos; 
					vertex.normal	= normal; 
					vertex.color	= color;
					return vertex;
				}


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

					// **************************************************************
					// Static Faces													*
					// **************************************************************
					float4 color	= input[0].color;			
					float3 normal	= UnityObjectToWorldNormal(CalculateNormal(p00, p11, p22));

					// Face 1
					vertices.Append(CreateVertex(UnityObjectToClipPos( p00 ), normal, color));
					vertices.Append(CreateVertex(UnityObjectToClipPos( p01 ), normal, color));			
					vertices.Append(CreateVertex(UnityObjectToClipPos( p20 ), normal, color));	
					vertices.RestartStrip();	
		   														   
					// Face 2	
					vertices.Append(CreateVertex(UnityObjectToClipPos( p01 ), normal, color));
					vertices.Append(CreateVertex(UnityObjectToClipPos( p11 ), normal, color));			
					vertices.Append(CreateVertex(UnityObjectToClipPos( p12 ), normal, color));	
					vertices.RestartStrip();		

					// Face 3
					vertices.Append(CreateVertex(UnityObjectToClipPos( p20 ), normal, color));
					vertices.Append(CreateVertex(UnityObjectToClipPos( p12 ), normal, color));			
					vertices.Append(CreateVertex(UnityObjectToClipPos( p22 ), normal, color));	
					vertices.RestartStrip();
			   

					// **************************************************************
					// Animated Faces												*
					// **************************************************************		
					float3 p0_out, p1_out, p2_out, n_out;
														   
					float delay		= 0.2;
					float i			= ( animationProgress < delay) ? 0 : ( animationProgress - delay)/ (1-delay);
					float4 animatedColor0, animatedColor1, animatedColor2;
					GetAnimatedColors(color, animatedColor0, animatedColor1, animatedColor2);
					

					// Face 0	
					p0_out	= RotateAround(p12, p20, s, cosPhi, sinPhi);;
					p1_out	= p12;
					p2_out	= p20;
					n_out	= UnityObjectToWorldNormal(CalculateNormal(p0_out, p1_out, p2_out));
					color	=  lerp(color, animatedColor0, animationProgress);

					vertices.Append(CreateVertex(UnityObjectToClipPos( p0_out ), n_out, color));
					vertices.Append(CreateVertex(UnityObjectToClipPos( p1_out ), n_out, color));			
					vertices.Append(CreateVertex(UnityObjectToClipPos( p2_out ), n_out, color));	
					vertices.RestartStrip();	

				
					// Face 1		
					p0_out	= p01;
					p1_out	= p12;
					p2_out	= RotateAround(p01, p12, s, cosPhi, sinPhi);
					n_out	= UnityObjectToWorldNormal(CalculateNormal(p0_out, p1_out, p2_out));					
					color	=  lerp(color, animatedColor1, animationProgress);

					vertices.Append(CreateVertex(UnityObjectToClipPos( p0_out ), n_out, color));
					vertices.Append(CreateVertex(UnityObjectToClipPos( p1_out ), n_out, color));			
					vertices.Append(CreateVertex(UnityObjectToClipPos( p2_out ), n_out, color));	
					vertices.RestartStrip();


					// Face 2
					p0_out	= p01;
					p1_out	= RotateAround(p20, p01, s, cosPhi, sinPhi);
					p2_out	= p20;
					n_out	= UnityObjectToWorldNormal(CalculateNormal(p0_out, p1_out, p2_out));	
					color	=  lerp(color, animatedColor2, animationProgress);

					vertices.Append(CreateVertex(UnityObjectToClipPos( p0_out ), n_out, color));
					vertices.Append(CreateVertex(UnityObjectToClipPos( p1_out ), n_out, color));			
					vertices.Append(CreateVertex(UnityObjectToClipPos( p2_out ), n_out, color));	
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
