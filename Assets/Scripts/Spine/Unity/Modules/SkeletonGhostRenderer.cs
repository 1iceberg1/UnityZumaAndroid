using System.Collections;
using UnityEngine;

namespace Spine.Unity.Modules
{
	public class SkeletonGhostRenderer : MonoBehaviour
	{
		public float fadeSpeed = 10f;

		private Color32[] colors;

		private Color32 black = new Color32(0, 0, 0, 0);

		private MeshFilter meshFilter;

		private MeshRenderer meshRenderer;

		private void Awake()
		{
			meshRenderer = base.gameObject.AddComponent<MeshRenderer>();
			meshFilter = base.gameObject.AddComponent<MeshFilter>();
		}

		public void Initialize(Mesh mesh, Material[] materials, Color32 color, bool additive, float speed, int sortingLayerID, int sortingOrder)
		{
			StopAllCoroutines();
			base.gameObject.SetActive(value: true);
			meshRenderer.sharedMaterials = materials;
			meshRenderer.sortingLayerID = sortingLayerID;
			meshRenderer.sortingOrder = sortingOrder;
			meshFilter.sharedMesh = UnityEngine.Object.Instantiate(mesh);
			colors = meshFilter.sharedMesh.colors32;
			if (color.a + color.r + color.g + color.b > 0)
			{
				for (int i = 0; i < colors.Length; i++)
				{
					colors[i] = color;
				}
			}
			fadeSpeed = speed;
			if (additive)
			{
				StartCoroutine(FadeAdditive());
			}
			else
			{
				StartCoroutine(Fade());
			}
		}

		private IEnumerator Fade()
		{
			for (int t = 0; t < 500; t++)
			{
				bool breakout = true;
				for (int i = 0; i < colors.Length; i++)
				{
					Color32 c = colors[i];
					if (c.a > 0)
					{
						breakout = false;
					}
					colors[i] = Color32.Lerp(c, black, Time.deltaTime * fadeSpeed);
				}
				meshFilter.sharedMesh.colors32 = colors;
				if (breakout)
				{
					break;
				}
				yield return null;
			}
			UnityEngine.Object.Destroy(meshFilter.sharedMesh);
			base.gameObject.SetActive(value: false);
		}

		private IEnumerator FadeAdditive()
		{
			Color32 black = this.black;
			for (int t = 0; t < 500; t++)
			{
				bool breakout = true;
				for (int i = 0; i < colors.Length; i++)
				{
					Color32 c = colors[i];
					black.a = c.a;
					if (c.r > 0 || c.g > 0 || c.b > 0)
					{
						breakout = false;
					}
					colors[i] = Color32.Lerp(c, black, Time.deltaTime * fadeSpeed);
				}
				meshFilter.sharedMesh.colors32 = colors;
				if (breakout)
				{
					break;
				}
				yield return null;
			}
			UnityEngine.Object.Destroy(meshFilter.sharedMesh);
			base.gameObject.SetActive(value: false);
		}

		public void Cleanup()
		{
			if (meshFilter != null && meshFilter.sharedMesh != null)
			{
				UnityEngine.Object.Destroy(meshFilter.sharedMesh);
			}
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
