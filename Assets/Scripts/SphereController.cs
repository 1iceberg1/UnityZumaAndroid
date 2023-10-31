using Superpow;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityStandardAssets.Utility;

public class SphereController : MonoBehaviour
{
	public List<Sphere> spheres = new List<Sphere>();

	public int index;

	public float endSlowDownTime;

	public Sphere sphereTester;

	private static int lastSphere;

	private static int sameSpheres;

	private GameObject smokeHole;

	[HideInInspector]
	public static List<SphereType> currentTypes;

	public List<SphereType> cTypes = new List<SphereType>();

	[HideInInspector]
	public List<Sphere> listPushers = new List<Sphere>();

	[HideInInspector]
	public List<Sphere> listMultiColors = new List<Sphere>();

	[HideInInspector]
	public float defaultSpeed;

	[HideInInspector]
	public static Attacks attacks;

	[HideInInspector]
	public bool completeSpawning;

	[HideInInspector]
	public List<Sphere> lateUpdateSpheres = new List<Sphere>();

	public static float endPausedTime;

	public static float endBackwardingTime;

	public static float beginDelayTime;

	public static bool isPausing;

	public List<Vector3> waypoints => Paths.instance.waypoints[index];

	private void Awake()
	{
		currentTypes = new List<SphereType>();
	}

	private void Start()
	{
		SetUp();
		BallShooter.instance.SetUp(attacks);
		StartCoroutine(ScheduleSpawn());
		InvokeRepeating("IUpdateSmokeHole", 2f, 0.3f);
	}

	private void SetUp()
	{
		defaultSpeed = attacks.speed;
		GameObject gameObject = GameObject.FindWithTag("Quad");
		GameObject gameObject2 = GameObject.FindWithTag("Paths");
		if (gameObject != null)
		{
			UnityEngine.Object.DestroyImmediate(gameObject);
		}
		if (gameObject2 != null)
		{
			UnityEngine.Object.DestroyImmediate(gameObject2);
		}
		UnityEngine.Object.Instantiate(attacks.quad, attacks.quad.transform.position, Quaternion.identity);
		UnityEngine.Object.Instantiate(attacks.paths, attacks.paths.transform.position, Quaternion.identity);
	}

	private void Update()
	{
		for (int i = 0; i < spheres.Count; i++)
		{
			spheres[i].MyUpdate();
		}
		while (lateUpdateSpheres.Count > 0)
		{
			lateUpdateSpheres[0].MyLateUpdate();
			lateUpdateSpheres.RemoveAt(0);
		}
	}

