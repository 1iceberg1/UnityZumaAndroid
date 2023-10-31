using UnityEngine;

namespace Spine.Unity.MeshGeneration
{
	public class SubmeshedMeshInstruction
	{
		public readonly ExposedList<SubmeshInstruction> submeshInstructions = new ExposedList<SubmeshInstruction>();

		public readonly ExposedList<Attachment> attachmentList = new ExposedList<Attachment>();

		public int vertexCount = -1;

		public Material[] GetUpdatedMaterialArray(Material[] materials)
		{
			return submeshInstructions.GetUpdatedMaterialArray(materials);
		}
	}
}
