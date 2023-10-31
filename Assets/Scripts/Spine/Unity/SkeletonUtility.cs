using System;
using System.Collections.Generic;
using UnityEngine;

namespace Spine.Unity
{
	[RequireComponent(typeof(ISkeletonAnimation))]
	[ExecuteInEditMode]
	public class SkeletonUtility : MonoBehaviour
	{
		public delegate void SkeletonUtilityDelegate();

		public Transform boneRoot;

		[HideInInspector]
		public SkeletonRenderer skeletonRenderer;

		[HideInInspector]
		public ISkeletonAnimation skeletonAnimation;

		[NonSerialized]
		public List<SkeletonUtilityBone> utilityBones = new List<SkeletonUtilityBone>();

		[NonSerialized]
		public List<SkeletonUtilityConstraint> utilityConstraints = new List<SkeletonUtilityConstraint>();

		protected bool hasTransformBones;

		protected bool hasUtilityConstraints;

		protected bool needToReprocessBones;

		public event SkeletonUtilityDelegate OnReset;

		public static PolygonCollider2D AddBoundingBoxGameObject(Skeleton skeleton, string skinName, string slotName, string attachmentName, Transform parent, bool isTrigger = true)
		{
			Skin skin = (!string.IsNullOrEmpty(skinName)) ? skeleton.data.FindSkin(skinName) : skeleton.data.defaultSkin;
			if (skin == null)
			{
				UnityEngine.Debug.LogError("Skin " + skinName + " not found!");
				return null;
			}
			Attachment attachment = skin.GetAttachment(skeleton.FindSlotIndex(slotName), attachmentName);
			if (attachment == null)
			{
				UnityEngine.Debug.LogFormat("Attachment in slot '{0}' named '{1}' not found in skin '{2}'.", slotName, attachmentName, skin.name);
				return null;
			}
			BoundingBoxAttachment boundingBoxAttachment = attachment as BoundingBoxAttachment;
			if (boundingBoxAttachment != null)
			{
				Slot slot = skeleton.FindSlot(slotName);
				return AddBoundingBoxGameObject(boundingBoxAttachment.Name, boundingBoxAttachment, slot, parent, isTrigger);
			}
			UnityEngine.Debug.LogFormat("Attachment '{0}' was not a Bounding Box.", attachmentName);
			return null;
		}

		public static PolygonCollider2D AddBoundingBoxGameObject(string name, BoundingBoxAttachment box, Slot slot, Transform parent, bool isTrigger = true)
		{
			GameObject gameObject = new GameObject("[BoundingBox]" + ((!string.IsNullOrEmpty(name)) ? name : box.Name));
			Transform transform = gameObject.transform;
			transform.parent = parent;
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			transform.localScale = Vector3.one;
			return AddBoundingBoxAsComponent(box, slot, gameObject, isTrigger);
		}

		public static PolygonCollider2D AddBoundingBoxAsComponent(BoundingBoxAttachment box, Slot slot, GameObject gameObject, bool isTrigger = true)
		{
			if (box == null)
			{
				return null;
			}
			PolygonCollider2D polygonCollider2D = gameObject.AddComponent<PolygonCollider2D>();
			polygonCollider2D.isTrigger = isTrigger;
			SetColliderPointsLocal(polygonCollider2D, slot, box);
			return polygonCollider2D;
		}

		public static void SetColliderPointsLocal(PolygonCollider2D collider, Slot slot, BoundingBoxAttachment box)
		{
			if (box != null)
			{
				if (box.IsWeighted())
				{
					UnityEngine.Debug.LogWarning("UnityEngine.PolygonCollider2D does not support weighted or animated points. Collider points will not be animated and may have incorrect orientation. If you want to use it as a collider, please remove weights and animations from the bounding box in Spine editor.");
				}
				Vector2[] localVertices = box.GetLocalVertices(slot, null);
				collider.SetPath(0, localVertices);
			}
		}

		public static Bounds GetBoundingBoxBounds(BoundingBoxAttachment boundingBox, float depth = 0f)
		{
			float[] vertices = boundingBox.Vertices;
			int num = vertices.Length;
			Bounds result = default(Bounds);
			result.center = new Vector3(vertices[0], vertices[1], 0f);
			for (int i = 2; i < num; i += 2)
			{
				result.Encapsulate(new Vector3(vertices[i], vertices[i + 1], 0f));
			}
			Vector3 size = result.size;
			size.z = depth;
			result.size = size;
			return result;
		}

