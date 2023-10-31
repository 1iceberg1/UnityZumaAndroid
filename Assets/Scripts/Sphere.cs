using Superpow;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Utility;

public class Sphere : MonoBehaviour
{
	public enum Status
	{
		Preparing,
		Joining,
		Moving,
		Flying,
		Exploding,
		Destroyed
	}

	public enum Type
	{
		Sphere1,
		Sphere2,
		Sphere3,
		Sphere4,
		Sphere5,
		Pusher,
		MultiColor,
		Bomb,
		WillExplode,
		Trace,
		Color,
		Hidden,
		Black
	}

	private Vector3 rotateAxis;

	private Vector3 velocity;

	private float beginJointTime;

	protected bool pushing;

	protected bool pendingEndReplacingPusher;

	protected bool isPausing;

	protected bool needBackwarding;

	protected bool isFallingHole;

	private Sphere touchedPusher;

	private float speedBeforeBackwarding;

	private float speedBeforePushing;

	private int _direction = 1;

	private GameObject shadow;

	private GameObject iceCover;

	public const float RADIUS = 0.26f;

	public const float FLYING_SPEED = 12f;

	public const float REVERSE_SPEED = 5f;

	public const float BACKWARD_SPEED = 2.5f;

	public const float PUSH_SPEED = 3.5f;

	public const float REPLACE_SPEED = 3.4f;

	public const float JOIN_TIME = 0.06f;

	public const float SLOWDOWN_TIME_LONG = 7f;

	public const float SLOWDOWN_TIME_SHORT = 5f;

	public Status status = Status.Moving;

	public Type type;

	public float speed;

	[HideInInspector]
	public int pathIndex;

	[HideInInspector]
	public Vector3 flyingDirection;

	[HideInInspector]
	public Sphere jointLeader;

	[HideInInspector]
	public Sphere jointFollower;

	[HideInInspector]
	public SphereController controller;

	[HideInInspector]
	public bool isBackwarding;

	[HideInInspector]
	public bool replacingPusher;

	[HideInInspector]
	public bool pendingReplacingPusher;

	[HideInInspector]
	public bool willExplode;

	[HideInInspector]
	public int colorIndex;

	[HideInInspector]
	public Type hiddenType;

	[HideInInspector]
	public int iceLayer;

	public float? tweeningTo;

	public Action<Sphere> onDestroyed;

	public Sphere _leader;

	public Sphere _follower;

	public List<Sphere> spheres => controller.spheres;

	public List<Vector3> waypoints => controller.waypoints;

	public Sphere leader
	{
		get
		{
			return _leader;
		}
		set
		{
			_leader = value;
			if (_leader != null)
			{
				_leader._follower = this;
			}
		}
	}

	public Sphere follower
	{
		get
		{
			return _follower;
		}
		set
		{
			_follower = value;
			if (_follower != null)
			{
				_follower._leader = this;
			}
		}
	}

	private Sphere newLeader => (!(leader != null) || leader.type == Type.Hidden) ? null : leader;

	private Sphere newFollower => (!(follower != null) || follower.type == Type.Hidden || follower.type == Type.Pusher) ? null : follower;

	public bool isHeader => leader == null;

	private Vector3 destination => (direction != 1) ? waypoints[pathIndex] : waypoints[pathIndex + 1];

	public int direction
	{
		get
		{
			return _direction;
		}
		set
		{
			if (_direction != value)
			{
				_direction = value;
				OnDirectionChanged();
			}
		}
	}

	public void Init(SphereController controller)
	{
		this.controller = controller;
		base.transform.position = controller.waypoints[0];
	}

