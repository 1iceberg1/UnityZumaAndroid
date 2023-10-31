using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoUtils : MonoBehaviour
{
	public Sphere[] spheres;

	public Material[] sphereMaterials;

	public Sphere[] sphereColors;

	public static MonoUtils instance;

	public Sphere pusher;

	public Sphere multiColor;

	public Sphere bomb;

	public Sphere hidden;

	public Sphere black;

	public GameObject sphereExplosion;

	public GameObject bombExplosion;

	public GameObject hiddenExplosion;

	public GameObject colorExplosion;

	public GameObject rainBomb;

	public Trace[] trace;

	public Transform coinMoveTarget;

	public GameObject smokeHolePrefab;

	public GameObject ballShadow;

	public GameObject iceCover;

	public Sprite[] iceCoverSprites;

	public GameObject[] breakIceFx;

	public Transform scoreMoveTarget;

	public GameObject bonusText;

	public Item[] items;

	[HideInInspector]
	public int numDroppingBomb;

	private void Awake()
	{
		instance = this;
	}

	public IEnumerator RainBomb()
	{
		numDroppingBomb++;
		float lastBombTime = 0f;
		int i = 0;
		while (GetTotalBalls() > 0)
		{
			if (GameState.IsNormal() && Time.time - lastBombTime >= 0.6f)
			{
				List<Vector3> positions = GetPositions();
				Vector3 vector;
				if (UnityEngine.Random.Range(0, 10) <= 5 && positions.Count > 5)
				{
					int random = CUtils.GetRandom<int>(4, 5);
					vector = positions[random];
				}
				else if (positions.Count == 2)
				{
					vector = positions[0];
				}
				else
				{
					positions.RemoveAt(positions.Count - 1);
					vector = CUtils.GetRandom(positions.ToArray());
				}
				GameObject gameObject = UnityEngine.Object.Instantiate(rainBomb);
				gameObject.transform.position = vector + new Vector3(-3f, 8f, -4f);
				Sound.instance.Play(Sound.Others.FireBall);
				iTween.MoveTo(gameObject, vector, 0.7f);
				lastBombTime = Time.time;
				i++;
				if (i == 6)
				{
					break;
				}
			}
			yield return new WaitForSeconds(0.1f);
		}
		yield return new WaitForSeconds(0.6f);
		numDroppingBomb--;
	}

	public int GetTotalBalls()
	{
		int num = 0;
		SphereController[] sphereControllers = MainController.instance.sphereControllers;
		foreach (SphereController sphereController in sphereControllers)
		{
			num += sphereController.spheres.Count;
		}
		return num;
	}

	private List<Vector3> GetPositions()
	{
		List<Vector3> list = new List<Vector3>();
		SphereController[] sphereControllers = MainController.instance.sphereControllers;
		int num = (sphereControllers.Length != 1) ? Mathf.Max(sphereControllers[0].spheres.Count, sphereControllers[1].spheres.Count) : sphereControllers[0].spheres.Count;
		for (int i = 0; i < num; i++)
		{
			SphereController[] array = sphereControllers;
			foreach (SphereController sphereController in array)
			{
				if (i < sphereController.spheres.Count)
				{
					list.Add(sphereController.spheres[i].transform.position);
				}
			}
		}
		return list;
	}
}