	private IEnumerator ScheduleSpawn()
	{
		yield return new WaitForSeconds(0.5f);
		Trace trace = UnityEngine.Object.Instantiate(MonoUtils.instance.trace[MainController.instance.numPaths - 1]);
		trace.Init(this);
		yield return new WaitForSeconds(3f);
		for (int i = 0; i < attacks.attacks.Count; i++)
		{
			Attack attack = attacks.attacks[i];
			beginDelayTime = Time.time;
			while (Time.time - beginDelayTime < attack.delay)
			{
				yield return new WaitForSeconds(0.2f);
				if (spheres.Count == 0)
				{
					yield return new WaitForSeconds(1f);
					break;
				}
			}
			if (i == 0 && index == 1)
			{
				yield return new WaitForSeconds(UnityEngine.Random.Range(4, 8));
			}
			while (isPausing && Time.time < endPausedTime)
			{
				yield return new WaitForSeconds(endPausedTime - Time.time);
			}
			while (GameState.IsBackwarding() || (spheres.Count > 0 && DistanceToLast() < 1.04f))
			{
				yield return new WaitForSeconds(1f);
			}
			if (string.IsNullOrEmpty(attack.listBalls))
			{
				lastSphere = UnityEngine.Random.Range(0, MonoUtils.instance.spheres.Length);
				int numBalls = UnityEngine.Random.Range(attack.ballFrom, attack.ballTo + 1);
				List<int> numbers = new List<int>();
				for (int k = 2; k <= numBalls; k++)
				{
					numbers.Add(k);
				}
				List<int> hiddenBallIndexes = GetRandomValues(numbers, attack.numHidden);
				List<int> blackBallIndexes = GetRandomValues(numbers, attack.numBlack);
				Spawn(isLeader: true);
				endSlowDownTime = Time.time + GetSlowDownTime();
				yield return 0;
				do
				{
					if (spheres.Count == 0)
					{
						if (numBalls != 1)
						{
							Spawn(isLeader: true);
						}
						numBalls--;
					}
					else
					{
						float num = DistanceToLast();
						if (num >= 1.04f && numBalls != 1)
						{
							Spawn(isLeader: true);
							numBalls--;
						}
						else if (num >= 0.52f)
						{
							if (numBalls == 1)
							{
								Spawn(isLeader: false, Sphere.Type.Pusher);
							}
							else if (hiddenBallIndexes.Contains(numBalls))
							{
								Spawn(isLeader: false, Sphere.Type.Hidden);
							}
							else if (blackBallIndexes.Contains(numBalls))
							{
								Spawn(isLeader: false, Sphere.Type.Black);
							}
							else
							{
								Spawn(isLeader: false);
							}
							numBalls--;
						}
					}
					yield return 0;
				}
				while (numBalls != 0);
				continue;
			}
			string[] listBalls = attack.listBalls.Trim().Split(null);
			for (int j = 0; j < listBalls.Length; j++)
			{
				string strType = listBalls[j];
				int typeIndex = Array.IndexOf(Const.ballTypeStrings, strType);
				Sphere.Type? type = null;
				int iceLayer = 0;
				if (typeIndex != -1)
				{
					type = Const.ballTypes[typeIndex];
				}
				if (strType.Contains("n"))
				{
					type = CUtils.GetRandom<Sphere.Type>(Sphere.Type.Sphere1, Sphere.Type.Sphere2, Sphere.Type.Sphere3, Sphere.Type.Sphere4, Sphere.Type.Sphere5);
				}
				if (strType.Contains("i"))
				{
					int num2 = strType.IndexOf('i');
					iceLayer = int.Parse(strType[num2 + 1].ToString());
				}
				Sphere sphere;
				if (j == 0)
				{
					sphere = Spawn(isLeader: true, type);
					endSlowDownTime = Time.time + GetSlowDownTime();
				}
				else
				{
					while (true)
					{
						if (spheres.Count == 0)
						{
							sphere = Spawn(isLeader: true, type);
							break;
						}
						float num3 = DistanceToLast();
						if (num3 >= 1.04f)
						{
							sphere = Spawn(isLeader: true, type);
							break;
						}
						if (num3 >= 0.52f)
						{
							sphere = Spawn(isLeader: false, type);
							break;
						}
						yield return 0;
					}
				}
				sphere.iceLayer = iceLayer;
			}
			if (spheres.Count != 0)
			{
				Spawn(isLeader: false, Sphere.Type.Pusher);
			}
		}
		completeSpawning = true;
	}

	private List<int> GetRandomValues(List<int> arr, int num)
	{
		List<int> list = new List<int>();
		while (num > 0)
		{
			int random = CUtils.GetRandom(arr.ToArray());
			list.Add(random);
			arr.Remove(random);
			num--;
			if (arr.Count == 0)
			{
				break;
			}
		}
		return list;
	}