		private void Update()
		{
			Skeleton skeleton = skeletonRenderer.skeleton;
			if (boneRoot != null && skeleton != null)
			{
				Vector3 one = Vector3.one;
				if (skeleton.FlipX)
				{
					one.x = -1f;
				}
				if (skeleton.FlipY)
				{
					one.y = -1f;
				}
				boneRoot.localScale = one;
			}
		}

		private void OnEnable()
		{
			if (skeletonRenderer == null)
			{
				skeletonRenderer = GetComponent<SkeletonRenderer>();
			}
			if (skeletonAnimation == null)
			{
				skeletonAnimation = GetComponent<SkeletonAnimation>();
				if (skeletonAnimation == null)
				{
					skeletonAnimation = GetComponent<SkeletonAnimator>();
				}
			}
			SkeletonRenderer obj = skeletonRenderer;
			obj.OnRebuild = (SkeletonRenderer.SkeletonRendererDelegate)Delegate.Remove(obj.OnRebuild, new SkeletonRenderer.SkeletonRendererDelegate(HandleRendererReset));
			SkeletonRenderer obj2 = skeletonRenderer;
			obj2.OnRebuild = (SkeletonRenderer.SkeletonRendererDelegate)Delegate.Combine(obj2.OnRebuild, new SkeletonRenderer.SkeletonRendererDelegate(HandleRendererReset));
			if (skeletonAnimation != null)
			{
				skeletonAnimation.UpdateLocal -= UpdateLocal;
				skeletonAnimation.UpdateLocal += UpdateLocal;
			}
			CollectBones();
		}

		private void Start()
		{
			CollectBones();
		}

		private void OnDisable()
		{
			SkeletonRenderer obj = skeletonRenderer;
			obj.OnRebuild = (SkeletonRenderer.SkeletonRendererDelegate)Delegate.Remove(obj.OnRebuild, new SkeletonRenderer.SkeletonRendererDelegate(HandleRendererReset));
			if (skeletonAnimation != null)
			{
				skeletonAnimation.UpdateLocal -= UpdateLocal;
				skeletonAnimation.UpdateWorld -= UpdateWorld;
				skeletonAnimation.UpdateComplete -= UpdateComplete;
			}
		}

		private void HandleRendererReset(SkeletonRenderer r)
		{
			if (this.OnReset != null)
			{
				this.OnReset();
			}
			CollectBones();
		}

		public void RegisterBone(SkeletonUtilityBone bone)
		{
			if (!utilityBones.Contains(bone))
			{
				utilityBones.Add(bone);
				needToReprocessBones = true;
			}
		}

		public void UnregisterBone(SkeletonUtilityBone bone)
		{
			utilityBones.Remove(bone);
		}

		public void RegisterConstraint(SkeletonUtilityConstraint constraint)
		{
			if (!utilityConstraints.Contains(constraint))
			{
				utilityConstraints.Add(constraint);
				needToReprocessBones = true;
			}
		}

		public void UnregisterConstraint(SkeletonUtilityConstraint constraint)
		{
			utilityConstraints.Remove(constraint);
		}

		public void CollectBones()
		{
			Skeleton skeleton = skeletonRenderer.skeleton;
			if (skeleton == null)
			{
				return;
			}
			if (boneRoot != null)
			{
				List<object> list = new List<object>();
				ExposedList<IkConstraint> ikConstraints = skeleton.IkConstraints;
				int i = 0;
				for (int count = ikConstraints.Count; i < count; i++)
				{
					list.Add(ikConstraints.Items[i].target);
				}
				ExposedList<TransformConstraint> transformConstraints = skeleton.TransformConstraints;
				int j = 0;
				for (int count2 = transformConstraints.Count; j < count2; j++)
				{
					list.Add(transformConstraints.Items[j].target);
				}
				List<SkeletonUtilityBone> list2 = utilityBones;
				int k = 0;
				for (int count3 = list2.Count; k < count3; k++)
				{
					SkeletonUtilityBone skeletonUtilityBone = list2[k];
					if (skeletonUtilityBone.bone != null)
					{
						hasTransformBones |= (skeletonUtilityBone.mode == SkeletonUtilityBone.Mode.Override);
						hasUtilityConstraints |= list.Contains(skeletonUtilityBone.bone);
					}
				}
				hasUtilityConstraints |= (utilityConstraints.Count > 0);
				if (skeletonAnimation != null)
				{
					skeletonAnimation.UpdateWorld -= UpdateWorld;
					skeletonAnimation.UpdateComplete -= UpdateComplete;
					if (hasTransformBones || hasUtilityConstraints)
					{
						skeletonAnimation.UpdateWorld += UpdateWorld;
					}
					if (hasUtilityConstraints)
					{
						skeletonAnimation.UpdateComplete += UpdateComplete;
					}
				}
				needToReprocessBones = false;
			}
			else
			{
				utilityBones.Clear();
				utilityConstraints.Clear();
			}
		}

