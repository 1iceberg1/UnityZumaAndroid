namespace Spine.Unity.MeshGeneration
{
	public interface ISubmeshSetMeshGenerator
	{
		float ZSpacing
		{
			get;
			set;
		}

		bool PremultiplyVertexColors
		{
			get;
			set;
		}

		bool AddNormals
		{
			get;
			set;
		}

		bool AddTangents
		{
			get;
			set;
		}

		MeshAndMaterials GenerateMesh(ExposedList<SubmeshInstruction> instructions, int startSubmesh, int endSubmesh);
	}
}
