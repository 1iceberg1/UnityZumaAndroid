using UnityEngine;

namespace Spine.Unity.MeshGeneration
{
	public interface ISimpleMeshGenerator
	{
		Mesh LastGeneratedMesh
		{
			get;
		}

		float Scale
		{
			set;
		}

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

		Mesh GenerateMesh(Skeleton skeleton);
	}
}
