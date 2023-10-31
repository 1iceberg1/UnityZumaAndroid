using System;
using System.Collections.Generic;
using UnityEngine;

namespace Spine.Unity.Modules
{
	[ExecuteInEditMode]
	public class SkeletonRendererCustomMaterials : MonoBehaviour
	{
		[Serializable]
		public struct SlotMaterialOverride : IEquatable<SlotMaterialOverride>
		{
			public bool overrideDisabled;

			[SpineSlot("", "", false)]
			public string slotName;

			public Material material;

			public bool Equals(SlotMaterialOverride other)
			{
				return overrideDisabled == other.overrideDisabled && slotName == other.slotName && material == other.material;
			}
		}

		[Serializable]
		public struct AtlasMaterialOverride : IEquatable<AtlasMaterialOverride>
		{
			public bool overrideDisabled;

			public Material originalMaterial;

			public Material replacementMaterial;

			public bool Equals(AtlasMaterialOverride other)
			{
				return overrideDisabled == other.overrideDisabled && originalMaterial == other.originalMaterial && replacementMaterial == other.replacementMaterial;
			}
		}

		public SkeletonRenderer skeletonRenderer;

		[SerializeField]
		private List<SlotMaterialOverride> customSlotMaterials = new List<SlotMaterialOverride>();

		[SerializeField]
		private List<AtlasMaterialOverride> customMaterialOverrides = new List<AtlasMaterialOverride>();

		private void SetCustomSlotMaterials()
		{
			if (skeletonRenderer == null)
			{
				UnityEngine.Debug.LogError("skeletonRenderer == null");
				return;
			}
			for (int i = 0; i < customSlotMaterials.Count; i++)
			{
				SlotMaterialOverride slotMaterialOverride = customSlotMaterials[i];
				if (!slotMaterialOverride.overrideDisabled && !string.IsNullOrEmpty(slotMaterialOverride.slotName))
				{
					Slot key = skeletonRenderer.skeleton.FindSlot(slotMaterialOverride.slotName);
					skeletonRenderer.CustomSlotMaterials[key] = slotMaterialOverride.material;
				}
			}
		}

		private void RemoveCustomSlotMaterials()
		{
			if (skeletonRenderer == null)
			{
				UnityEngine.Debug.LogError("skeletonRenderer == null");
				return;
			}
			for (int i = 0; i < customSlotMaterials.Count; i++)
			{
				SlotMaterialOverride slotMaterialOverride = customSlotMaterials[i];
				if (!string.IsNullOrEmpty(slotMaterialOverride.slotName))
				{
					Slot key = skeletonRenderer.skeleton.FindSlot(slotMaterialOverride.slotName);
					if (skeletonRenderer.CustomSlotMaterials.TryGetValue(key, out Material value) && !(value != slotMaterialOverride.material))
					{
						skeletonRenderer.CustomSlotMaterials.Remove(key);
					}
				}
			}
		}

		private void SetCustomMaterialOverrides()
		{
			if (skeletonRenderer == null)
			{
				UnityEngine.Debug.LogError("skeletonRenderer == null");
				return;
			}
			for (int i = 0; i < customMaterialOverrides.Count; i++)
			{
				AtlasMaterialOverride atlasMaterialOverride = customMaterialOverrides[i];
				if (!atlasMaterialOverride.overrideDisabled)
				{
					skeletonRenderer.CustomMaterialOverride[atlasMaterialOverride.originalMaterial] = atlasMaterialOverride.replacementMaterial;
				}
			}
		}

		private void RemoveCustomMaterialOverrides()
		{
			if (skeletonRenderer == null)
			{
				UnityEngine.Debug.LogError("skeletonRenderer == null");
				return;
			}
			for (int i = 0; i < customMaterialOverrides.Count; i++)
			{
				AtlasMaterialOverride atlasMaterialOverride = customMaterialOverrides[i];
				if (skeletonRenderer.CustomMaterialOverride.TryGetValue(atlasMaterialOverride.originalMaterial, out Material value) && !(value != atlasMaterialOverride.replacementMaterial))
				{
					skeletonRenderer.CustomMaterialOverride.Remove(atlasMaterialOverride.originalMaterial);
				}
			}
		}

		private void OnEnable()
		{
			if (skeletonRenderer == null)
			{
				skeletonRenderer = GetComponent<SkeletonRenderer>();
			}
			if (skeletonRenderer == null)
			{
				UnityEngine.Debug.LogError("skeletonRenderer == null");
				return;
			}
			skeletonRenderer.Initialize(overwrite: false);
			SetCustomMaterialOverrides();
			SetCustomSlotMaterials();
		}

		private void OnDisable()
		{
			if (skeletonRenderer == null)
			{
				UnityEngine.Debug.LogError("skeletonRenderer == null");
				return;
			}
			RemoveCustomMaterialOverrides();
			RemoveCustomSlotMaterials();
		}
	}
}