	private Sphere Spawn(bool isLeader, Sphere.Type? type = default(Sphere.Type?))
	{
		Sphere sphere = (!type.HasValue) ? SpawnRandom() : Spawn(type);
		sphere.transform.SetParent(base.transform);
		sphere.Init(this);
		if (!isLeader)
		{
			sphere.leader = spheres[spheres.Count - 1];
		}
		Sphere sphere2 = sphere;
		sphere2.onDestroyed = (Action<Sphere>)Delegate.Combine(sphere2.onDestroyed, new Action<Sphere>(OnSphereDestroyed));
		sphere.Move();
		spheres.Add(sphere);
		if (type == Sphere.Type.Pusher)
		{
			listPushers.Add(sphere);
		}
		else if (Utils.IsNormalBall(sphere))
		{
			Add2ListTypes(sphere.type);
		}
		else if (type == Sphere.Type.Hidden)
		{
			Add2ListTypes(sphere.hiddenType);
		}
		return sphere;
	}

	public static Sphere Spawn(Sphere.Type? type = default(Sphere.Type?))
	{
		if (!type.HasValue)
		{
			int num = (currentTypes != null && currentTypes.Count != 0) ? ((int)CUtils.GetRandom(currentTypes.ToArray()).type) : UnityEngine.Random.Range(0, MonoUtils.instance.spheres.Length);
			return UnityEngine.Object.Instantiate(MonoUtils.instance.spheres[num]);
		}
		if (type == Sphere.Type.MultiColor)
		{
			return UnityEngine.Object.Instantiate(MonoUtils.instance.multiColor);
		}
		if (type == Sphere.Type.Bomb)
		{
			return UnityEngine.Object.Instantiate(MonoUtils.instance.bomb);
		}
		if (type == Sphere.Type.Color)
		{
			int num2 = UnityEngine.Random.Range(0, MonoUtils.instance.spheres.Length);
			Sphere sphere = UnityEngine.Object.Instantiate(MonoUtils.instance.sphereColors[num2]);
			sphere.colorIndex = num2;
			return sphere;
		}
		if (type == Sphere.Type.Pusher)
		{
			return UnityEngine.Object.Instantiate(MonoUtils.instance.pusher);
		}
		if (type == Sphere.Type.Hidden)
		{
			Sphere sphere2 = UnityEngine.Object.Instantiate(MonoUtils.instance.hidden);
			sphere2.hiddenType = (Sphere.Type)GetNormalRandom();
			return sphere2;
		}
		if (type == Sphere.Type.Black)
		{
			return UnityEngine.Object.Instantiate(MonoUtils.instance.black);
		}
		return UnityEngine.Object.Instantiate(MonoUtils.instance.spheres[(int)type.Value]);
	}

	public Sphere SpawnRandom()
	{
		return UnityEngine.Object.Instantiate(MonoUtils.instance.spheres[GetNormalRandom()]);
	}

	private static int GetNormalRandom()
	{
		float[] obj = new float[5]
		{
			0f,
			100f,
			100f,
			100f,
			100f
		};
		obj[0] = attacks.sameBallCoef;
		float[] probs = obj;
		int num = CUtils.ChooseRandomWithProbs(probs);
		if (num == 0)
		{
			sameSpheres++;
		}
		else
		{
			sameSpheres = 1;
		}
		if (sameSpheres > attacks.maxSameBalls)
		{
			sameSpheres = 1;
			num = UnityEngine.Random.Range(1, MonoUtils.instance.spheres.Length);
		}
		return lastSphere = (num + lastSphere) % MonoUtils.instance.spheres.Length;
	}

	public Sphere SpawnPusher()
	{
		return UnityEngine.Object.Instantiate(MonoUtils.instance.pusher);
	}

	private void OnSphereDestroyed(Sphere sphere)
	{
		spheres.Remove(sphere);
		if (Utils.IsNormalBall(sphere))
		{
			RemoveFromListTypes(sphere.type);
		}
		else if (sphere.type == Sphere.Type.Hidden)
		{
			RemoveFromListTypes(sphere.hiddenType);
		}
		else if (sphere.type == Sphere.Type.Pusher)
		{
			listPushers.Remove(sphere);
		}
		else if (sphere.type == Sphere.Type.MultiColor)
		{
			listMultiColors.Remove(sphere);
		}
		if (spheres.Count == 0 && completeSpawning)
		{
			GiveComplimentOnComplete(sphere);
			MainController.instance.OnCompletePath(index);
		}
	}

