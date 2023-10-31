using Spine.Unity.MeshGeneration;
using UnityEngine;

namespace Spine.Unity.Modules
{
	[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
	public class SkeletonPartsRenderer : MonoBehaviour
	{
		private ISubmeshSetMeshGenerator meshGenerator;

		private MeshRenderer meshRenderer;

		private MeshFilter meshFilter;

		public ISubmeshSetMeshGenerator MeshGenerator
		{
			get
			{
				LazyIntialize();
				return meshGenerator;
			}
		}

		public MeshRenderer MeshRenderer
		{
			get
			{
				LazyIntialize();
				return meshRenderer;
			}
		}

		public MeshFilter MeshFilter
		{
			get
			{
				LazyIntialize();
				return meshFilter;
			}
		}

		private void LazyIntialize()
		{
			if (meshGenerator == null)
			{
				meshGenerator = new ArraysSubmeshSetMeshGenerator();
				meshFilter = GetComponent<MeshFilter>();
				meshRenderer = GetComponent<MeshRenderer>();
			}
		}

		public void ClearMesh()
		{
			LazyIntialize();
			meshFilter.sharedMesh = null;
		}

		public void RenderParts(ExposedList<SubmeshInstruction> instructions, int startSubmesh, int endSubmesh)
		{
			LazyIntialize();
			MeshAndMaterials meshAndMaterials = meshGenerator.GenerateMesh(instructions, startSubmesh, endSubmesh);
			meshFilter.sharedMesh = meshAndMaterials.mesh;
			meshRenderer.sharedMaterials = meshAndMaterials.materials;
		}

		public void SetPropertyBlock(MaterialPropertyBlock block)
		{
			LazyIntialize();
			meshRenderer.SetPropertyBlock(block);
		}

		public static SkeletonPartsRenderer NewPartsRendererGameObject(Transform parent, string name)
		{
			GameObject gameObject = new GameObject(name, typeof(MeshFilter), typeof(MeshRenderer));
			gameObject.transform.SetParent(parent, worldPositionStays: false);
			return gameObject.AddComponent<SkeletonPartsRenderer>();
		}
	}
}
