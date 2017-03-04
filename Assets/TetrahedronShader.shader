
Shader "Custom/Tetrahedron" 
{
	SubShader 
	{
		Pass
		{
			Tags { "RenderType"="Opaque" }
			LOD 200
			//Cull off
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



				uniform float cosPhi;
				uniform float sinPhi; 


				float3 RotatePointAround(float3 axisPoint0, float3 axisPoint1, float3 p, float cosPhi, float sinPhi)
				{
					float3 pointOnAxis		= (axisPoint0 + axisPoint1) /2;
					float3 rotationAxis		= normalize(axisPoint1 - axisPoint0);

					float x = p.x;
					float y = p.y;
					float z = p.z;

					float a = pointOnAxis.x;
					float b = pointOnAxis.y;
					float c = pointOnAxis.z;

					float u = rotationAxis.x;
					float v = rotationAxis.y;
					float w = rotationAxis.z;


					float x_rotated = (a * (v*v + w*w) - u * (b*v + c*w - u*x - v*y - w*z)) * (1 - cosPhi) + x * cosPhi + (-c*v + b*w - w*y + v*z) * sinPhi;
					float y_rotated = (b * (u*u + w*w) - v * (a*u + c*w - u*x - v*y - w*z)) * (1 - cosPhi) + y * cosPhi + ( c*u - a*w + w*x - u*z) * sinPhi;
					float z_rotated = (c * (u*u + v*v) - w * (a*u + b*v - u*x - v*y - w*z)) * (1 - cosPhi) + z * cosPhi + (-b*u + a*v - v*x + u*y) * sinPhi;

					return float3(x_rotated, y_rotated, z_rotated);
				}


				float3 CalculateNormal(float3 p0, float3 p1, float3 p2)
				{
					float3 v01 = normalize(p1 - p0);
					float3 v02 = normalize(p2 - p0);	
					return normalize(cross(v01, v02));	
					
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

					// Calculate Normal
					


					FS_INPUT output	= (FS_INPUT)0;
					UNITY_TRANSFER_INSTANCE_ID (input[0], output);

					// Normal and color for outer faces (static)
					output.normal		= UnityObjectToWorldNormal(CalculateNormal(p00, p11, p22));
					output.color		= float4(1, 0, 0, 1);	


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
					output.color = float4(0, 1, 0, 1);		 

					// Face 1		
					float3 p0_out	= p01;
					float3 p1_out	= p12;
					float3 p2_out	= RotatePointAround(p01, p12, p20, cosPhi, sinPhi);
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
					p1_out	= RotatePointAround(p20, p01, p12, cosPhi, sinPhi);
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
					p0_out	= RotatePointAround(p12, p20, p01, cosPhi, sinPhi);;
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

					half nl = max(0, dot(input.normal, _WorldSpaceLightPos0.xyz));
					// factor in the light color
					
					
					return input.color * nl * _LightColor0;
					//return float4(1, 0, 0, 1)* nl * _LightColor0;
				}
			ENDCG
		}
	} 
}