	private void GiveComplimentOnComplete(Sphere lastExpodeSphere)
	{
		int pathMarker = lastExpodeSphere.pathIndex;
		Vector3 vector = lastExpodeSphere.transform.position;
		Vector3? vector2 = null;
		List<Vector3> list = new List<Vector3>();
		list.Add(vector);
		while (true)
		{
			int pathIndex = 0;
			vector2 = GetPosition(vector, pathMarker, 1, 2f, ref pathIndex);
			if (vector2.HasValue)
			{
				list.Add(vector2.Value);
				pathMarker = pathIndex;
				vector = waypoints[pathMarker];
				continue;
			}
			break;
		}
		int count = list.Count;
		float num = (float)count * 0.2f;
		MainController.instance.SetCompleteTime(Time.time + 1f + num);
		StartCoroutine(IEGiveCompliment(list, 1000));
	}

	private IEnumerator IEGiveCompliment(List<Vector3> positions, int bonusScore)
	{
		foreach (Vector3 position in positions)
		{
			yield return new WaitForSeconds(0.2f);
			MainController.instance.GiveCompliment(position, 1000);
		}
	}

	public Vector3? GetPosition(Vector3 marker, int pathMarker, int side, float distance, ref int pathIndex)
	{
		float num = 0f;
		if (side == -1)
		{
			for (int num2 = pathMarker; num2 >= 0; num2--)
			{
				Vector3 vector = waypoints[num2];
				num = ((num2 != pathMarker) ? (num + (vector - waypoints[num2 + 1]).magnitude) : (num + (vector - marker).magnitude));
				if (num >= distance)
				{
					pathIndex = num2;
					float d = num - distance;
					return vector + (waypoints[num2 - side] - vector).normalized * d;
				}
			}
		}
		else
		{
			for (int i = pathMarker + 1; i < waypoints.Count; i++)
			{
				Vector3 vector2 = waypoints[i];
				num = ((i != pathMarker + 1) ? (num + (vector2 - waypoints[i - 1]).magnitude) : (num + (vector2 - marker).magnitude));
				if (num >= distance)
				{
					pathIndex = i - 1;
					float d2 = num - distance;
					return vector2 + (waypoints[i - 1] - vector2).normalized * d2;
				}
			}
		}
		return null;
	}

	public void Insert(Sphere sphere, int index)
	{
		spheres.Insert(index, sphere);
		if (Utils.IsNormalBall(sphere))
		{
			Add2ListTypes(sphere.type);
		}
		else if (sphere.type == Sphere.Type.MultiColor)
		{
			listMultiColors.Add(sphere);
		}
		sphere.onDestroyed = (Action<Sphere>)Delegate.Combine(sphere.onDestroyed, new Action<Sphere>(OnSphereDestroyed));
	}

