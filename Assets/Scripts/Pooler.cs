using System.Collections.Generic;
using UnityEngine;

public class Pooler : MonoBehaviour
{
	public GameObject objectPrefab;

	private Stack<GameObject> pooledObjects;

	private void Awake()
	{
		pooledObjects = new Stack<GameObject>();
	}

	public GameObject GetPooledObject()
	{
		if (pooledObjects.Count > 0)
		{
			GameObject gameObject = pooledObjects.Pop();
			gameObject.SetActive(value: true);
			return gameObject;
		}
		return UnityEngine.Object.Instantiate(objectPrefab);
	}

	public void Push(GameObject obj)
	{
		obj.SetActive(value: false);
		pooledObjects.Push(obj);
	}
}
