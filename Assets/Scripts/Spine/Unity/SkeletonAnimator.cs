using System;
using System.Collections.Generic;
using UnityEngine;

namespace Spine.Unity
{
	[RequireComponent(typeof(Animator))]
	public class SkeletonAnimator : SkeletonRenderer, ISkeletonAnimation
	{
		public enum MixMode
		{
			AlwaysMix,
			MixNext,
			SpineStyle
		}

		public MixMode[] layerMixModes = new MixMode[0];

		public bool autoReset;

		private List<Animation> previousAnimations = new List<Animation>();

		private readonly Dictionary<int, Animation> animationTable = new Dictionary<int, Animation>();

		private readonly Dictionary<AnimationClip, int> clipNameHashCodeTable = new Dictionary<AnimationClip, int>();

		private Animator animator;

		protected event UpdateBonesDelegate _UpdateLocal;

		protected event UpdateBonesDelegate _UpdateWorld;

		protected event UpdateBonesDelegate _UpdateComplete;

		public event UpdateBonesDelegate UpdateLocal;

		public event UpdateBonesDelegate UpdateWorld;

		public event UpdateBonesDelegate UpdateComplete;

		public override void Initialize(bool overwrite)
		{
			if (!valid || overwrite)
			{
				base.Initialize(overwrite);
				if (valid)
				{
					animationTable.Clear();
					clipNameHashCodeTable.Clear();
					animator = GetComponent<Animator>();
					SkeletonData skeletonData = skeletonDataAsset.GetSkeletonData(quiet: true);
					foreach (Animation animation in skeletonData.Animations)
					{
						animationTable.Add(animation.Name.GetHashCode(), animation);
					}
				}
			}
		}

