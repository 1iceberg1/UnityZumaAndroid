using Spine.Unity.MeshGeneration;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Spine.Unity.Modules
{
	[ExecuteInEditMode]
	[HelpURL("https://github.com/pharan/spine-unity-docs/blob/master/SkeletonRenderSeparator.md")]
	public class SkeletonRenderSeparator : MonoBehaviour
	{
		public const int DefaultSortingOrderIncrement = 5;

		[SerializeField]
		protected SkeletonRenderer skeletonRenderer;

		private MeshRenderer mainMeshRenderer;

		public bool copyPropertyBlock;

		[Tooltip("Copies MeshRenderer flags into each parts renderer")]
		public bool copyMeshRendererFlags;

		public List<SkeletonPartsRenderer> partsRenderers = new List<SkeletonPartsRenderer>();

		private MaterialPropertyBlock copiedBlock;

		public SkeletonRenderer SkeletonRenderer
		{
			get
			{
				return skeletonRenderer;
			}
			set
			{
				if (skeletonRenderer != null)
				{
					skeletonRenderer.GenerateMeshOverride -= HandleRender;
				}
				skeletonRenderer = value;
				base.enabled = false;
			}
		}

		private void OnEnable()
		{
			if (skeletonRenderer == null)
			{
				return;
			}
			if (copiedBlock == null)
			{
				copiedBlock = new MaterialPropertyBlock();
			}
			mainMeshRenderer = skeletonRenderer.GetComponent<MeshRenderer>();
			skeletonRenderer.GenerateMeshOverride -= HandleRender;
			skeletonRenderer.GenerateMeshOverride += HandleRender;
			if (!copyMeshRendererFlags)
			{
				return;
			}
			LightProbeUsage lightProbeUsage = mainMeshRenderer.lightProbeUsage;
			bool receiveShadows = mainMeshRenderer.receiveShadows;
			for (int i = 0; i < partsRenderers.Count; i++)
			{
				SkeletonPartsRenderer skeletonPartsRenderer = partsRenderers[i];
				if (!(skeletonPartsRenderer == null))
				{
					MeshRenderer meshRenderer = skeletonPartsRenderer.MeshRenderer;
					meshRenderer.lightProbeUsage = lightProbeUsage;
					meshRenderer.receiveShadows = receiveShadows;
				}
			}
		}

		private void OnDisable()
		{
			if (!(skeletonRenderer == null))
			{
				skeletonRenderer.GenerateMeshOverride -= HandleRender;
				foreach (SkeletonPartsRenderer partsRenderer in partsRenderers)
				{
					partsRenderer.ClearMesh();
				}
			}
		}

		private void HandleRender(SkeletonRenderer.SmartMesh.Instruction instruction)
		{
			int count = partsRenderers.Count;
			if (count <= 0)
			{
				return;
			}
			if (copyPropertyBlock)
			{
				mainMeshRenderer.GetPropertyBlock(copiedBlock);
			}
			ExposedList<SubmeshInstruction> submeshInstructions = instruction.submeshInstructions;
			SubmeshInstruction[] items = submeshInstructions.Items;
			int num = submeshInstructions.Count - 1;
			bool calculateNormals = skeletonRenderer.calculateNormals;
			bool calculateTangents = skeletonRenderer.calculateTangents;
			bool pmaVertexColors = skeletonRenderer.pmaVertexColors;
			int i = 0;
			SkeletonPartsRenderer skeletonPartsRenderer = partsRenderers[i];
			int j = 0;
			int startSubmesh = 0;
			for (; j <= num; j++)
			{
				if (items[j].forceSeparate || j == num)
				{
					ISubmeshSetMeshGenerator meshGenerator = skeletonPartsRenderer.MeshGenerator;
					meshGenerator.AddNormals = calculateNormals;
					meshGenerator.AddTangents = calculateTangents;
					meshGenerator.PremultiplyVertexColors = pmaVertexColors;
					if (copyPropertyBlock)
					{
						skeletonPartsRenderer.SetPropertyBlock(copiedBlock);
					}
					skeletonPartsRenderer.RenderParts(instruction.submeshInstructions, startSubmesh, j + 1);
					startSubmesh = j + 1;
					i++;
					if (i >= count)
					{
						break;
					}
					skeletonPartsRenderer = partsRenderers[i];
				}
			}
			for (; i < count; i++)
			{
				partsRenderers[i].ClearMesh();
			}
		}
	}
}
