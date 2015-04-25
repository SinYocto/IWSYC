//----------------------------------------------
// Desc: kid角色控制器, 处理角色的物理碰撞和输入操控逻辑
// Date: 2015-04-25
// Author: Siny
// CopyRight: Siny
// ---------------------------------------------
using UnityEngine;
using System.Collections;

public class KidController : MonoBehaviour 
{
	// move
	public float moveSpeed = 5.0f;

	// gravity
	public float gravity = -10.0f;
	public float finalFallSpeedY = -14.0625f;

	// jump
	public float jumpSpeed = 13.28125f;
	public float extraJumpSpeed = 10.9375f;

	int mMaxJumpCounts = 2;
	int mCurJumpCount = 0;
	
	public Transform leftFoot;
	public Transform rightFoot;
	public Transform leftHead;
	public Transform rightHead;
	public Transform topFront;
	public Transform bottomFront;
	
	Transform mTransform;
	Vector3 mVelocity = Vector3.zero;
	
	bool mGrounded = false;
	bool mFacingRight = true;
	
	void Awake()
	{
		mTransform = base.transform;

		// 加上轻微的偏移以避免类似以下的情况
		// 如:将前部阻挡判断点略高于地面, 能防止在水平地面前进时被判定为前部阻挡, 无法前进
		leftFoot.Translate(0.01f, 0, 0);
		rightFoot.Translate(-0.01f, 0, 0);
		leftHead.Translate(0.01f, 0, 0);
		rightHead.Translate(-0.01f, 0, 0);
		topFront.Translate(0, -0.01f, 0);
		bottomFront.Translate(0, 0.01f, 0);
	}
	
	void OnTriggerEnter2D(Collider2D other)
	{
		Debug.Log(other.gameObject.name + " hit " + mTransform.name);
	}
	
	void Update()
	{
		// 水平方向速度
		mVelocity.x = InputManager.Instance.GetInputX() * moveSpeed;
		
		if((mFacingRight && mVelocity.x < 0) || 
		   (!mFacingRight && mVelocity.x > 0))
		{
			flip();
		}

		// 垂直方向速度
		if (mGrounded)
		{
			mVelocity.y = 0;
		}
		else
		{
			mVelocity.y += Time.deltaTime * gravity;
		}
		
		handleJumping();

		// 对目标位移进行阻挡判断, 根据阻挡进行修正
		Vector3 translation = mVelocity * Time.deltaTime;
		checkTargetPosCollision(ref translation);

		// 应用经阻挡修正后的位移
		mTransform.Translate(translation, Space.World);
	}

	// 进行头部, 脚部, 前部三个方位的射线阻挡判断, 修正位移
	void checkTargetPosCollision(ref Vector3 translation)
	{
		mGrounded = false;
		if (translation.y > 0)	// head
		{
			Vector3 targetTransY = new Vector3(0, translation.y, 0);	//注意只使用当前方向的分量, 下同
			Vector3 transLH = targetTransY;
			Vector3 transRH = targetTransY;
			
			bool hitLH = rayCast(leftHead.position, targetTransY, out transLH);
			bool hitRH = rayCast(rightHead.position, targetTransY, out transRH);
			
			translation.y = (transLH.y < transRH.y) ? transLH.y : transRH.y;

			if (hitLH || hitRH)
			{
				mVelocity.y = 0;	// 头部产生碰撞则将速度即刻减为0
			}
		}
		else  // foot
		{
			Vector3 targetTransY = new Vector3(0, translation.y, 0);
			Vector3 transLF = targetTransY;
			Vector3 transRF = targetTransY;
			
			bool hitLF = rayCast(leftFoot.position, targetTransY, out transLF);
			bool hitRF = rayCast(rightFoot.position, targetTransY, out transRF);
			
			translation.y = (transLF.y > transRF.y) ? transLF.y : transRF.y;
			
			mGrounded = hitLF || hitRF;
		}

		// front
		Vector3 targetTransX = new Vector3(translation.x, 0, 0);
		Vector3 transTF = targetTransX;
		Vector3 transBF = targetTransX;
		
		bool hitTF = rayCast(topFront.position, targetTransX, out transTF);
		bool hitBF = rayCast(bottomFront.position, targetTransX, out transBF);
		
		translation.x = (Mathf.Abs(transTF.x) < Mathf.Abs(transBF.x)) ? transTF.x : transBF.x;		
	}
	
	bool rayCast(Vector3 startPos, Vector3 translation, out Vector3 correctedTranslation)
	{
		bool hit = false;
		correctedTranslation = translation;
		
		RaycastHit2D hitInfo = Physics2D.Raycast(YoctoMath.Vec2(startPos), 
		                                         YoctoMath.Vec2(translation), 
		                                         translation.magnitude,
		                                         ~(YoctoHelper.Layer("Player")));
		
		if (hitInfo.collider != null)
		{
			hit = true;
			correctedTranslation = YoctoMath.Vec3(hitInfo.point) - startPos;
		}
		
		return hit;
	}

	void flip()
	{
		Vector3 localScale = mTransform.localScale;
		localScale.x *= -1.0f;
		mTransform.localScale = localScale;
		
		mFacingRight = !mFacingRight;
	}

	void handleJumping()
	{
		// 跳跃段数可用并按下跳跃键时, 以一定初始速度跳跃
		if (mCurJumpCount < mMaxJumpCounts && InputManager.Instance.GetJumpButtonDown())
		{
			float speed = (mCurJumpCount == 0) ? jumpSpeed : extraJumpSpeed;
			mVelocity.y = speed;
			mCurJumpCount++;
		}

		// 着地着重置跳跃计数
		if (mGrounded)
		{
			mCurJumpCount = 0;
		}

		// 跳跃上升过程中, 若松开跳跃键, 则减速(i wanna中实现不同按键时长不同跳跃高度的方式)
		if (mVelocity.y > 0 && InputManager.Instance.GetJumpButtonUp())
		{
			mVelocity.y *= 0.45f;
		}
	}
}
