//rotate order ZXY
float4x4 RotZMatrix(float psi){
    float4x4 m = float4x4(
        cos(psi), -sin(psi), 0.0, 0.0,
        sin(psi), cos(psi), 0.0, 0.0,
        0.0, 0.0, 1.0, 0.0,
        0.0, 0.0, 0.0, 1.0
    );
	return m;
}
float4x4 RotXMatrix(float phi){
    float4x4 m = float4x4(
        1.0, 0.0, 0.0, 0.0,
        0.0, cos(phi), -sin(phi), 0.0,
        0.0, sin(phi), cos(phi), 0.0,
        0.0, 0.0, 0.0, 1.0
    );
	return m;
}
float4x4 RotYMatrix(float theta){
    float4x4 m = float4x4(
        cos(theta), 0.0, sin(theta), 0.0,
        0.0, 1.0, 0.0, 0.0,
        -sin(theta), 0.0, cos(theta), 0.0,
        0.0, 0.0, 0.0, 1.0
    );
	return m;
}

//up convert forward
float4 QuaterniontLookRotation(float3 forward, float3 up)
{
    float4 quaternion = float4(0.0, 0.0, 0.0, 0.0);
    forward = normalize(forward);
    up = normalize(up);
    float3 v1 = forward;
    float3 v2 = cross(up, v1);
    float3 v3 = cross(v1, v2);



    float m00 = v2.x;
    float m01 = v2.y;
    float m02 = v2.z;
    float m10 = v3.x;
    float m11 = v3.y;
    float m12 = v3.z;
    float m20 = v1.x;
    float m21 = v1.y;
    float m22 = v1.z;

    float num8 = (m00 + m11) + m22;
    if (num8 > 0){
         float num = sqrt(num8 + 1.0);
         quaternion.w = num * 0.5;
         num = 0.5 / num;
         quaternion.x = (m12 - m21) * num;
         quaternion.y = (m20 - m02) * num;
         quaternion.z = (m01 - m10) * num;

    }else if ((m00 >= m11) && (m00 >= m22)){
         float num7 = sqrt(1.0 + m00 - m11 - m22);
         float num4 = 0.5 / num7;
         quaternion.x = 0.5 * num7;
         quaternion.y = (m01 + m10) * num4;
         quaternion.z = (m02 + m20) * num4;
         quaternion.w = (m12 - m21) * num4;
    }else if (m11 > m22){
         float num6 = sqrt(1.0 + m11 - m00 - m22);
         float num3 = 0.5 / num6;
         quaternion.x = (m10+ m01) * num3;
         quaternion.y = 0.5 * num6;
         quaternion.z = (m21 + m12) * num3;
         quaternion.w = (m20 - m02) * num3;
    }else{
        float num5 = sqrt(1.0 + m22 - m00 - m11);
        float num2 = 0.5 / num5;
        quaternion.x = (m20 + m02) * num2;
        quaternion.y = (m21 + m12) * num2;
        quaternion.z = 0.5 * num5;
        quaternion.w = (m01 - m10) * num2;
    }

    return  quaternion;

}

float4x4 Quaternion2Matrix(float4 quaternion){
    float4 q = quaternion;
    float x = q.x;
    float z = q.y;
    float y = q.z;
    float w = q.w;

    float4x4 m = float4x4(
        1.0-2.0*y*y-2.0*z*z, 2.0*x*y+2.0*w*z, 2.0*x*z-2.0*w*y, 0.0,
        2.0*x*y-2.0*w*z, 1.0-2.0*x*x-2.0*z*z, 2.0*y*z+2.0*w*x, 0.0,
        2.0*x*z+2.0*w*y, 2.0*y*z-2.0*w*x, 1.0-2.0*x*x-2.0*y*y, 0.0,
        0.0, 0.0, 0.0, 1.0
    );

    return m;
}

//asinの定義域が問題
float3 Quaterniont2Eular(float4 quaternion){
    float PI = 3.14159265;
    float4 q = quaternion;
    q = q / length(q);
    float rotz = atan2(2.0 * (q.x * q.y + q.z * q.w), 1.0 - 2.0 * (q.y * q.y + q.z * q.z));
    float roty = asin(2 * (q.x * q.z - q.w * q.y));
    float rotx = atan2(2.0 * (q.x * q.w + q.y * q.z), 1.0 - 2.0 * (q.z * q.z + q.w * q.w));


    //float3 rot = float3(rotx * 2.0, roty * 2.0, rotz * 2.0);
    float3 rot = float3(rotx, roty, rotz);
    return rot;
}

float4x4 MultiRotationMatrix(float3 rotation){
    float a = rotation.x;
    float b = rotation.y;
    float c = rotation.z;
    float4x4 m = float4x4(
        cos(a) * cos(b) * cos(c) - sin(a) * sin(c), -cos(a) * cos(b) * sin(c) - sin(a) * cos(c), cos(a) * sin(b), 0.0,
        sin(a) * cos(b) * cos(c) + cos(a) * sin(c), -sin(a) * cos(b) * sin(c) + cos(a) * cos(c), sin(a) * sin(b), 0.0,
        -sin(b) * cos(c), sin(b) * sin(c), cos(b), 0.0,
        0.0, 0.0, 0.0, 1.0
    );
    return m;
}


float4x4 TranslateMatrix(float3 position){
	float4x4 m = float4x4(
		1.0, 0.0, 0.0, position.x,
		0.0, 1.0, 0.0, position.y,
		0.0, 0.0, 1.0, position.z,
		0.0, 0.0, 0.0, 1.0
		);
	return m;
}

float4x4 ScaleMatrix(float3 scale) {
	float4x4 m = float4x4(
		scale.x, 0.0, 0.0, 0.0,
		0.0, scale.y, 0.0, 0.0,
		0.0, 0.0, scale.z, 0.0,
		0.0, 0.0, 0.0, 1.0
		);
	return m;
}

float4x4 Rodrigues(float3 axis, float theta){
    float3 n = normalize(axis);
    float4x4 m = float4x4(
        cos(theta)+n.x*n.x*(1.0-cos(theta)),        n.x*n.y*(1.0-cos(theta))-n.z*sin(theta),    n.x*n.z*(1.0-cos(theta))+n.y*sin(theta),    0.0,
        n.y*n.x*(1.0-cos(theta))+n.z*sin(theta),    cos(theta)+n.y*n.y*(1.0-cos(theta)),        n.y*n.z*(1.0-cos(theta))-n.x*sin(theta),    0.0,
        n.z*n.x*(1.0-cos(theta))-n.y*sin(theta),    n.z*n.y*(1.0-cos(theta))+n.x*sin(theta),    cos(theta)+n.z*n.z*(1.0-cos(theta)),        0.0,
        0.0, 0.0, 0.0, 1.0
    );
	return m;
}