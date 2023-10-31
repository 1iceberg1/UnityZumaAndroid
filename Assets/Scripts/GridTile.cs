using UnityEngine;
using UnityEngine.UI;

public class GridTile : MonoBehaviour
{
	public GameObject tile;

	public int numTop;

	public int numBottom;

	public int numRight;

	public int numLeft;

	public Vector3 centerPosition;

	public Transform parentTransform;

	private void Start()
	{
		Sprite sprite = tile.GetComponent<Image>().sprite;
		float width = sprite.rect.width;
		float height = sprite.rect.height;
		for (int i = 0; i < numRight; i++)
		{
			for (int j = 0; j < numTop; j++)
			{
				float num = (float)i * width + width / 2f;
				float num2 = (float)j * height + height / 2f;
				GameObject gameObject = UnityEngine.Object.Instantiate(tile);
				GameObject gameObject2 = UnityEngine.Object.Instantiate(tile);
				GameObject gameObject3 = UnityEngine.Object.Instantiate(tile);
				GameObject gameObject4 = UnityEngine.Object.Instantiate(tile);
				gameObject.transform.SetParent(parentTransform);
				gameObject2.transform.SetParent(parentTransform);
				gameObject3.transform.SetParent(parentTransform);
				gameObject4.transform.SetParent(parentTransform);
				gameObject.transform.localScale = Vector3.one;
				gameObject2.transform.localScale = Vector3.one;
				gameObject3.transform.localScale = Vector3.one;
				gameObject4.transform.localScale = Vector3.one;
				gameObject.transform.localPosition = new Vector3(num, num2, 0f);
				gameObject2.transform.localPosition = new Vector3(0f - num, num2, 0f);
				gameObject3.transform.localPosition = new Vector3(num, 0f - num2, 0f);
				gameObject4.transform.localPosition = new Vector3(0f - num, 0f - num2, 0f);
			}
		}
	}
}
