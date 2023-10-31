using System;
using UnityEngine;
using UnityEngine.UI;
//using UnityScript.Lang;

[Serializable]
public class SM_effectCaster : MonoBehaviour
{
	public GameObject moveThis;

	public RaycastHit hit;

	public GameObject[] createThis;

	public float cooldown;

	public float changeCooldown;

	public int selected;

	public Text writeThis;

	private float rndNr;

	private GameObject effect;

	public void Start()
	{
		//selected = Extensions.get_length((System.Array)createThis) - 1;
		writeThis.text = selected.ToString() + " " + createThis[selected].name;
	}

	public void Update()
	{
		if (!(cooldown <= 0f))
		{
			cooldown -= Time.deltaTime;
		}
		if (!(changeCooldown <= 0f))
		{
			changeCooldown -= Time.deltaTime;
		}
		Ray ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
		if (Physics.Raycast(ray, out hit))
		{
			moveThis.transform.position = hit.point;
			if (Input.GetMouseButton(0) && !(cooldown > 0f))
			{
				effect = UnityEngine.Object.Instantiate(createThis[selected], moveThis.transform.position, moveThis.transform.rotation);
				if (effect.CompareTag("explosion") || effect.CompareTag("missile") || effect.CompareTag("breath"))
				{
					Vector3 position = effect.transform.position;
					float y = position.y + 1.5f;
					Vector3 position2 = effect.transform.position;
					position2.y = y;
					Vector3 vector2 = effect.transform.position = position2;
				}
				if (effect.CompareTag("shield"))
				{
					Vector3 position3 = effect.transform.position;
					float y2 = position3.y + 0.5f;
					Vector3 position4 = effect.transform.position;
					position4.y = y2;
					Vector3 vector4 = effect.transform.position = position4;
				}
				cooldown = 0.15f;
			}
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.UpArrow) && !(changeCooldown > 0f))
		{
			selected++;
			//if (selected > Extensions.get_length((System.Array)createThis) - 1)
			//{
			//	selected = 0;
			//}
			writeThis.text = selected.ToString() + " " + createThis[selected].name;
			changeCooldown = 0.1f;
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.DownArrow) && !(changeCooldown > 0f))
		{
			selected--;
			if (selected < 0)
			{
				//selected = Extensions.get_length((System.Array)createThis) - 1;
			}
			writeThis.text = selected.ToString() + " " + createThis[selected].name;
			changeCooldown = 0.1f;
		}
	}

	public void Main()
	{
	}
}
