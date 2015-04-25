//----------------------------------------------
// Desc: 输入封装层:处理实际键盘/触摸输入, 提供给逻辑控制层易用易理解的接口
// Date: 2015-04-25
// Author: Siny
// CopyRight: Siny
// ---------------------------------------------
using UnityEngine;
using System.Collections;

// YTODO:改成使用单例模版
public class InputManager : MonoBehaviour 
{
	public static InputManager Instance
	{
		get
		{
			InputManager instance = GameObject.FindObjectOfType<InputManager>();

			if (instance == null)
			{
				GameObject go = new GameObject("InputManager");
				instance = go.AddComponent<InputManager>();
			}

			return instance;
		}
	}

	static KeyCode keyLeft = KeyCode.A;
	static KeyCode keyRight = KeyCode.D;
//	static KeyCode keyUp = KeyCode.W;
//	static KeyCode keyDown = KeyCode.S;
	static KeyCode keyJump = KeyCode.K;
//	static KeyCode keyShoot = KeyCode.J;

	public float GetInputX()
	{
		float axis = 0;

		#if (UNITY_EDITOR || UNITY_STANDALONE)
		if (Input.GetKey(keyLeft))
		{
			axis = -1.0f;
		}

		if (Input.GetKey(keyRight))
		{
			axis = 1.0f;
		}
		#elif (UNITY_IOS || UNITY_ANDROID)
		// ...
		#endif

		return axis;
	}

	public bool GetJumpButton()
	{
		#if (UNITY_EDITOR || UNITY_STANDALONE)
		return Input.GetKey(keyJump);
		#elif (UNITY_IOS || UNITY_ANDROID)
		// ...
		return false;
		#endif
	}

	public bool GetJumpButtonDown()
	{
		#if (UNITY_EDITOR || UNITY_STANDALONE)
		return Input.GetKeyDown(keyJump);
		#elif (UNITY_IOS || UNITY_ANDROID)
		// ...
		return false;
		#endif
	}
	
	public bool GetJumpButtonUp()
	{
		#if (UNITY_EDITOR || UNITY_STANDALONE)
		return Input.GetKeyUp(keyJump);
		#elif (UNITY_IOS || UNITY_ANDROID)
		// ...
		return false;
		#endif
	}

}
