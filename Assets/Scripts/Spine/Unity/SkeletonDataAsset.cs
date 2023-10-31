using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Spine.Unity
{
	public class SkeletonDataAsset : ScriptableObject
	{
		public AtlasAsset[] atlasAssets = new AtlasAsset[0];

		public float scale = 0.01f;

		public TextAsset skeletonJSON;

		public string[] fromAnimation = new string[0];

		public string[] toAnimation = new string[0];

		public float[] duration = new float[0];

		public float defaultMix;

		public RuntimeAnimatorController controller;

		private SkeletonData skeletonData;

		private AnimationStateData stateData;

		public bool IsLoaded => skeletonData != null;

		private void Reset()
		{
			Clear();
		}

		public static SkeletonDataAsset CreateRuntimeInstance(TextAsset skeletonDataFile, AtlasAsset atlasAsset, bool initialize, float scale = 0.01f)
		{
			return CreateRuntimeInstance(skeletonDataFile, new AtlasAsset[1]
			{
				atlasAsset
			}, initialize, scale);
		}

		public static SkeletonDataAsset CreateRuntimeInstance(TextAsset skeletonDataFile, AtlasAsset[] atlasAssets, bool initialize, float scale = 0.01f)
		{
			SkeletonDataAsset skeletonDataAsset = ScriptableObject.CreateInstance<SkeletonDataAsset>();
			skeletonDataAsset.Clear();
			skeletonDataAsset.skeletonJSON = skeletonDataFile;
			skeletonDataAsset.atlasAssets = atlasAssets;
			skeletonDataAsset.scale = scale;
			if (initialize)
			{
				skeletonDataAsset.GetSkeletonData(quiet: true);
			}
			return skeletonDataAsset;
		}

		public void Clear()
		{
			skeletonData = null;
			stateData = null;
		}

		public SkeletonData GetSkeletonData(bool quiet)
		{
			if (atlasAssets == null)
			{
				atlasAssets = new AtlasAsset[0];
				if (!quiet)
				{
					UnityEngine.Debug.LogError("Atlas not set for SkeletonData asset: " + base.name, this);
				}
				Clear();
				return null;
			}
			if (skeletonJSON == null)
			{
				if (!quiet)
				{
					UnityEngine.Debug.LogError("Skeleton JSON file not set for SkeletonData asset: " + base.name, this);
				}
				Clear();
				return null;
			}
			if (atlasAssets.Length == 0)
			{
				Clear();
				return null;
			}
			if (skeletonData != null)
			{
				return skeletonData;
			}
			Atlas[] atlasArray = GetAtlasArray();
			AttachmentLoader attachmentLoader = new AtlasAttachmentLoader(atlasArray);
			float num = scale;
			bool flag = skeletonJSON.name.ToLower().Contains(".skel");
			SkeletonData sd;
			try
			{
				sd = ((!flag) ? ReadSkeletonData(skeletonJSON.text, attachmentLoader, num) : ReadSkeletonData(skeletonJSON.bytes, attachmentLoader, num));
			}
			catch (Exception ex)
			{
				if (!quiet)
				{
					UnityEngine.Debug.LogError("Error reading skeleton JSON file for SkeletonData asset: " + base.name + "\n" + ex.Message + "\n" + ex.StackTrace, this);
				}
				return null;
			}
			InitializeWithData(sd);
			return skeletonData;
		}

		internal void InitializeWithData(SkeletonData sd)
		{
			skeletonData = sd;
			stateData = new AnimationStateData(skeletonData);
			FillStateData();
		}

		internal Atlas[] GetAtlasArray()
		{
			List<Atlas> list = new List<Atlas>(atlasAssets.Length);
			for (int i = 0; i < atlasAssets.Length; i++)
			{
				AtlasAsset atlasAsset = atlasAssets[i];
				if (!(atlasAsset == null))
				{
					Atlas atlas = atlasAsset.GetAtlas();
					if (atlas != null)
					{
						list.Add(atlas);
					}
				}
			}
			return list.ToArray();
		}

		internal static SkeletonData ReadSkeletonData(byte[] bytes, AttachmentLoader attachmentLoader, float scale)
		{
			MemoryStream input = new MemoryStream(bytes);
			SkeletonBinary skeletonBinary = new SkeletonBinary(attachmentLoader);
			skeletonBinary.Scale = scale;
			SkeletonBinary skeletonBinary2 = skeletonBinary;
			return skeletonBinary2.ReadSkeletonData(input);
		}

		internal static SkeletonData ReadSkeletonData(string text, AttachmentLoader attachmentLoader, float scale)
		{
			StringReader reader = new StringReader(text);
			SkeletonJson skeletonJson = new SkeletonJson(attachmentLoader);
			skeletonJson.Scale = scale;
			SkeletonJson skeletonJson2 = skeletonJson;
			return skeletonJson2.ReadSkeletonData(reader);
		}

		public void FillStateData()
		{
			if (stateData == null)
			{
				return;
			}
			stateData.defaultMix = defaultMix;
			int i = 0;
			for (int num = fromAnimation.Length; i < num; i++)
			{
				if (fromAnimation[i].Length != 0 && toAnimation[i].Length != 0)
				{
					stateData.SetMix(fromAnimation[i], toAnimation[i], duration[i]);
				}
			}
		}

		public AnimationStateData GetAnimationStateData()
		{
			if (stateData != null)
			{
				return stateData;
			}
			GetSkeletonData(quiet: false);
			return stateData;
		}
	}
}
