using UnityEngine;

namespace Spine.Unity.MeshGeneration
{
	public struct MeshAndMaterials
	{
		public readonly Mesh mesh;

		public readonly Material[] materials;

		public MeshAndMaterials(Mesh mesh, Material[] materials)
		{
			this.mesh = mesh;
			this.materials = materials;
		}
	}
}
