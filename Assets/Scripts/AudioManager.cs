using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	// Audio players components.
	public AudioSource effectsSource;
	public AudioSource musicSource;

	// Singleton instance.
	public static AudioManager Instance = null;

	public Sound[] sounds;
	public Sound[] music;
	AudioClip currentClip;

	
	// Initialize the singleton instance.
	private void Awake()
	{
		ChangeBGMVolume(PlayerPrefs.GetInt("bgmVol"));
		ChangeSFXVolume(PlayerPrefs.GetInt("sfxVol"));
		// If there is not already an instance of SoundManager, set it to this.
		if (Instance == null)
		{
			Instance = this;
			//Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
			DontDestroyOnLoad(gameObject);
		}
		//If an instance already exists, destroy whatever this object is to enforce the singleton.
		else if (Instance != this)
		{
			Destroy(gameObject);
		}
	}
	

	// Will only play one identical sound at a time
	public void Play(string name)
	{
		AudioClip clip = null;
		for (int i = 0; i < sounds.Length; i++)
        {
			if(name.Equals(sounds[i].name))
            {
				clip = sounds[i].clip;

				if(currentClip != clip)
                {
					currentClip = clip;
					effectsSource.PlayOneShot(currentClip);
				}
                else
                {
					effectsSource.Stop();
					effectsSource.PlayOneShot(currentClip);
				}

				break;
			}
        }
		if(clip == null)
        {
			Debug.LogError("Sound named '" + name + "' not found! (Check for typos?)");
        }
	}

	// Will overlap if played with other sounds
	public void PlayOneShot(string name)
    {
		AudioClip clip = null;
		for (int i = 0; i < sounds.Length; i++)
		{
			if (name.Equals(sounds[i].name))
			{
				clip = sounds[i].clip;
				effectsSource.PlayOneShot(clip);

				break;
			}
		}
		if (clip == null)
		{
			Debug.LogError("Sound named '" + name + "' not found! (Check for typos?)");
		}
	}

	// Play a single clip through the music source.
	public void PlayMusic(string name)
	{
		AudioClip clip = null;
		for (int i = 0; i < music.Length; i++)
		{
			if (name.Equals(music[i].name))
			{
                if (!musicSource.isPlaying)
                {
					clip = music[i].clip;
					musicSource.Stop();
					musicSource.clip = clip;
					musicSource.Play();
					break;
				}
			}
		}
		if (clip == null)
		{
			Debug.LogError("BGM named '" + name + "' not found! (Check for typos?)");
		}
	}

	public void ChangeMasterVolume(float value)
    {
		AudioListener.volume = value/100;
    }

	public void ChangeBGMVolume(float value)
	{
		if(musicSource != null)
        {
			musicSource.volume = value / 75;
		}
	}

	public void ChangeSFXVolume(float value)
	{
		if(effectsSource != null)
        {
			effectsSource.volume = value / 30;
		}
	}

	//Load variables on entering scene
	private void OnEnable()
	{
		
	}
}


[System.Serializable]
public class Sound
{
	public string name;
	public AudioClip clip;
}