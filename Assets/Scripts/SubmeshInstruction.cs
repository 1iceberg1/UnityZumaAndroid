using UnityEngine;

namespace Spine.Unity.MeshGeneration
{
	public struct SubmeshInstruction
	{
		public Skeleton skeleton;

		public int startSlot;

		public int endSlot;

		public Material material;

		public int triangleCount;

		public int vertexCount;

		public int firstVertexIndex;

		public bool forceSeparate;

		public int SlotCount => endSlot - startSlot;
	}
}
