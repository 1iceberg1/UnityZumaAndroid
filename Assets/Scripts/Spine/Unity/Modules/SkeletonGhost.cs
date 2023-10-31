using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Spine.Unity.Modules
{
	[RequireComponent(typeof(SkeletonRenderer))]
	public class SkeletonGhost : MonoBehaviour
	{
		private const HideFlags GhostHideFlags = HideFlags.HideInHierarchy;

		private const string GhostingShaderName = "Spine/Special/SkeletonGhost";

		public bool ghostingEnabled = true;

		public float spawnRate = 0.05f;

		public Color32 color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 0);

		[Tooltip("Remember to set color alpha to 0 if Additive is true")]
		public bool additive = true;

		public int maximumGhosts = 10;

		public float fadeSpeed = 10f;

		public Shader ghostShader;

		[Tooltip("0 is Color and Alpha, 1 is Alpha only.")]
		[Range(0f, 1f)]
		public float textureFade = 1f;

		[Header("Sorting")]
		public bool sortWithDistanceOnly;

		public float zOffset;

		private float nextSpawnTime;

		private SkeletonGhostRenderer[] pool;

		private int poolIndex;

		private SkeletonRenderer skeletonRenderer;

		private MeshRenderer meshRenderer;

		private MeshFilter meshFilter;

		private readonly Dictionary<Material, Material> materialTable = new Dictionary<Material, Material>();

		private void Start()
		{
			if (ghostShader == null)
			{
				ghostShader = Shader.Find("Spine/Special/SkeletonGhost");
			}
			skeletonRenderer = GetComponent<SkeletonRenderer>();
			meshFilter = GetComponent<MeshFilter>();
			meshRenderer = GetComponent<MeshRenderer>();
			nextSpawnTime = Time.time + spawnRate;
			pool = new SkeletonGhostRenderer[maximumGhosts];
			for (int i = 0; i < maximumGhosts; i++)
			{
				GameObject gameObject = new GameObject(base.gameObject.name + " Ghost", typeof(SkeletonGhostRenderer));
				pool[i] = gameObject.GetComponent<SkeletonGhostRenderer>();
				gameObject.SetActive(value: false);
				gameObject.hideFlags = HideFlags.HideInHierarchy;
			}
			IAnimationStateComponent animationStateComponent = skeletonRenderer as IAnimationStateComponent;
			if (animationStateComponent != null)
			{
				animationStateComponent.AnimationState.Event += OnEvent;
			}
		}

		private void OnEvent(TrackEntry trackEntry, Event e)
		{
			if (e.Data.Name.Equals("Ghosting", StringComparison.Ordinal))
			{
				ghostingEnabled = (e.Int > 0);
				if (e.Float > 0f)
				{
					spawnRate = e.Float;
				}
				if (!string.IsNullOrEmpty(e.stringValue))
				{
					color = HexToColor(e.String);
				}
			}
		}

		private void Ghosting(float val)
		{
			ghostingEnabled = (val > 0f);
		}

		private void Update()
		{
			if (!ghostingEnabled || !(Time.time >= nextSpawnTime))
			{
				return;
			}
			GameObject gameObject = pool[poolIndex].gameObject;
			Material[] sharedMaterials = meshRenderer.sharedMaterials;
			for (int i = 0; i < sharedMaterials.Length; i++)
			{
				Material material = sharedMaterials[i];
				Material material2;
				if (!materialTable.ContainsKey(material))
				{
					material2 = new Material(material);
					material2.shader = ghostShader;
					material2.color = Color.white;
					if (material2.HasProperty("_TextureFade"))
					{
						material2.SetFloat("_TextureFade", textureFade);
					}
					materialTable.Add(material, material2);
				}
				else
				{
					material2 = materialTable[material];
				}
				sharedMaterials[i] = material2;
			}
			Transform transform = gameObject.transform;
			transform.parent = base.transform;
			pool[poolIndex].Initialize(meshFilter.sharedMesh, sharedMaterials, color, additive, fadeSpeed, meshRenderer.sortingLayerID, (!sortWithDistanceOnly) ? (meshRenderer.sortingOrder - 1) : meshRenderer.sortingOrder);
			transform.localPosition = new Vector3(0f, 0f, zOffset);
			transform.localRotation = Quaternion.identity;
			transform.localScale = Vector3.one;
			transform.parent = null;
			poolIndex++;
			if (poolIndex == pool.Length)
			{
				poolIndex = 0;
			}
			nextSpawnTime = Time.time + spawnRate;
		}

		private void OnDestroy()
		{
			for (int i = 0; i < maximumGhosts; i++)
			{
				if (pool[i] != null)
				{
					pool[i].Cleanup();
				}
			}
			foreach (Material value in materialTable.Values)
			{
				UnityEngine.Object.Destroy(value);
			}
		}

		private static Color32 HexToColor(string hex)
		{
			if (hex.Length < 6)
			{
				return Color.magenta;
			}
			hex = hex.Replace("#", string.Empty);
			byte r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
			byte g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
			byte b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
			byte a = byte.MaxValue;
			if (hex.Length == 8)
			{
				a = byte.Parse(hex.Substring(6, 2), NumberStyles.HexNumber);
			}
			return new Color32(r, g, b, a);
		}
	}
}
