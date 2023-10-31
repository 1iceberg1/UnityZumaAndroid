using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Spine.Unity
{
	[ExecuteInEditMode]
	[AddComponentMenu("Spine/BoneFollower")]
	public class BoneFollower : MonoBehaviour
	{
		public SkeletonRenderer skeletonRenderer;

		[SpineBone("", "skeletonRenderer")]
		public string boneName;

		public bool followZPosition = true;

		public bool followBoneRotation = true;

		[Tooltip("Follows the skeleton's flip state by controlling this Transform's local scale.")]
		public bool followSkeletonFlip = true;

		[Tooltip("Follows the target bone's local scale. BoneFollower cannot inherit world/skewed scale because of UnityEngine.Transform property limitations.")]
		public bool followLocalScale;

		[FormerlySerializedAs("resetOnAwake")]
		public bool initializeOnAwake = true;

		[NonSerialized]
		public bool valid;

		[NonSerialized]
		public Bone bone;

		private Transform skeletonTransform;

		public SkeletonRenderer SkeletonRenderer
		{
			get
			{
				return skeletonRenderer;
			}
			set
			{
				skeletonRenderer = value;
				Initialize();
			}
		}

		public void Awake()
		{
			if (initializeOnAwake)
			{
				Initialize();
			}
		}

		public void HandleRebuildRenderer(SkeletonRenderer skeletonRenderer)
		{
			Initialize();
		}

		public void Initialize()
		{
			bone = null;
			valid = (skeletonRenderer != null && skeletonRenderer.valid);
			if (valid)
			{
				skeletonTransform = skeletonRenderer.transform;
				SkeletonRenderer obj = skeletonRenderer;
				obj.OnRebuild = (SkeletonRenderer.SkeletonRendererDelegate)Delegate.Remove(obj.OnRebuild, new SkeletonRenderer.SkeletonRendererDelegate(HandleRebuildRenderer));
				SkeletonRenderer obj2 = skeletonRenderer;
				obj2.OnRebuild = (SkeletonRenderer.SkeletonRendererDelegate)Delegate.Combine(obj2.OnRebuild, new SkeletonRenderer.SkeletonRendererDelegate(HandleRebuildRenderer));
				if (!string.IsNullOrEmpty(boneName))
				{
					bone = skeletonRenderer.skeleton.FindBone(boneName);
				}
			}
		}

		private void OnDestroy()
		{
			if (skeletonRenderer != null)
			{
				SkeletonRenderer obj = skeletonRenderer;
				obj.OnRebuild = (SkeletonRenderer.SkeletonRendererDelegate)Delegate.Remove(obj.OnRebuild, new SkeletonRenderer.SkeletonRendererDelegate(HandleRebuildRenderer));
			}
		}

		public void LateUpdate()
		{
			if (!valid)
			{
				Initialize();
				return;
			}
			if (bone == null)
			{
				if (string.IsNullOrEmpty(boneName))
				{
					return;
				}
				bone = skeletonRenderer.skeleton.FindBone(boneName);
				if (bone == null)
				{
					UnityEngine.Debug.LogError("Bone not found: " + boneName, this);
					return;
				}
			}
			Transform transform = base.transform;
			if (transform.parent == skeletonTransform)
			{
				Transform transform2 = transform;
				float worldX = bone.worldX;
				float worldY = bone.worldY;
				float z;
				if (followZPosition)
				{
					z = 0f;
				}
				else
				{
					Vector3 localPosition = transform.localPosition;
					z = localPosition.z;
				}
				transform2.localPosition = new Vector3(worldX, worldY, z);
				if (followBoneRotation)
				{
					transform.localRotation = Quaternion.Euler(0f, 0f, bone.WorldRotationX);
				}
			}
			else
			{
				Vector3 position = skeletonTransform.TransformPoint(new Vector3(bone.worldX, bone.worldY, 0f));
				if (!followZPosition)
				{
					Vector3 position2 = transform.position;
					position.z = position2.z;
				}
				transform.position = position;
				if (followBoneRotation)
				{
					Vector3 eulerAngles = skeletonTransform.rotation.eulerAngles;
					Transform transform3 = transform;
					float x = eulerAngles.x;
					float y = eulerAngles.y;
					Vector3 eulerAngles2 = skeletonTransform.rotation.eulerAngles;
					transform3.rotation = Quaternion.Euler(x, y, eulerAngles2.z + bone.WorldRotationX);
				}
			}
			Vector3 localScale = (!followLocalScale) ? Vector3.one : new Vector3(bone.scaleX, bone.scaleY, 1f);
			if (followSkeletonFlip)
			{
				localScale.y *= ((!(bone.skeleton.flipX ^ bone.skeleton.flipY)) ? 1f : (-1f));
			}
			transform.localScale = localScale;
		}
	}
}
