using UnityEngine;

public class MarginObject : MonoBehaviour
{
	public bool isMarginLeft;

	public bool isMarginRight;

	public bool isMarginTop;

	public bool isMarginBottom;

	public float marginLeft;

	public float marginRight;

	public float marginTop;

	public float marginBottom;

	private void Start()
	{
		float width = UICamera.instance.GetWidth();
		float height = UICamera.instance.GetHeight();
		if (isMarginLeft)
		{
			Transform transform = base.transform;
			float x = 0f - width + marginLeft;
			Vector3 position = base.transform.position;
			float y = position.y;
			Vector3 position2 = base.transform.position;
			transform.position = new Vector3(x, y, position2.z);
		}
		if (isMarginRight)
		{
			Transform transform2 = base.transform;
			float x2 = width - marginRight;
			Vector3 position3 = base.transform.position;
			float y2 = position3.y;
			Vector3 position4 = base.transform.position;
			transform2.position = new Vector3(x2, y2, position4.z);
		}
		if (isMarginTop)
		{
			Transform transform3 = base.transform;
			Vector3 position5 = base.transform.position;
			float x3 = position5.x;
			float y3 = height - marginTop;
			Vector3 position6 = base.transform.position;
			transform3.position = new Vector3(x3, y3, position6.z);
		}
		if (isMarginBottom)
		{
			Transform transform4 = base.transform;
			Vector3 position7 = base.transform.position;
			float x4 = position7.x;
			float y4 = 0f - height + marginBottom;
			Vector3 position8 = base.transform.position;
			transform4.position = new Vector3(x4, y4, position8.z);
		}
	}
}