	private void Start()
	{
		if (isHeader && status == Status.Moving && type != Type.Trace)
		{
			tweeningTo = controller.defaultSpeed;
			SetSpeed(4f);
		}
		if (Utils.IsNormalBall(this) || type == Type.MultiColor || type == Type.MultiColor)
		{
			shadow = UnityEngine.Object.Instantiate(MonoUtils.instance.ballShadow);
			if (controller != null)
			{
				shadow.transform.SetParent(controller.transform);
			}
			UpdateShadow();
		}
		if (iceLayer != 0)
		{
			iceCover = UnityEngine.Object.Instantiate(MonoUtils.instance.iceCover);
			iceCover.GetComponent<SpriteRenderer>().sprite = MonoUtils.instance.iceCoverSprites[iceLayer - 1];
			if (controller != null)
			{
				iceCover.transform.SetParent(controller.transform);
			}
			UpdateIceCover();
		}
	}

	public void ChangeSpeedValue(float from, float to, float time)
	{
		if (time == 0f || from == to)
		{
			speed = to;
			return;
		}
		speed = from;
		iTween.ValueTo(base.gameObject, iTween.Hash("from", from, "to", to, "time", time, "easetype", iTween.EaseType.linear, "onupdate", "OnUpdateSpeed", "oncomplete", "OnCompleteUpdateSpeed"));
	}

	private void OnUpdateSpeed(float value)
	{
		speed = value;
	}

	private void OnCompleteUpdateSpeed()
	{
		tweeningTo = null;
	}

	private void CalculateRotateAxis()
	{
		if (waypoints != null && pathIndex + direction >= 0 && pathIndex >= 0)
		{
			Vector3 vector = waypoints[pathIndex + direction] - waypoints[pathIndex];
			rotateAxis = new Vector3(0f - vector.y, vector.x, 0f).normalized;
		}
	}

	public void Move()
	{
		status = Status.Moving;
	}

	public void Fly()
	{
		status = Status.Flying;
		GameState.BeginFlying();
		base.transform.SetParent(null);
	}

	public void BeginJoining()
	{
		status = Status.Joining;
		GameState.BeginJoining();
	}

	private void BeginBackwarding()
	{
		isBackwarding = true;
		speedBeforeBackwarding = speed;
		SetSpeedAtOnce(2.5f);
		direction = -1;
		OnDirectionChanged();
	}

	public void EndBackwarding()
	{
		isBackwarding = false;
		SetSpeed(speedBeforeBackwarding);
		direction = 1;
		OnDirectionChanged();
	}

	private void UpdateShadow()
	{
		shadow.transform.position = base.transform.position + new Vector3(0.07f, -0.07f, 0f);
	}

	private void UpdateIceCover()
	{
		iceCover.transform.position = base.transform.position - Vector3.forward * 0.269999981f;
	}

