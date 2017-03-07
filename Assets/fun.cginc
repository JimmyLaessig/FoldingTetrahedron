


bool Equals(float4 a, float4 b)
{
    return (a.x == b.x && a.y == b.y && a.z == b.z && a.w == b.w);
}


float3 RotateAround(float3 axisPoint0, float3 axisPoint1, float3 p, float cosPhi, float sinPhi)
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