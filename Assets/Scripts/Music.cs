using System.Collections;
using UnityEngine;

public class Music : MonoBehaviour
{
	public enum Type
	{
		None,
		Home,
		Main1,
		Main2,
		Main3
	}

	public AudioSource audioSource;

	public static Music instance;

	public AudioClip[] musicClips;

	private Type currentType;

	private void Awake()
	{
		instance = this;
	}

	public bool IsMuted()
	{
		return !IsEnabled();
	}

	public bool IsEnabled()
	{
		return CPlayerPrefs.GetBool("music_enabled", defaultValue: true);
	}

	public void SetEnabled(bool enabled, bool updateMusic = false)
	{
		CPlayerPrefs.SetBool("music_enabled", enabled);
		if (updateMusic)
		{
			UpdateSetting();
		}
	}

	public void Play(Type type)
	{
		if (type != 0 && (currentType != type || !audioSource.isPlaying))
		{
			StartCoroutine(PlayNewMusic(type));
		}
	}

	public void Play()
	{
		Play(currentType);
	}

	public void Stop()
	{
		audioSource.Stop();
	}

	private IEnumerator PlayNewMusic(Type type)
	{
		while (audioSource.volume >= 0.1f)
		{
			audioSource.volume -= 0.2f;
			yield return new WaitForSeconds(0.1f);
		}
		audioSource.Stop();
		if (IsEnabled())
		{
			currentType = type;
			audioSource.clip = musicClips[(int)type];
			audioSource.Play();
		}
		audioSource.volume = 1f;
	}

	private void UpdateSetting()
	{
		if (!(audioSource == null))
		{
			if (IsEnabled())
			{
				Play();
			}
			else
			{
				audioSource.Stop();
			}
		}
	}
}