	public void BreakIce()
	{
		iceLayer--;
		if (iceLayer == 0)
		{
			UnityEngine.Object.Destroy(iceCover);
		}
		else
		{
			iceCover.GetComponent<SpriteRenderer>().sprite = MonoUtils.instance.iceCoverSprites[iceLayer - 1];
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(MonoUtils.instance.breakIceFx[iceLayer]);
		gameObject.transform.SetParent(base.transform);
		gameObject.transform.localPosition = -Vector3.forward * 0.3f;
		UnityEngine.Object.Destroy(gameObject, 1f);
	}

	private void Update()
	{
		if (status == Status.Flying)
		{
			base.transform.position += flyingDirection * Time.deltaTime * 12f;
			UpdateRotation(BallShooter.instance.currentDirection, 200f);
		}
		else if (status == Status.Preparing)
		{
			UpdateRotation(BallShooter.instance.currentDirection, 500f);
		}
		else if (status == Status.Joining)
		{
			base.transform.Rotate(Vector3.forward * Time.deltaTime * -200f, Space.World);
		}
		if (shadow != null)
		{
			UpdateShadow();
		}
		if (iceLayer != 0)
		{
			UpdateIceCover();
		}
	}

	public void MyUpdate()
	{
		if (status == Status.Moving)
		{
			if (isHeader)
			{
				isPausing = ((SphereController.isPausing && Mathf.Approximately(speed, controller.defaultSpeed)) || (GameState.pauseGame && type != Type.Trace));
				needBackwarding = (GameState.IsBackwarding() && (Mathf.Approximately(speed, controller.defaultSpeed) || speed == 0f));
				if (!isPausing)
				{
					if (needBackwarding)
					{
						BeginBackwarding();
					}
					if (UpdateLeaderPosition(speed))
					{
						UpdateRotation(speed);
					}
				}
				return;
			}
			CopyArgs(leader);
			Vector3? position = GetPosition(leader, -1, 0.52f, ref pathIndex);
			if (position.HasValue)
			{
				base.transform.position = position.Value;
				if (!isPausing)
				{
					UpdateRotation(speed);
				}
			}
		}
		else
		{
			if (status != Status.Joining)
			{
				return;
			}
			Vector3? vector = null;
			if (jointFollower != null)
			{
				vector = GetPosition(jointFollower, jointFollower.direction, 0.52f, ref pathIndex);
			}
			else if (jointLeader != null)
			{
				vector = GetPosition(jointLeader, -jointLeader.direction, 0.26f, ref pathIndex);
			}
			if (!vector.HasValue)
			{
				MyDestroy();
				GameState.EndJoining();
				return;
			}
			base.transform.position = Vector3.SmoothDamp(base.transform.position, vector.Value, ref velocity, 0.06f);
			if ((!(jointFollower == null) || !(Time.time - beginJointTime > 0.06f)) && (!(jointLeader == null) || !(Time.time - beginJointTime > 0.06f)) && (!(jointFollower != null) || !(jointLeader != null) || !(GetDistance(jointFollower, jointLeader) > 1.01399994f)))
			{
				return;
			}
			leader = jointLeader;
			if (jointLeader == null)
			{
				base.transform.position = vector.Value;
				tweeningTo = jointFollower.tweeningTo;
				SetSpeed(jointFollower.speed);
			}
			if (jointLeader != null)
			{
				float num = jointLeader.speed - 3.5f + ((!(jointFollower != null)) ? 0f : jointFollower.speed) - speedBeforePushing;
				Sphere head = GetHead();
				Sphere sphere = head;
				while (sphere != null)
				{
					if (jointFollower != null)
					{
						sphere.tweeningTo = jointFollower.tweeningTo;
					}
					sphere.direction = 1;
					sphere.OnDirectionChanged();
					if (sphere.isHeader)
					{
						if (head.pendingEndReplacingPusher)
						{
							head.pendingEndReplacingPusher = false;
							num -= 3.4f;
							GameState.EndReplacingPusher();
						}
						head.pushing = false;
						head.SetSpeed(num);
					}
					else
					{
						sphere.SetSpeedAtOnce(num);
					}
					sphere = sphere.follower;
				}
			}
			else if (pendingReplacingPusher)
			{
				pendingReplacingPusher = false;
				Sphere next = GetNext();
				if (next != null && next.type == Type.Pusher)
				{
					BeginReplacingPusher(next);
				}
			}
			controller.lateUpdateSpheres.Add(this);
		}
	}

	public void MyLateUpdate()
	{
		if (status == Status.Joining)
		{
			status = Status.Moving;
			if (jointFollower != null)
			{
				jointFollower.leader = this;
			}
			GameState.EndJoining();
			base.transform.SetParent(controller.transform);
			if (shadow != null)
			{
				shadow.transform.SetParent(controller.transform);
			}
			Sphere follower = this.follower;
			while (follower != null)
			{
				follower.SetSpeedAtOnce(speed);
				follower = follower.follower;
			}
			controller.OnListSpheresChanged(this);
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		GameObject gameObject = collider.gameObject;
		if (gameObject.tag == "Sphere")
		{
			Sphere component = gameObject.GetComponent<Sphere>();
			if (status == Status.Flying && component.status == Status.Moving)
			{
				controller = component.controller;
				if (component.iceLayer != 0)
				{
					component.BreakIce();
				}
				if (component.type == Type.Pusher)
				{
					touchedPusher = component;
					return;
				}
				if (type == Type.Bomb)
				{
					UnityEngine.Object.Instantiate(MonoUtils.instance.bombExplosion, component.transform.position - Vector3.forward, Quaternion.identity);
					MyDestroy();
					controller.OnBombTouchSphere(component);
					Sound.instance.Play(Sound.Others.Explosion);
					return;
				}
				if (type == Type.Color)
				{
					MyDestroy();
					List<Sphere> aroundSpheres = component.GetAroundSpheres(2);
					foreach (Sphere item in aroundSpheres)
					{
						item.GetComponent<MeshRenderer>().material = MonoUtils.instance.sphereMaterials[colorIndex];
						SphereController.ChangeSphereType(item, (Type)colorIndex);
						GameObject gameObject2 = UnityEngine.Object.Instantiate(MonoUtils.instance.colorExplosion, item.transform.position - Vector3.forward, Quaternion.identity);
						UnityEngine.Object.Destroy(gameObject2, 3f);
						FollowTarget2D followTarget2D = gameObject2.AddComponent<FollowTarget2D>();
						followTarget2D.target = item.transform;
						followTarget2D.offset = -Vector3.forward;
					}
					GameState.EndFlying();
					controller.ArrangeSpheres(isExploding: false);
					Sound.instance.Play(Sound.Others.ChangeColor);
					return;
				}
				Sound.instance.Play(Sound.Others.Hit);
				GameState.EndFlying();
				BeginJoining();
				int num = 0;
				Vector3? vector = GetPosition(component, 1, 0.52f, ref num);
				Vector3? vector2 = GetPosition(component, -1, 0.52f, ref num);
				if (!vector.HasValue || !vector2.HasValue)
				{
					Vector3 normalized = (component.destination - component.transform.position).normalized;
					vector = component.transform.position + normalized * 0.26f * 2f;
					vector2 = component.transform.position - normalized * 0.26f * 2f;
				}
				float sqrMagnitude = (base.transform.position - vector.Value).sqrMagnitude;
				float sqrMagnitude2 = (base.transform.position - vector2.Value).sqrMagnitude;
				float num2 = Vector3.Angle(flyingDirection, vector.Value - base.transform.position);
				float num3 = Vector3.Angle(flyingDirection, vector2.Value - base.transform.position);
				bool flag = true;
				if (!(sqrMagnitude < sqrMagnitude2))
				{
					flag = ((sqrMagnitude2 > sqrMagnitude / 1.5f && num2 < num3 / 1.5f) ? true : false);
				}
				else if (sqrMagnitude > sqrMagnitude2 / 1.5f && num3 < num2 / 1.5f)
				{
					flag = false;
				}
				if (flag)
				{
					jointLeader = component.leader;
					jointFollower = component;
					controller.Insert(this, spheres.IndexOf(component));
				}
				else
				{
					jointLeader = component;
					jointFollower = component.follower;
					controller.Insert(this, spheres.IndexOf(component) + 1);
				}
				PushHeadToJoin();
				beginJointTime = Time.time;
				if (jointLeader == null && touchedPusher != null)
				{
					pendingReplacingPusher = true;
				}
			}
			else if (status == Status.Moving && isHeader && component.status == Status.Moving)
			{
				if (!(GetNext() == component))
				{
					return;
				}
				bool reversing = component.direction != direction;
				if (component.type != Type.Pusher)
				{
					if (replacingPusher && !pushing)
					{
						EndReplacingPusher();
						SetSpeed(speed - 3.4f);
					}
					Sphere sphere = component;
					while (sphere != null)
					{
						sphere.tweeningTo = tweeningTo;
						sphere.direction = direction;
						if (sphere.isHeader)
						{
							if (replacingPusher && pushing)
							{
								replacingPusher = false;
								sphere.pendingEndReplacingPusher = true;
							}
							if (pendingEndReplacingPusher)
							{
								sphere.pendingEndReplacingPusher = true;
							}
							if (pushing)
							{
								pushing = false;
								sphere.pushing = true;
							}
							sphere.SetSpeed(speed);
						}
						else
						{
							sphere.SetSpeedAtOnce(speed);
						}
						sphere = sphere.leader;
					}
					leader = component;
					Timer.Schedule(this, 0f, delegate
					{
						if (reversing)
						{
							GameState.EndReversing();
							controller.OnListSpheresChanged(this);
						}
					});
				}
				else
				{
					BeginReplacingPusher(component);
				}
			}
			else
			{
				if (status != Status.Joining || (!(jointLeader == null) && !(jointFollower == null)))
				{
					return;
				}
				int index = spheres.IndexOf(this);
				if (jointLeader == null && GetNext(index) == component)
				{
					if (component.type == Type.Pusher)
					{
						pendingReplacingPusher = true;
						return;
					}
					jointLeader = component;
					PushHeadToJoin();
				}
				else if (jointFollower == null && GetPrevious(index) == component)
				{
					jointFollower = component;
				}
			}
		}
		else if (gameObject.tag == "Item" && status == Status.Flying)
		{
			GameState.EndFlying();
			UnityEngine.Object.Destroy(shadow);
			Explode();
		}
	}

	private void OnTriggerExit(Collider collider)
	{
		GameObject gameObject = collider.gameObject;
		if (gameObject.tag == "Sphere")
		{
			Sphere component = gameObject.GetComponent<Sphere>();
			if (status == Status.Flying && component.type == Type.Pusher)
			{
				touchedPusher = null;
			}
		}
	}

	private void PushHeadToJoin()
	{
		if (jointLeader != null)
		{
			Sphere head = GetHead(jointLeader);
			speedBeforePushing = head.speed;
			head.SetSpeedAtOnce(head.speed + 3.5f);
			head.pushing = true;
			jointLeader.OnCut();
		}
	}

	public Sphere GetHead()
	{
		return GetHead(this);
	}

	public Sphere GetTail()
	{
		return GetTail(this);
	}

	public Sphere GetHead(Sphere fromSphere)
	{
		Sphere sphere = fromSphere;
		while (!sphere.isHeader)
		{
			sphere = sphere.leader;
		}
		return sphere;
	}

	public Sphere GetTail(Sphere fromSphere)
	{
		Sphere sphere = fromSphere;
		while (sphere.follower != null)
		{
			sphere = sphere.follower;
		}
		return sphere;
	}

	public Sphere GetNext()
	{
		return GetNext(spheres.IndexOf(this));
	}

	public Sphere GetPrevious()
	{
		return GetPrevious(spheres.IndexOf(this));
	}

	public Sphere GetNext(int index)
	{
		return (index - 1 < 0) ? null : spheres[index - 1];
	}

	public Sphere GetPrevious(int index)
	{
		return (index + 1 >= spheres.Count) ? null : spheres[index + 1];
	}

	public List<Sphere> GetTheSames()
	{
		List<Sphere> list = new List<Sphere>();
		Sphere sphere = this;
		while (sphere.leader != null && (sphere.leader.type == type || (sphere.leader.willExplode && sphere.willExplode)))
		{
			sphere = sphere.leader;
		}
		list.Add(sphere);
		while (sphere.follower != null && (sphere.follower.type == type || (sphere.follower.willExplode && sphere.willExplode)))
		{
			sphere = sphere.follower;
			list.Add(sphere);
		}
		return list;
	}

	public List<Sphere> GetTheSamesOfMultiColor()
	{
		List<Sphere>[] array = new List<Sphere>[5];
		if (newLeader != null && newFollower != null)
		{
			array[0] = leader.GetTheSames();
			List<Sphere> theSames = follower.GetTheSames();
			array[0].Add(this);
			array[0].AddRange(theSames);
		}
		else if (newFollower != null)
		{
			array[1] = new List<Sphere>();
			array[1].Add(this);
			array[1].AddRange(follower.GetTheSames());
			if (follower.newFollower != null)
			{
				array[2] = new List<Sphere>();
				array[2].Add(this);
				array[2].Add(follower);
				array[2].Add(follower.follower);
			}
		}
		else if (newLeader != null)
		{
			array[3] = leader.GetTheSames();
			array[3].Add(this);
			if (leader.newLeader != null)
			{
				array[4] = new List<Sphere>();
				array[4].Add(this);
				array[4].Add(leader);
				array[4].Add(leader.leader);
			}
		}
		int num = 0;
		int num2 = -1;
		for (int i = 0; i < 5; i++)
		{
			if (array[i] != null && array[i].Count > num)
			{
				num = array[i].Count;
				num2 = i;
			}
		}
		return (num2 == -1) ? null : array[num2];
	}

	public List<Sphere> GetAroundSpheres(int radius)
	{
		List<Sphere> list = new List<Sphere>();
		Sphere sphere = this;
		for (int i = 0; i < radius; i++)
		{
			if (!(sphere.leader != null))
			{
				break;
			}
			sphere = sphere.leader;
			list.Add(sphere);
		}
		sphere = this;
		list.Add(this);
		for (int j = 0; j < radius; j++)
		{
			if (!(sphere.follower != null))
			{
				break;
			}
			if (sphere.follower.type == Type.Pusher)
			{
				break;
			}
			sphere = sphere.follower;
			list.Add(sphere);
		}
		return list;
	}

	private void InsertBetween(Sphere a, Sphere c)
	{
		leader = a;
		c.leader = this;
	}

	private float GetDistance(Sphere a, Sphere b)
	{
		float num = 0f;
		for (int i = a.pathIndex; i <= b.pathIndex; i++)
		{
			num = ((i != b.pathIndex) ? (num + (waypoints[i + 1] - waypoints[i]).magnitude) : (num + b.DistanceToIndex()));
		}
		return num - a.DistanceToIndex();
	}

	public float GetDistanceToEnd()
	{
		float num = 0f;
		for (int i = pathIndex; i < waypoints.Count - 1; i++)
		{
			num += (waypoints[i + 1] - waypoints[i]).magnitude;
		}
		return num;
	}

	private float DistanceToIndex()
	{
		return (base.transform.position - waypoints[pathIndex]).magnitude;
	}

	public float GetRemainingTime()
	{
		return Mathf.Max(controller.endSlowDownTime - Time.time, 0f);
	}

	private Vector3? GetPosition(Sphere marker, int side, float distance, ref int pathIndex)
	{
		return controller.GetPosition(marker.transform.position, marker.pathIndex, side, distance, ref pathIndex);
	}

	public void CopyArgs(Sphere sphere)
	{
		speed = sphere.speed;
		tweeningTo = sphere.tweeningTo;
		direction = sphere.direction;
		isPausing = sphere.isPausing;
	}

	private bool UpdateLeaderPosition(float speed)
	{
		float num = Time.deltaTime * speed - Vector3.Distance(base.transform.position, destination);
		if (num > 0f)
		{
			pathIndex += direction;
			for (int i = pathIndex; i < waypoints.Count - 1 && i > 0; i += direction)
			{
				num -= (waypoints[i + direction] - waypoints[i]).magnitude;
				if (num <= 0f || i == waypoints.Count - 2)
				{
					pathIndex = i;
					break;
				}
			}
			if (pathIndex >= waypoints.Count - 1)
			{
				isFallingHole = true;
				MyDestroy();
				return false;
			}
			if (pathIndex < 0)
			{
				pathIndex = 0;
			}
		}
		base.transform.position += (destination - base.transform.position).normalized * Time.deltaTime * speed;
		return true;
	}

	protected virtual void UpdateRotation(float speed)
	{
		CalculateRotateAxis();
		base.transform.Rotate(rotateAxis * Time.deltaTime * -160f * speed, Space.World);
	}

	private void UpdateRotation(Vector3 velocity, float speed)
	{
		if (type != Type.Bomb && type != Type.Pusher)
		{
			rotateAxis = new Vector3(0f - velocity.y, velocity.x, 0f).normalized;
			base.transform.Rotate(rotateAxis * Time.deltaTime * (0f - speed), Space.World);
		}
	}

	private void OnCut()
	{
		if (follower != null)
		{
			follower.leader = null;
		}
		follower = null;
	}

	public void OnDirectionChanged()
	{
		if (type == Type.Pusher)
		{
			base.transform.Find("Wheel1").GetComponent<Animator>().SetTrigger((direction != 1) ? "backward" : "forward");
			base.transform.Find("Wheel2").GetComponent<Animator>().SetTrigger((direction != 1) ? "backward" : "forward");
		}
	}

	public void SetSpeedAtOnce(float speed)
	{
		iTween.Stop(base.gameObject);
		this.speed = speed;
	}

	public void SetSpeed(float speed)
	{
		iTween.Stop(base.gameObject);
		if (!pushing && !replacingPusher)
		{
			float? num = tweeningTo;
			if (num.HasValue)
			{
				float? num2 = tweeningTo;
				ChangeSpeedValue(speed, num2.Value, GetRemainingTime());
				return;
			}
		}
		this.speed = speed;
	}

	public void Explode()
	{
		status = Status.Exploding;
		MyDestroy();
		UnityEngine.Object.Instantiate(MonoUtils.instance.sphereExplosion, base.transform.position, Quaternion.identity);
	}

	public void BeginReplacingPusher(Sphere pusher)
	{
		if (pusher != null)
		{
			pusher.Explode();
		}
		if (!(spheres[0] == this) && !GameState.IsReplacingPusher() && !(leader != null))
		{
			SetSpeedAtOnce(speed + 3.4f);
			replacingPusher = true;
			GameState.BeginReplacingPusher();
		}
	}

	public void EndReplacingPusher()
	{
		if (replacingPusher)
		{
			replacingPusher = false;
			GameState.EndReplacingPusher();
		}
		pendingEndReplacingPusher = false;
	}

	private void OnRemove()
	{
		if (this.follower != null)
		{
			this.follower.leader = null;
			if (status == Status.Moving || (status == Status.Exploding && this.follower.status == Status.Moving))
			{
				if (replacingPusher || pendingReplacingPusher)
				{
					speed -= 3.4f;
				}
				Sphere follower = this.follower;
				while (follower != null)
				{
					this.follower.tweeningTo = tweeningTo;
					if (follower == this.follower)
					{
						follower.pushing = pushing;
						follower.SetSpeed(speed);
					}
					else
					{
						follower.SetSpeedAtOnce(speed);
					}
					follower = follower.follower;
				}
			}
		}
		if (leader != null)
		{
			leader.follower = null;
		}
		EndReplacingPusher();
	}

	public void MyDestroy()
	{
		OnRemove();
		if (status == Status.Exploding)
		{
			GetComponent<SphereCollider>().enabled = false;
			UnityEngine.Object.Destroy(base.gameObject, 0.4f);
			if (shadow != null)
			{
				UnityEngine.Object.Destroy(shadow, 0.4f);
			}
			iTween.ScaleTo(base.gameObject, Vector3.zero, 0.4f);
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
			if (shadow != null)
			{
				UnityEngine.Object.Destroy(shadow);
			}
			if (isFallingHole && type != Type.Trace && type != Type.Pusher)
			{
				MainController.instance.IncreaseFallingHole();
			}
		}
		if (onDestroyed != null)
		{
			onDestroyed(this);
		}
		status = Status.Destroyed;
	}
}
