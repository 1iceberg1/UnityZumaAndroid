using UnityEngine;

public class Item : MonoBehaviour
{
	private Sphere flyingSphere;

	private void Start()
	{
		Invoke("MyDestroy", 10f);
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject.tag == "Sphere")
		{
			Sphere component = collider.gameObject.GetComponent<Sphere>();
			if (component.status == Sphere.Status.Flying)
			{
				flyingSphere = component;
				DoTask();
			}
		}
	}

	protected virtual void DoTask()
	{
		UnityEngine.Object.Destroy(base.gameObject, 0.1f);
		flyingSphere.MyDestroy();
	}

	private void MyDestroy()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
