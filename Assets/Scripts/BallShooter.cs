using Superpow;
using System;
using UnityEngine;

public class BallShooter : MonoBehaviour
{
	private delegate void ApplyUpdate();

	public enum Type
	{
		Rotate,
		Up,
		Down
	}

	private Sphere currentBall;

	private Sphere flyingBall;

	private Camera uiCamera;

	private bool isCannonPointerDown;

	private bool isReplacing;

	private bool isUpdating;

	private bool isExchanging;

	private ApplyUpdate applyUpdate;

	public Type type;

	public UIEvent uiEvent;

	public Slot[] slots = new Slot[2];

	public Vector3 currentDirection = Vector3.up;

	public static BallShooter instance;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		UIEvent uIEvent = uiEvent;
		uIEvent.onMouseDown = (Action)Delegate.Combine(uIEvent.onMouseDown, new Action(OnCannonPointerDown));
		UIEvent uIEvent2 = uiEvent;
		uIEvent2.onMouseUp = (Action)Delegate.Combine(uIEvent2.onMouseUp, new Action(OnCannonPointerUp));
		SpawnBalls();
		if (!Guide.IsDone(Guide.Type.SwitchBalls))
		{
			Timer.Schedule(this, 2f, delegate
			{
				GuideController.instance.Show(Guide.Type.SwitchBalls);
			});
		}
	}

	public void SetUp(Attacks attacks)
	{
		type = attacks.shooterType;
		if (type == Type.Rotate)
		{
			base.transform.parent.position = attacks.cannonPosition.position;
		}
	}

	private void SpawnBalls(Sphere.Type? type = default(Sphere.Type?), int? number = default(int?))
	{
		int num = 0;
		Slot[] array = slots;
		foreach (Slot slot in array)
		{
			if (slot.sphere != null && type.HasValue && (slot.sphere.type != type.GetValueOrDefault() || !type.HasValue))
			{
				UnityEngine.Object.DestroyImmediate(slot.sphere.gameObject);
				slot.sphere = null;
			}
			if (slot.sphere == null)
			{
				slot.sphere = SphereController.Spawn(type);
				slot.sphere.transform.position = slot.transform.position;
				slot.sphere.transform.SetParent(base.transform.parent);
				slot.sphere.status = Sphere.Status.Preparing;
			}
			num++;
			if (num == number)
			{
				break;
			}
		}
	}

	private void ReplaceBall()
	{
		isReplacing = true;
		iTween.MoveTo(slots[1].sphere.gameObject, iTween.Hash("position", slots[0].transform.position, "time", 0.2f, "oncompletetarget", base.gameObject, "oncomplete", "OnReplaceBallComplete"));
	}

	private void OnReplaceBallComplete()
	{
		slots[0].sphere = slots[1].sphere;
		slots[1].sphere = null;
		SpawnBalls();
		isReplacing = false;
		if (applyUpdate != null)
		{
			applyUpdate();
		}
	}

	public void UpdateBalls()
	{
		applyUpdate = UpdateBalls;
		if (isReplacing || isExchanging)
		{
			return;
		}
		Slot[] array = slots;
		foreach (Slot slot in array)
		{
			if (slot.sphere != null && !Utils.IsSpecialBall(slot.sphere) && SphereController.currentTypes.Find((SphereType x) => x.type == slot.sphere.type) == null)
			{
				UnityEngine.Object.DestroyImmediate(slot.sphere.gameObject);
				slot.sphere = null;
			}
		}
		SpawnBalls();
		applyUpdate = null;
	}

	private void ExchangeBalls()
	{
		if (!(slots[0].sphere == null) && !(slots[1].sphere == null))
		{
			isExchanging = true;
			iTween.MoveTo(slots[1].sphere.gameObject, slots[0].transform.position, 0.2f);
            iTween.MoveTo(slots[0].sphere.gameObject, slots[1].transform.position, 0.4f);
            CUtils.Swap(ref slots[0].sphere, ref slots[1].sphere);

            
            Timer.Schedule(this, 0.2f, delegate
			{
				isExchanging = false;
				if (applyUpdate != null)
				{
					applyUpdate();
				}
			});
		}
	}

	public void CreateMultiColors()
	{
		applyUpdate = CreateMultiColors;
		if (!isReplacing && !isExchanging)
		{
			Timer.Schedule(this, 0f, delegate
			{
				SpawnBalls(Sphere.Type.MultiColor);
				applyUpdate = null;
				GameState.EndFlying();
			});
		}
	}

	public void CreateBomb()
	{
		applyUpdate = CreateBomb;
		if (!isReplacing && !isExchanging)
		{
			Timer.Schedule(this, 0f, delegate
			{
				SpawnBalls(Sphere.Type.Bomb, 1);
				applyUpdate = null;
				GameState.EndFlying();
			});
		}
	}

	public void CreateColor()
	{
		applyUpdate = CreateColor;
		if (!isReplacing && !isExchanging)
		{
			Timer.Schedule(this, 0f, delegate
			{
				SpawnBalls(Sphere.Type.Color, 1);
				applyUpdate = null;
				GameState.EndFlying();
			});
		}
	}

	private void OnCannonPointerDown()
	{
		isCannonPointerDown = true;
		ExchangeBalls();
		if (GuideController.instance.IsShowing(Guide.Type.SwitchBalls))
		{
			GuideController.instance.Done(Guide.Type.SwitchBalls);
		}
	}

	private void OnCannonPointerUp()
	{
		isCannonPointerDown = false;
	}

	private void Shoot(Vector3 touchPoint)
	{
		if (!(slots[0].sphere == null) && GameState.IsNormal())
		{
			flyingBall = slots[0].sphere;
			if (type == Type.Rotate)
			{
				currentDirection = (touchPoint - base.transform.parent.position).normalized;
				flyingBall.flyingDirection = currentDirection;
				base.transform.parent.rotation = Quaternion.FromToRotation(Vector3.up, currentDirection);
			}
			else
			{
				base.transform.parent.SetX(touchPoint.x);
				flyingBall.flyingDirection = Vector3.up;
			}
			flyingBall.Fly();
			slots[0].sphere = null;
			ReplaceBall();
			MainController.instance.OnCannonShoot();
			Sound.instance.Play(Sound.Others.Shoot);
		}
	}

	private void Update()
	{
		if (Input.GetMouseButton(0) && !isCannonPointerDown && !CUtils.IsPointerOverUIObject() && !DialogController.instance.IsDialogShowing() && MainController.instance.status == MainController.Status.Playing && MonoUtils.instance.numDroppingBomb == 0)
		{
			Vector3 touchPoint = Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
			touchPoint.z = 0f;
			Shoot(touchPoint);
		}
	}
}
