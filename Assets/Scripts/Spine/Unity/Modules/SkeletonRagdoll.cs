using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spine.Unity.Modules
{
	[RequireComponent(typeof(SkeletonRenderer))]
	public class SkeletonRagdoll : MonoBehaviour
	{
		public class LayerFieldAttribute : PropertyAttribute
		{
		}

		private static Transform parentSpaceHelper;

		[Header("Hierarchy")]
		[SpineBone("", "")]
		public string startingBoneName = string.Empty;

		[SpineBone("", "")]
		public List<string> stopBoneNames = new List<string>();

		[Header("Parameters")]
		public bool applyOnStart;

		[Tooltip("Warning!  You will have to re-enable and tune mix values manually if attempting to remove the ragdoll system.")]
		public bool disableIK = true;

		public bool disableOtherConstraints;

		[Space(18f)]
		[Tooltip("Set RootRigidbody IsKinematic to true when Apply is called.")]
		public bool pinStartBone;

		[Tooltip("Enable Collision between adjacent ragdoll elements (IE: Neck and Head)")]
		public bool enableJointCollision;

		public bool useGravity = true;

		[Tooltip("If no BoundingBox Attachment is attached to a bone, this becomes the default Width or Radius of a Bone's ragdoll Rigidbody")]
		public float thickness = 0.125f;

		[Tooltip("Default rotational limit value.  Min is negative this value, Max is this value.")]
		public float rotationLimit = 20f;

		public float rootMass = 20f;

		[Tooltip("If your ragdoll seems unstable or uneffected by limits, try lowering this value.")]
		[Range(0.01f, 1f)]
		public float massFalloffFactor = 0.4f;

		[Tooltip("The layer assigned to all of the rigidbody parts.")]
		public int colliderLayer;

		[Range(0f, 1f)]
		public float mix = 1f;

		private ISkeletonAnimation targetSkeletonComponent;

		private Skeleton skeleton;

		private Dictionary<Bone, Transform> boneTable = new Dictionary<Bone, Transform>();

		private Transform ragdollRoot;

		private Vector3 rootOffset;

		private bool isActive;

		public Rigidbody RootRigidbody
		{
			get;
			private set;
		}

		public Bone StartingBone
		{
			get;
			private set;
		}

		public Vector3 RootOffset => rootOffset;

		public bool IsActive => isActive;

		public Rigidbody[] RigidbodyArray
		{
			get
			{
				if (!isActive)
				{
					return new Rigidbody[0];
				}
				Rigidbody[] array = new Rigidbody[boneTable.Count];
				int num = 0;
				foreach (Transform value in boneTable.Values)
				{
					array[num] = value.GetComponent<Rigidbody>();
					num++;
				}
				return array;
			}
		}

		public Vector3 EstimatedSkeletonPosition => RootRigidbody.position - rootOffset;

		private IEnumerator Start()
		{
			if (parentSpaceHelper == null)
			{
				parentSpaceHelper = new GameObject("Parent Space Helper").transform;
				parentSpaceHelper.hideFlags = HideFlags.HideInHierarchy;
			}
			targetSkeletonComponent = (GetComponent<SkeletonRenderer>() as ISkeletonAnimation);
			if (targetSkeletonComponent == null)
			{
				UnityEngine.Debug.LogError("Attached Spine component does not implement ISkeletonAnimation. This script is not compatible.");
			}
			skeleton = targetSkeletonComponent.Skeleton;
			if (applyOnStart)
			{
				yield return null;
				Apply();
			}
		}

		public void Apply()
		{
			isActive = true;
			mix = 1f;
			StartingBone = skeleton.FindBone(startingBoneName);
			RecursivelyCreateBoneProxies(StartingBone);
			RootRigidbody = boneTable[StartingBone].GetComponent<Rigidbody>();
			RootRigidbody.isKinematic = pinStartBone;
			RootRigidbody.mass = rootMass;
			List<Collider> list = new List<Collider>();
			foreach (KeyValuePair<Bone, Transform> item in boneTable)
			{
				Bone key = item.Key;
				Transform value = item.Value;
				list.Add(value.GetComponent<Collider>());
				Transform transform;
				if (key == StartingBone)
				{
					ragdollRoot = new GameObject("RagdollRoot").transform;
					ragdollRoot.SetParent(base.transform, worldPositionStays: false);
					if (key == skeleton.RootBone)
					{
						ragdollRoot.localPosition = new Vector3(key.WorldX, key.WorldY, 0f);
						ragdollRoot.localRotation = Quaternion.Euler(0f, 0f, GetPropagatedRotation(key));
					}
					else
					{
						ragdollRoot.localPosition = new Vector3(key.Parent.WorldX, key.Parent.WorldY, 0f);
						ragdollRoot.localRotation = Quaternion.Euler(0f, 0f, GetPropagatedRotation(key.Parent));
					}
					transform = ragdollRoot;
					rootOffset = value.position - base.transform.position;
				}
				else
				{
					transform = boneTable[key.Parent];
				}
				Rigidbody component = transform.GetComponent<Rigidbody>();
				if (component != null)
				{
					HingeJoint hingeJoint = value.gameObject.AddComponent<HingeJoint>();
					hingeJoint.connectedBody = component;
					Vector3 connectedAnchor = transform.InverseTransformPoint(value.position);
					connectedAnchor.x *= 1f;
					hingeJoint.connectedAnchor = connectedAnchor;
					hingeJoint.axis = Vector3.forward;
					hingeJoint.GetComponent<Rigidbody>().mass = hingeJoint.connectedBody.mass * massFalloffFactor;
					hingeJoint.limits = new JointLimits
					{
						min = 0f - rotationLimit,
						max = rotationLimit
					};
					hingeJoint.useLimits = true;
					hingeJoint.enableCollision = enableJointCollision;
				}
			}
			for (int i = 0; i < list.Count; i++)
			{
				for (int j = 0; j < list.Count; j++)
				{
					if (i != j)
					{
						Physics.IgnoreCollision(list[i], list[j]);
					}
				}
			}
			SkeletonUtilityBone[] componentsInChildren = GetComponentsInChildren<SkeletonUtilityBone>();
			if (componentsInChildren.Length > 0)
			{
				List<string> list2 = new List<string>();
				SkeletonUtilityBone[] array = componentsInChildren;
				foreach (SkeletonUtilityBone skeletonUtilityBone in array)
				{
					if (skeletonUtilityBone.mode == SkeletonUtilityBone.Mode.Override)
					{
						list2.Add(skeletonUtilityBone.gameObject.name);
						UnityEngine.Object.Destroy(skeletonUtilityBone.gameObject);
					}
				}
				if (list2.Count > 0)
				{
					string text = "Destroyed Utility Bones: ";
					for (int l = 0; l < list2.Count; l++)
					{
						text += list2[l];
						if (l != list2.Count - 1)
						{
							text += ",";
						}
					}
					UnityEngine.Debug.LogWarning(text);
				}
			}
			if (disableIK)
			{
				ExposedList<IkConstraint> ikConstraints = skeleton.IkConstraints;
				int m = 0;
				for (int count = ikConstraints.Count; m < count; m++)
				{
					ikConstraints.Items[m].mix = 0f;
				}
			}
			if (disableOtherConstraints)
			{
				ExposedList<TransformConstraint> transformConstraints = skeleton.transformConstraints;
				int n = 0;
				for (int count2 = transformConstraints.Count; n < count2; n++)
				{
					transformConstraints.Items[n].rotateMix = 0f;
					transformConstraints.Items[n].scaleMix = 0f;
					transformConstraints.Items[n].shearMix = 0f;
					transformConstraints.Items[n].translateMix = 0f;
				}
				ExposedList<PathConstraint> pathConstraints = skeleton.pathConstraints;
				int num = 0;
				for (int count3 = pathConstraints.Count; num < count3; num++)
				{
					pathConstraints.Items[num].rotateMix = 0f;
					pathConstraints.Items[num].translateMix = 0f;
				}
			}
			targetSkeletonComponent.UpdateWorld += UpdateSpineSkeleton;
		}

		public Coroutine SmoothMix(float target, float duration)
		{
			return StartCoroutine(SmoothMixCoroutine(target, duration));
		}

		private IEnumerator SmoothMixCoroutine(float target, float duration)
		{
			float startTime = Time.time;
			float startMix = mix;
			while (mix > 0f)
			{
				skeleton.SetBonesToSetupPose();
				mix = Mathf.SmoothStep(startMix, target, (Time.time - startTime) / duration);
				yield return null;
			}
		}

		public void SetSkeletonPosition(Vector3 worldPosition)
		{
			if (!isActive)
			{
				UnityEngine.Debug.LogWarning("Can't call SetSkeletonPosition while Ragdoll is not active!");
				return;
			}
			Vector3 vector = worldPosition - base.transform.position;
			base.transform.position = worldPosition;
			foreach (Transform value in boneTable.Values)
			{
				value.position -= vector;
			}
			UpdateSpineSkeleton(null);
			skeleton.UpdateWorldTransform();
		}

		public void Remove()
		{
			isActive = false;
			foreach (Transform value in boneTable.Values)
			{
				UnityEngine.Object.Destroy(value.gameObject);
			}
			UnityEngine.Object.Destroy(ragdollRoot.gameObject);
			boneTable.Clear();
			targetSkeletonComponent.UpdateWorld -= UpdateSpineSkeleton;
		}

		public Rigidbody GetRigidbody(string boneName)
		{
			Bone bone = skeleton.FindBone(boneName);
			return (bone == null || !boneTable.ContainsKey(bone)) ? null : boneTable[bone].GetComponent<Rigidbody>();
		}

		private void RecursivelyCreateBoneProxies(Bone b)
		{
			string name = b.data.name;
			if (stopBoneNames.Contains(name))
			{
				return;
			}
			GameObject gameObject = new GameObject(name);
			gameObject.layer = colliderLayer;
			Transform transform = gameObject.transform;
			boneTable.Add(b, transform);
			transform.parent = base.transform;
			transform.localPosition = new Vector3(b.WorldX, b.WorldY, 0f);
			transform.localRotation = Quaternion.Euler(0f, 0f, b.WorldRotationX - b.shearX);
			transform.localScale = new Vector3(b.WorldScaleX, b.WorldScaleY, 1f);
			List<Collider> list = AttachBoundingBoxRagdollColliders(b);
			if (list.Count == 0)
			{
				float length = b.Data.Length;
				if (length == 0f)
				{
					SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>();
					sphereCollider.radius = thickness * 0.5f;
				}
				else
				{
					BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
					boxCollider.size = new Vector3(length, thickness, thickness);
					boxCollider.center = new Vector3(length * 0.5f, 0f);
				}
			}
			Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
			rigidbody.constraints = RigidbodyConstraints.FreezePositionZ;
			foreach (Bone child in b.Children)
			{
				RecursivelyCreateBoneProxies(child);
			}
		}

		private void UpdateSpineSkeleton(ISkeletonAnimation skeletonRenderer)
		{
			bool flipX = skeleton.flipX;
			bool flipY = skeleton.flipY;
			bool flag = flipX ^ flipY;
			bool flag2 = flipX || flipY;
			foreach (KeyValuePair<Bone, Transform> item in boneTable)
			{
				Bone key = item.Key;
				Transform value = item.Value;
				bool flag3 = key == StartingBone;
				Transform transform = (!flag3) ? boneTable[key.Parent] : ragdollRoot;
				Vector3 position = transform.position;
				Quaternion rotation = transform.rotation;
				parentSpaceHelper.position = position;
				parentSpaceHelper.rotation = rotation;
				parentSpaceHelper.localScale = transform.localScale;
				Vector3 position2 = value.position;
				Vector3 vector = parentSpaceHelper.InverseTransformDirection(value.right);
				Vector3 vector2 = parentSpaceHelper.InverseTransformPoint(position2);
				float num = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
				if (flag2)
				{
					if (flag3)
					{
						if (flipX)
						{
							vector2.x *= -1f;
						}
						if (flipY)
						{
							vector2.y *= -1f;
						}
						num *= ((!flag) ? 1f : (-1f));
						if (flipX)
						{
							num += 180f;
						}
					}
					else if (flag)
					{
						num *= -1f;
						vector2.y *= -1f;
					}
				}
				key.x = Mathf.Lerp(key.x, vector2.x, mix);
				key.y = Mathf.Lerp(key.y, vector2.y, mix);
				key.rotation = Mathf.Lerp(key.rotation, num, mix);
			}
		}

		private List<Collider> AttachBoundingBoxRagdollColliders(Bone b)
		{
			List<Collider> list = new List<Collider>();
			Transform transform = boneTable[b];
			GameObject gameObject = transform.gameObject;
			Skin skin = skeleton.Skin ?? skeleton.Data.DefaultSkin;
			List<Attachment> list2 = new List<Attachment>();
			foreach (Slot slot in skeleton.Slots)
			{
				if (slot.Bone == b)
				{
					skin.FindAttachmentsForSlot(skeleton.Slots.IndexOf(slot), list2);
					foreach (Attachment item in list2)
					{
						BoundingBoxAttachment boundingBoxAttachment = item as BoundingBoxAttachment;
						if (boundingBoxAttachment != null && item.Name.ToLower().Contains("ragdoll"))
						{
							BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
							Bounds boundingBoxBounds = SkeletonUtility.GetBoundingBoxBounds(boundingBoxAttachment, thickness);
							boxCollider.center = boundingBoxBounds.center;
							boxCollider.size = boundingBoxBounds.size;
							list.Add(boxCollider);
						}
					}
				}
			}
			return list;
		}

		private static float GetPropagatedRotation(Bone b)
		{
			Bone parent = b.Parent;
			float num = b.AppliedRotation;
			while (parent != null)
			{
				num += parent.AppliedRotation;
				parent = parent.parent;
			}
			return num;
		}
	}
}
