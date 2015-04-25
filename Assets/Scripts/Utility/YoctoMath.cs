//----------------------------------------------
// Desc: 数学方法类
// Date: 2015-04-23
// Author: Siny
// CopyRight: Siny
// ---------------------------------------------
using UnityEngine;
using System.Collections;

public static class YoctoMath 
{
	public static Vector2 Vec2(Vector3 vec3)
	{
		return new Vector2(vec3.x, vec3.y);
	}
	
	public static Vector3 Vec3(Vector2 vec2)
	{
		return new Vector3(vec2.x, vec2.y, 0);
	}
}