		private void Update()
		{
			if (!valid)
			{
				return;
			}
			if (layerMixModes.Length < animator.layerCount)
			{
				Array.Resize(ref layerMixModes, animator.layerCount);
			}
			if (autoReset)
			{
				List<Animation> list = previousAnimations;
				int i = 0;
				for (int count = list.Count; i < count; i++)
				{
					list[i].SetKeyedItemsToSetupPose(skeleton);
				}
				list.Clear();
				int j = 0;
				for (int layerCount = animator.layerCount; j < layerCount; j++)
				{
					float num = (j != 0) ? animator.GetLayerWeight(j) : 1f;
					if (num <= 0f)
					{
						continue;
					}
					bool flag = animator.GetNextAnimatorStateInfo(j).fullPathHash != 0;
					AnimatorClipInfo[] currentAnimatorClipInfo = animator.GetCurrentAnimatorClipInfo(j);
					AnimatorClipInfo[] nextAnimatorClipInfo = animator.GetNextAnimatorClipInfo(j);
					for (int k = 0; k < currentAnimatorClipInfo.Length; k++)
					{
						AnimatorClipInfo animatorClipInfo = currentAnimatorClipInfo[k];
						float num2 = animatorClipInfo.weight * num;
						if (num2 != 0f)
						{
							list.Add(animationTable[NameHashCode(animatorClipInfo.clip)]);
						}
					}
					if (!flag)
					{
						continue;
					}
					for (int l = 0; l < nextAnimatorClipInfo.Length; l++)
					{
						AnimatorClipInfo animatorClipInfo2 = nextAnimatorClipInfo[l];
						float num3 = animatorClipInfo2.weight * num;
						if (num3 != 0f)
						{
							list.Add(animationTable[NameHashCode(animatorClipInfo2.clip)]);
						}
					}
				}
			}
			int m = 0;
			for (int layerCount2 = animator.layerCount; m < layerCount2; m++)
			{
				float num4 = (m != 0) ? animator.GetLayerWeight(m) : 1f;
				AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(m);
				AnimatorStateInfo nextAnimatorStateInfo = animator.GetNextAnimatorStateInfo(m);
				bool flag2 = nextAnimatorStateInfo.fullPathHash != 0;
				AnimatorClipInfo[] currentAnimatorClipInfo2 = animator.GetCurrentAnimatorClipInfo(m);
				AnimatorClipInfo[] nextAnimatorClipInfo2 = animator.GetNextAnimatorClipInfo(m);
				MixMode mixMode = layerMixModes[m];
				if (mixMode == MixMode.AlwaysMix)
				{
					for (int n = 0; n < currentAnimatorClipInfo2.Length; n++)
					{
						AnimatorClipInfo animatorClipInfo3 = currentAnimatorClipInfo2[n];
						float num5 = animatorClipInfo3.weight * num4;
						if (num5 != 0f)
						{
							animationTable[NameHashCode(animatorClipInfo3.clip)].Apply(skeleton, 0f, AnimationTime(currentAnimatorStateInfo.normalizedTime, animatorClipInfo3.clip.length, currentAnimatorStateInfo.loop), currentAnimatorStateInfo.loop, null, num5, setupPose: false, mixingOut: false);
						}
					}
					if (!flag2)
					{
						continue;
					}
					for (int num6 = 0; num6 < nextAnimatorClipInfo2.Length; num6++)
					{
						AnimatorClipInfo animatorClipInfo4 = nextAnimatorClipInfo2[num6];
						float num7 = animatorClipInfo4.weight * num4;
						if (num7 != 0f)
						{
							animationTable[NameHashCode(animatorClipInfo4.clip)].Apply(skeleton, 0f, nextAnimatorStateInfo.normalizedTime * animatorClipInfo4.clip.length, nextAnimatorStateInfo.loop, null, num7, setupPose: false, mixingOut: false);
						}
					}
					continue;
				}
				int num8;
				for (num8 = 0; num8 < currentAnimatorClipInfo2.Length; num8++)
				{
					AnimatorClipInfo animatorClipInfo5 = currentAnimatorClipInfo2[num8];
					float num9 = animatorClipInfo5.weight * num4;
					if (num9 != 0f)
					{
						animationTable[NameHashCode(animatorClipInfo5.clip)].Apply(skeleton, 0f, AnimationTime(currentAnimatorStateInfo.normalizedTime, animatorClipInfo5.clip.length, currentAnimatorStateInfo.loop), currentAnimatorStateInfo.loop, null, 1f, setupPose: false, mixingOut: false);
						break;
					}
				}
				for (; num8 < currentAnimatorClipInfo2.Length; num8++)
				{
					AnimatorClipInfo animatorClipInfo6 = currentAnimatorClipInfo2[num8];
					float num10 = animatorClipInfo6.weight * num4;
					if (num10 != 0f)
					{
						animationTable[NameHashCode(animatorClipInfo6.clip)].Apply(skeleton, 0f, AnimationTime(currentAnimatorStateInfo.normalizedTime, animatorClipInfo6.clip.length, currentAnimatorStateInfo.loop), currentAnimatorStateInfo.loop, null, num10, setupPose: false, mixingOut: false);
					}
				}
				num8 = 0;
				if (!flag2)
				{
					continue;
				}
				if (mixMode == MixMode.SpineStyle)
				{
					for (; num8 < nextAnimatorClipInfo2.Length; num8++)
					{
						AnimatorClipInfo animatorClipInfo7 = nextAnimatorClipInfo2[num8];
						float num11 = animatorClipInfo7.weight * num4;
						if (num11 != 0f)
						{
							animationTable[NameHashCode(animatorClipInfo7.clip)].Apply(skeleton, 0f, nextAnimatorStateInfo.normalizedTime * animatorClipInfo7.clip.length, nextAnimatorStateInfo.loop, null, 1f, setupPose: false, mixingOut: false);
							break;
						}
					}
				}
				for (; num8 < nextAnimatorClipInfo2.Length; num8++)
				{
					AnimatorClipInfo animatorClipInfo8 = nextAnimatorClipInfo2[num8];
					float num12 = animatorClipInfo8.weight * num4;
					if (num12 != 0f)
					{
						animationTable[NameHashCode(animatorClipInfo8.clip)].Apply(skeleton, 0f, nextAnimatorStateInfo.normalizedTime * animatorClipInfo8.clip.length, nextAnimatorStateInfo.loop, null, num12, setupPose: false, mixingOut: false);
					}
				}
			}
			if (this._UpdateLocal != null)
			{
				this._UpdateLocal(this);
			}
			skeleton.UpdateWorldTransform();
			if (this._UpdateWorld != null)
			{
				this._UpdateWorld(this);
				skeleton.UpdateWorldTransform();
			}
			if (this._UpdateComplete != null)
			{
				this._UpdateComplete(this);
			}
		}

		private static float AnimationTime(float normalizedTime, float clipLength, bool loop)
		{
			float num = normalizedTime * clipLength;
			if (loop)
			{
				return num;
			}
			return (!(clipLength - num < 71f / (678f * (float)Math.PI))) ? num : clipLength;
		}

		private int NameHashCode(AnimationClip clip)
		{
			if (!clipNameHashCodeTable.TryGetValue(clip, out int value))
			{
				value = clip.name.GetHashCode();
				clipNameHashCodeTable.Add(clip, value);
			}
			return value;
		}
	}
}
