using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Transition : MonoBehaviour
{
	private static GameObject m_canvas;

	private GameObject m_overlay;

	private void Awake()
	{
		m_canvas = new GameObject("TransitionCanvas");
		Canvas canvas = m_canvas.AddComponent<Canvas>();
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		CanvasScaler canvasScaler = m_canvas.AddComponent<CanvasScaler>();
		canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
		canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
		canvasScaler.referenceResolution = new Vector2(720f, 1280f);
		canvasScaler.matchWidthOrHeight = 0.5f;
		Object.DontDestroyOnLoad(m_canvas);
	}

	public static void LoadLevel(string level, float duration, Color color)
	{
		GameObject gameObject = new GameObject("Transition");
		gameObject.AddComponent<Transition>();
		gameObject.GetComponent<Transition>().StartFade(level, duration, color);
		gameObject.transform.SetParent(m_canvas.transform, worldPositionStays: false);
		gameObject.transform.SetAsLastSibling();
	}

	private void StartFade(string level, float duration, Color fadeColor)
	{
		StartCoroutine(RunFade(level, duration, fadeColor));
	}

	private IEnumerator RunFade(string level, float duration, Color fadeColor)
	{
		Texture2D bgTex = new Texture2D(1, 1);
		bgTex.SetPixel(0, 0, fadeColor);
		bgTex.Apply();
		m_overlay = new GameObject("Background Layer");
		Image image = m_overlay.AddComponent<Image>();
		Rect rect = new Rect(0f, 0f, bgTex.width, bgTex.height);
		Sprite sprite = Sprite.Create(bgTex, rect, new Vector2(0.5f, 0.5f), 1f);
		image.material.mainTexture = bgTex;
		image.sprite = sprite;
		Color newColor = image.color = image.color;
		image.canvasRenderer.SetAlpha(0f);
		m_overlay.transform.localScale = new Vector3(1f, 1f, 1f);
		m_overlay.GetComponent<RectTransform>().sizeDelta = new Vector2(900f, 1400f);
		m_overlay.transform.SetParent(m_canvas.transform, worldPositionStays: false);
		m_overlay.transform.SetAsFirstSibling();
		GameObject loadingImage = UnityEngine.Object.Instantiate(Resources.Load(CUtils.LoadingImage())) as GameObject;
		loadingImage.transform.SetParent(m_canvas.transform, worldPositionStays: false);
		float time2 = 0f;
		float halfDuration = duration / 2f;
		while (time2 < halfDuration)
		{
			time2 += Time.deltaTime;
			image.canvasRenderer.SetAlpha(Mathf.InverseLerp(0f, 1f, time2 / halfDuration));
			loadingImage.GetComponent<Image>().canvasRenderer.SetAlpha(Mathf.InverseLerp(0f, 1f, time2 / halfDuration));
			yield return new WaitForEndOfFrame();
		}
		image.canvasRenderer.SetAlpha(1f);
		yield return new WaitForEndOfFrame();
		SceneManager.LoadScene(level);
		time2 = 0f;
		while (time2 < halfDuration)
		{
			time2 += Time.deltaTime;
			image.canvasRenderer.SetAlpha(Mathf.InverseLerp(1f, 0f, time2 / halfDuration));
			loadingImage.GetComponent<Image>().canvasRenderer.SetAlpha(Mathf.InverseLerp(1f, 0f, time2 / halfDuration));
			yield return new WaitForEndOfFrame();
		}
		image.canvasRenderer.SetAlpha(0f);
		yield return new WaitForEndOfFrame();
		UnityEngine.Object.Destroy(m_canvas);
	}
}
