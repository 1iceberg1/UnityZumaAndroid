using UnityEngine;

public class NotificationAlert : CMonoBehaviour
{
	public enum Type
	{
		taskComplete
	}

	public GameObject notificationObj;

	public Animator anim;

	public Type alertType;

	public float showAnimationTime = 5f;

	protected virtual void Start()
	{
		if (anim == null)
		{
			anim = GetComponent<Animator>();
		}
		bool active = IsNotificationAlertVisible();
		notificationObj.SetActive(active);
	}

	protected void OnAlertNotification()
	{
		notificationObj.SetActive(value: true);
		anim.SetBool("show", value: true);
		Invoke("OnAlertComplete", showAnimationTime);
	}

	protected void OnAlertComplete()
	{
		if (anim.isActiveAndEnabled)
		{
			anim.SetBool("show", value: false);
		}
	}

	public void OnHideNotification()
	{
		notificationObj.SetActive(value: false);
	}

	public virtual bool IsNotificationAlertVisible()
	{
		return false;
	}
}