	public void OnListSpheresChanged(Sphere sphere, int minForExploding = 3)
	{
		Sphere next = sphere.GetNext();
		if (sphere.type == Sphere.Type.MultiColor && sphere.isHeader && next != null && next.type != Sphere.Type.Pusher)
		{
			ArrangeSpheres(isExploding: false);
			return;
		}
		List<Sphere> list = (sphere.type != Sphere.Type.MultiColor) ? sphere.GetTheSames() : sphere.GetTheSamesOfMultiColor();
		bool flag = list != null && list.Count >= minForExploding && list[0].type != Sphere.Type.Hidden;
		if (flag)
		{
			if (list[0].iceLayer == 0)
			{
				RevealHiddenSphere(list[0].leader);
				BreakIceSphere(list[0].leader);
			}
			if (list.Last().iceLayer == 0)
			{
				RevealHiddenSphere(list.Last().follower);
				BreakIceSphere(list.Last().follower);
			}
			Sphere head = list[0].GetHead();
			foreach (Sphere item in list)
			{
				if (item.iceLayer == 0)
				{
					item.status = Sphere.Status.Exploding;
					item.replacingPusher = head.replacingPusher;
					item.pendingReplacingPusher = head.pendingReplacingPusher;
				}
			}
			foreach (Sphere item2 in list)
			{
				if (item2.iceLayer != 0)
				{
					item2.BreakIce();
					item2.willExplode = false;
				}
				else
				{
					item2.Explode();
				}
			}
			BallShooter.instance.UpdateBalls();
			int count = list.Count;
			ScoreNumber.instance.AddScore(count * (count + 1) * 13);
			MainController.instance.GiveCompliment(list);
			MainController.instance.SpawnRandomItem(list);
			Sound.instance.Play(Sound.Others.Match);
		}
		else if (sphere.type != Sphere.Type.MultiColor)
		{
			for (int num = listMultiColors.Count - 1; num >= 0; num--)
			{
				OnListSpheresChanged(listMultiColors[num]);
			}
		}
		for (int num2 = listPushers.Count - 1; num2 >= 0; num2--)
		{
			Sphere sphere2 = listPushers[num2];
			int num3 = spheres.IndexOf(sphere2);
			if (sphere2.isHeader && (num3 == 0 || spheres[num3 - 1].type == Sphere.Type.Pusher))
			{
				sphere2.Explode();
				ScoreNumber.instance.AddScore(10000);
			}
		}
		ArrangeSpheres(flag);
	}

	public void ArrangeSpheres(bool isExploding)
	{
		for (int i = 0; i < spheres.Count; i++)
		{
			Sphere sphere = spheres[i];
			if (sphere.type == Sphere.Type.Pusher || !(sphere.follower == null))
			{
				continue;
			}
			Sphere previous = sphere.GetPrevious(i);
			if (previous != null)
			{
				if (sphere.type == previous.type || sphere.type == Sphere.Type.MultiColor || previous.type == Sphere.Type.Pusher || previous.type == Sphere.Type.MultiColor)
				{
					if (!GameState.IsReversing())
					{
						Reverse(sphere, isExploding);
					}
				}
				else
				{
					Stop(sphere);
				}
			}
			else if (DistanceToLast() >= 1.04f)
			{
				Stop(sphere);
			}
		}
	}

	public void RevealHiddenSphere(Sphere sphere)
	{
		if (!(sphere == null) && sphere.type == Sphere.Type.Hidden)
		{
			sphere.GetComponent<MeshRenderer>().material = MonoUtils.instance.sphereMaterials[(int)sphere.hiddenType];
			sphere.type = sphere.hiddenType;
			GameObject gameObject = UnityEngine.Object.Instantiate(MonoUtils.instance.hiddenExplosion, sphere.transform.position - Vector3.forward, Quaternion.identity);
			FollowTarget2D followTarget2D = gameObject.AddComponent<FollowTarget2D>();
			followTarget2D.target = sphere.transform;
			followTarget2D.offset = -Vector3.forward;
		}
	}

	public void BreakIceSphere(Sphere sphere)
	{
		if (!(sphere == null) && sphere.iceLayer != 0)
		{
			sphere.BreakIce();
		}
	}

	private float DistanceToLast()
	{
		return (waypoints[0] - spheres[spheres.Count - 1].transform.position).magnitude;
	}

	public static void Add2ListTypes(Sphere.Type type)
	{
		SphereType sphereType = currentTypes.Find((SphereType x) => x.type == type);
		if (sphereType == null)
		{
			sphereType = new SphereType();
			sphereType.type = type;
			currentTypes.Add(sphereType);
		}
		sphereType.number++;
	}

	public static void ChangeSphereType(Sphere sphere, Sphere.Type newType)
	{
		if (sphere.type == Sphere.Type.Hidden)
		{
			RemoveFromListTypes(sphere.hiddenType);
		}
		else
		{
			RemoveFromListTypes(sphere.type);
		}
		sphere.type = newType;
		if (Utils.IsNormalBall(sphere))
		{
			Add2ListTypes(sphere.type);
		}
	}

