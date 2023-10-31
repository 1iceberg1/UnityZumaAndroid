using UnityEngine;

namespace Spine.Unity
{
	public class DoubleBufferedMesh
	{
		private readonly Mesh mesh1 = SpineMesh.NewMesh();

		private readonly Mesh mesh2 = SpineMesh.NewMesh();

		private bool usingMesh1;

		public Mesh GetNextMesh()
		{
			usingMesh1 = !usingMesh1;
			return (!usingMesh1) ? mesh2 : mesh1;
		}
	}
}