		private void UpdateLocal(ISkeletonAnimation anim)
		{
			if (needToReprocessBones)
			{
				CollectBones();
			}
			List<SkeletonUtilityBone> list = utilityBones;
			if (list != null)
			{
				int i = 0;
				for (int count = list.Count; i < count; i++)
				{
					list[i].transformLerpComplete = false;
				}
				UpdateAllBones();
			}
		}

		private void UpdateWorld(ISkeletonAnimation anim)
		{
			UpdateAllBones();
			int i = 0;
			for (int count = utilityConstraints.Count; i < count; i++)
			{
				utilityConstraints[i].DoUpdate();
			}
		}

		private void UpdateComplete(ISkeletonAnimation anim)
		{
			UpdateAllBones();
		}

		private void UpdateAllBones()
		{
			if (boneRoot == null)
			{
				CollectBones();
			}
			List<SkeletonUtilityBone> list = utilityBones;
			if (list != null)
			{
				int i = 0;
				for (int count = list.Count; i < count; i++)
				{
					list[i].DoUpdate();
				}
			}
		}

		public Transform GetBoneRoot()
		{
			if (boneRoot != null)
			{
				return boneRoot;
			}
			boneRoot = new GameObject("SkeletonUtility-Root").transform;
			boneRoot.parent = base.transform;
			boneRoot.localPosition = Vector3.zero;
			boneRoot.localRotation = Quaternion.identity;
			boneRoot.localScale = Vector3.one;
			return boneRoot;
		}

		public GameObject SpawnRoot(SkeletonUtilityBone.Mode mode, bool pos, bool rot, bool sca)
		{
			GetBoneRoot();
			Skeleton skeleton = skeletonRenderer.skeleton;
			GameObject result = SpawnBone(skeleton.RootBone, boneRoot, mode, pos, rot, sca);
			CollectBones();
			return result;
		}

		public GameObject SpawnHierarchy(SkeletonUtilityBone.Mode mode, bool pos, bool rot, bool sca)
		{
			GetBoneRoot();
			Skeleton skeleton = skeletonRenderer.skeleton;
			GameObject result = SpawnBoneRecursively(skeleton.RootBone, boneRoot, mode, pos, rot, sca);
			CollectBones();
			return result;
		}

		public GameObject SpawnBoneRecursively(Bone bone, Transform parent, SkeletonUtilityBone.Mode mode, bool pos, bool rot, bool sca)
		{
			GameObject gameObject = SpawnBone(bone, parent, mode, pos, rot, sca);
			ExposedList<Bone> children = bone.Children;
			int i = 0;
			for (int count = children.Count; i < count; i++)
			{
				Bone bone2 = children.Items[i];
				SpawnBoneRecursively(bone2, gameObject.transform, mode, pos, rot, sca);
			}
			return gameObject;
		}

		public GameObject SpawnBone(Bone bone, Transform parent, SkeletonUtilityBone.Mode mode, bool pos, bool rot, bool sca)
		{
			GameObject gameObject = new GameObject(bone.Data.Name);
			gameObject.transform.parent = parent;
			SkeletonUtilityBone skeletonUtilityBone = gameObject.AddComponent<SkeletonUtilityBone>();
			skeletonUtilityBone.skeletonUtility = this;
			skeletonUtilityBone.position = pos;
			skeletonUtilityBone.rotation = rot;
			skeletonUtilityBone.scale = sca;
			skeletonUtilityBone.mode = mode;
			skeletonUtilityBone.zPosition = true;
			skeletonUtilityBone.Reset();
			skeletonUtilityBone.bone = bone;
			skeletonUtilityBone.boneName = bone.Data.Name;
			skeletonUtilityBone.valid = true;
			if (mode == SkeletonUtilityBone.Mode.Override)
			{
				if (rot)
				{
					gameObject.transform.localRotation = Quaternion.Euler(0f, 0f, skeletonUtilityBone.bone.AppliedRotation);
				}
				if (pos)
				{
					gameObject.transform.localPosition = new Vector3(skeletonUtilityBone.bone.X, skeletonUtilityBone.bone.Y, 0f);
				}
				gameObject.transform.localScale = new Vector3(skeletonUtilityBone.bone.scaleX, skeletonUtilityBone.bone.scaleY, 0f);
			}
			return gameObject;
		}
	}
}
