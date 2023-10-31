using System.Collections.Generic;

namespace Spine.Unity.MeshGeneration
{
	public interface ISubmeshedMeshGenerator
	{
		List<Slot> Separators
		{
			get;
		}

		float ZSpacing
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

		SubmeshedMeshInstruction GenerateInstruction(Skeleton skeleton);

		MeshAndMaterials GenerateMesh(SubmeshedMeshInstruction wholeMeshInstruction);
	}
}