	public static void RemoveFromListTypes(Sphere.Type type)
	{
		SphereType sphereType = currentTypes.Find((SphereType x) => x.type == type);
		if (sphereType != null)
		{
			sphereType.number--;
			if (sphereType.number == 0)
			{
				currentTypes.Remove(sphereType);
			}
		}
	}

	private float GetSlowDownTime()
	{
		float num = (attacks.numPaths != 1) ? 0.5f : 1f;
		return (spheres.Count >= 10) ? (num * 5f) : (num * 7f);
	}

	public void Reverse(Sphere sphere, bool isExploding)
	{
		GameState.BeginReversing();
		if (isExploding)
		{
			Stop(sphere);
			Timer.Schedule(this, 0.3f, delegate
			{
				if (sphere != null && sphere.follower == null)
				{
					DoReverse(sphere);
				}
				else
				{
					GameState.EndReversing();
				}
			});
		}
		else
		{
			DoReverse(sphere);
		}
	}

	public void Stop(Sphere sphere)
	{
		Sphere sphere2 = sphere;
		while (sphere2 != null)
		{
			sphere2.SetSpeedAtOnce(0f);
			sphere2.tweeningTo = null;
			if (sphere2.isHeader)
			{
				sphere2.EndReplacingPusher();
			}
			sphere2 = sphere2.leader;
		}
	}

	public void DoReverse(Sphere sphere)
	{
		Sphere sphere2 = sphere;
		while (sphere2 != null)
		{
			sphere2.SetSpeedAtOnce(5f);
			sphere2.tweeningTo = null;
			sphere2.direction = -1;
			sphere2.OnDirectionChanged();
			if (sphere2.isHeader)
			{
				sphere2.EndReplacingPusher();
			}
			sphere2 = sphere2.leader;
		}
	}

	public static void Pause()
	{
		isPausing = true;
		endPausedTime = Time.time + 5f;
		Timer.Schedule(MainController.instance, 5f, delegate
		{
			isPausing = false;
		});
		Sound.instance.Play(Sound.Others.Pause);
	}

	public static void Backward()
	{
		GameState.BeginBackwarding();
		endBackwardingTime = Time.time + 1.3f;
		Timer.Schedule(MainController.instance, 1.3f, delegate
		{
			SphereController[] sphereControllers = MainController.instance.sphereControllers;
			foreach (SphereController sphereController in sphereControllers)
			{
				sphereController.EndBackwarding();
			}
			GameState.EndBackwarding();
		});
		Sound.instance.Play(Sound.Others.MoveBack);
	}

	private void EndBackwarding()
	{
		foreach (Sphere sphere in spheres)
		{
			if (sphere.isHeader && sphere.isBackwarding)
			{
				sphere.EndBackwarding();
			}
		}
	}

	public void OnBombTouchSphere(Sphere sphere)
	{
		List<Sphere> aroundSpheres = sphere.GetAroundSpheres(2);
		foreach (Sphere item in aroundSpheres)
		{
			item.willExplode = true;
		}
		Timer.Schedule(this, 0f, delegate
		{
			GameState.EndFlying();
			OnListSpheresChanged(sphere, 1);
		});
	}

	private void IUpdateSmokeHole()
	{
		if (spheres.Count == 0)
		{
			return;
		}
		float distanceToEnd = spheres[0].GetDistanceToEnd();
		if (distanceToEnd < 2.5f)
		{
			if (smokeHole == null)
			{
				smokeHole = UnityEngine.Object.Instantiate(MonoUtils.instance.smokeHolePrefab);
				smokeHole.transform.position = waypoints[waypoints.Count - 1] - Vector3.forward * 0.4f;
			}
		}
		else if (smokeHole != null)
		{
			UnityEngine.Object.Destroy(smokeHole.gameObject);
			smokeHole = null;
		}
	}
}
