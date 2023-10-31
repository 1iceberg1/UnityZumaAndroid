using System;

namespace Spine
{
	public class DeformTimeline : CurveTimeline
	{
		internal int slotIndex;

		internal float[] frames;

		internal float[][] frameVertices;

		internal VertexAttachment attachment;

		public int SlotIndex
		{
			get
			{
				return slotIndex;
			}
			set
			{
				slotIndex = value;
			}
		}

		public float[] Frames
		{
			get
			{
				return frames;
			}
			set
			{
				frames = value;
			}
		}

		public float[][] Vertices
		{
			get
			{
				return frameVertices;
			}
			set
			{
				frameVertices = value;
			}
		}

		public VertexAttachment Attachment
		{
			get
			{
				return attachment;
			}
			set
			{
				attachment = value;
			}
		}

		public override int PropertyId => 100663296 + slotIndex;

		public DeformTimeline(int frameCount)
			: base(frameCount)
		{
			frames = new float[frameCount];
			frameVertices = new float[frameCount][];
		}

		public void SetFrame(int frameIndex, float time, float[] vertices)
		{
			frames[frameIndex] = time;
			frameVertices[frameIndex] = vertices;
		}

		public override void Apply(Skeleton skeleton, float lastTime, float time, ExposedList<Event> firedEvents, float alpha, bool setupPose, bool mixingOut)
		{
			Slot slot = skeleton.slots.Items[slotIndex];
			VertexAttachment vertexAttachment = slot.attachment as VertexAttachment;
			if (vertexAttachment == null || !vertexAttachment.ApplyDeform(attachment))
			{
				return;
			}
			ExposedList<float> attachmentVertices = slot.attachmentVertices;
			float[] array = frames;
			if (time < array[0])
			{
				if (setupPose)
				{
					attachmentVertices.Clear();
				}
				return;
			}
			float[][] array2 = frameVertices;
			int num = array2[0].Length;
			if (attachmentVertices.Count != num)
			{
				alpha = 1f;
			}
			if (attachmentVertices.Capacity < num)
			{
				attachmentVertices.Capacity = num;
			}
			attachmentVertices.Count = num;
			float[] items = attachmentVertices.Items;
			if (time >= array[array.Length - 1])
			{
				float[] array3 = array2[array.Length - 1];
				if (alpha == 1f)
				{
					Array.Copy(array3, 0, items, 0, num);
				}
				else if (setupPose)
				{
					VertexAttachment vertexAttachment2 = vertexAttachment;
					if (vertexAttachment2.bones == null)
					{
						float[] vertices = vertexAttachment2.vertices;
						for (int i = 0; i < num; i++)
						{
							float num2 = vertices[i];
							items[i] = num2 + (array3[i] - num2) * alpha;
						}
					}
					else
					{
						for (int j = 0; j < num; j++)
						{
							items[j] = array3[j] * alpha;
						}
					}
				}
				else
				{
					for (int k = 0; k < num; k++)
					{
						items[k] += (array3[k] - items[k]) * alpha;
					}
				}
				return;
			}
			int num3 = Animation.BinarySearch(array, time);
			float[] array4 = array2[num3 - 1];
			float[] array5 = array2[num3];
			float num4 = array[num3];
			float curvePercent = GetCurvePercent(num3 - 1, 1f - (time - num4) / (array[num3 - 1] - num4));
			if (alpha == 1f)
			{
				for (int l = 0; l < num; l++)
				{
					float num5 = array4[l];
					items[l] = num5 + (array5[l] - num5) * curvePercent;
				}
			}
			else if (setupPose)
			{
				VertexAttachment vertexAttachment3 = vertexAttachment;
				if (vertexAttachment3.bones == null)
				{
					float[] vertices2 = vertexAttachment3.vertices;
					for (int m = 0; m < num; m++)
					{
						float num6 = array4[m];
						float num7 = vertices2[m];
						items[m] = num7 + (num6 + (array5[m] - num6) * curvePercent - num7) * alpha;
					}
				}
				else
				{
					for (int n = 0; n < num; n++)
					{
						float num8 = array4[n];
						items[n] = (num8 + (array5[n] - num8) * curvePercent) * alpha;
					}
				}
			}
			else
			{
				for (int num9 = 0; num9 < num; num9++)
				{
					float num10 = array4[num9];
					items[num9] += (num10 + (array5[num9] - num10) * curvePercent - items[num9]) * alpha;
				}
			}
		}
	}
}
