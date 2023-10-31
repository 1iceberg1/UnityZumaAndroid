using UnityEngine;

namespace Spine.Unity.MeshGeneration
{
	public class ArraysSimpleMeshGenerator : ArraysMeshGenerator, ISimpleMeshGenerator
	{
		protected float scale = 1f;

		protected Mesh lastGeneratedMesh;

		private readonly DoubleBufferedMesh doubleBufferedMesh = new DoubleBufferedMesh();

		private int[] triangles;

		public float Scale
		{
			get
			{
				return scale;
			}
			set
			{
				scale = value;
			}
		}

		public float ZSpacing
		{
			get;
			set;
		}

		public Mesh LastGeneratedMesh => lastGeneratedMesh;

		public Mesh GenerateMesh(Skeleton skeleton)
		{
			int num = 0;
			int num2 = 0;
			Slot[] items = skeleton.drawOrder.Items;
			int count = skeleton.drawOrder.Count;
			for (int i = 0; i < count; i++)
			{
				Slot slot = items[i];
				Attachment attachment = slot.attachment;
				RegionAttachment regionAttachment = attachment as RegionAttachment;
				int num3;
				int num4;
				if (regionAttachment != null)
				{
					num3 = 4;
					num4 = 6;
				}
				else
				{
					MeshAttachment meshAttachment = attachment as MeshAttachment;
					if (meshAttachment == null)
					{
						continue;
					}
					num3 = meshAttachment.worldVerticesLength >> 1;
					num4 = meshAttachment.triangles.Length;
				}
				num2 += num4;
				num += num3;
			}
			ArraysMeshGenerator.EnsureSize(num, ref base.meshVertices, ref meshUVs, ref meshColors32);
			triangles = (triangles ?? new int[num2]);
			Vector3 boundsMin = default(Vector3);
			Vector3 boundsMax = default(Vector3);
			if (num == 0)
			{
				boundsMin = new Vector3(0f, 0f, 0f);
				boundsMax = new Vector3(0f, 0f, 0f);
			}
			else
			{
				boundsMin.x = 2.14748365E+09f;
				boundsMin.y = 2.14748365E+09f;
				boundsMax.x = -2.14748365E+09f;
				boundsMax.y = -2.14748365E+09f;
				boundsMin.z = -0.01f * scale;
				boundsMax.z = 0.01f * scale;
				int vertexIndex = 0;
				ArraysMeshGenerator.FillVerts(skeleton, 0, count, ZSpacing, base.PremultiplyVertexColors, base.meshVertices, meshUVs, meshColors32, ref vertexIndex, ref attachmentVertexBuffer, ref boundsMin, ref boundsMax);
				boundsMax.x *= scale;
				boundsMax.y *= scale;
				boundsMin.x *= scale;
				boundsMax.y *= scale;
				Vector3[] meshVertices = base.meshVertices;
				for (int j = 0; j < num; j++)
				{
					Vector3 vector = meshVertices[j];
					vector.x *= scale;
					vector.y *= scale;
					meshVertices[j] = vector;
				}
			}
			ArraysMeshGenerator.FillTriangles(ref triangles, skeleton, num2, 0, 0, count, isLastSubmesh: true);
			Mesh nextMesh = doubleBufferedMesh.GetNextMesh();
			nextMesh.vertices = base.meshVertices;
			nextMesh.colors32 = meshColors32;
			nextMesh.uv = meshUVs;
			nextMesh.bounds = ArraysMeshGenerator.ToBounds(boundsMin, boundsMax);
			nextMesh.triangles = triangles;
			TryAddNormalsTo(nextMesh, num);
			if (addTangents)
			{
				ArraysMeshGenerator.SolveTangents2DEnsureSize(ref meshTangents, ref tempTanBuffer, num);
				ArraysMeshGenerator.SolveTangents2DTriangles(tempTanBuffer, triangles, num2, base.meshVertices, meshUVs, num);
				ArraysMeshGenerator.SolveTangents2DBuffer(meshTangents, tempTanBuffer, num);
			}
			lastGeneratedMesh = nextMesh;
			return nextMesh;
		}
	}
}
