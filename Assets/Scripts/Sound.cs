using UnityEngine;

public class Sound : MonoBehaviour
{
	public enum Button
	{
		Default
	}

	public enum Others
	{
		Shoot,
		Hit,
		Explosion,
		Match,
		FireBall,
		MoveBack,
		Pause,
		Win,
		Fail,
		ChangeColor
	}

	public AudioSource audioSource;

	public AudioSource loopAudioSource;

	public AudioClip[] buttonClips;

	public AudioClip[] otherClips;

	public static Sound instance;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		UpdateSetting();
	}

	public bool IsMuted()
	{
		return !IsEnabled();
	}

	public bool IsEnabled()
	{
		return CPlayerPrefs.GetBool("sound_enabled", defaultValue: true);
	}

	public void SetEnabled(bool enabled)
	{
		CPlayerPrefs.SetBool("sound_enabled", enabled);
		UpdateSetting();
	}

	public void Play(AudioClip clip)
	{
		audioSource.PlayOneShot(clip);
	}

	public void Play(AudioSource audioSource)
	{
		if (IsEnabled())
		{
			audioSource.Play();
		}
	}

	public void PlayButton(Button type = Button.Default)
	{
		audioSource.PlayOneShot(buttonClips[(int)type]);
	}

	public void Play(Others type, float volume = 0.5f)
	{
		audioSource.volume = volume;
		audioSource.PlayOneShot(otherClips[(int)type]);
	}

	public void PlayLooping(Others type, float volume = 1f)
	{
		loopAudioSource.volume = volume;
		loopAudioSource.PlayOneShot(otherClips[(int)type]);
	}

	public void StopLooping()
	{
		loopAudioSource.Stop();
	}

	public void UpdateSetting()
	{
		audioSource.mute = IsMuted();
		loopAudioSource.mute = IsMuted();
	}
}
