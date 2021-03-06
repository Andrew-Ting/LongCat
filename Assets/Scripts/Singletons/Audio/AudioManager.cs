using System;
using UnityEngine;
using System.Collections;

/*usage
 * Play(string name)
 * Stop()
 * Mute()
 * Unmute()
 * TransitionBGM(string name, float timeChange)
 * PlayTemporary(string name)
*/

public class AudioManager : MonoBehaviour
{
    private const int parts = 30;
    public Sound[] sounds;

    static Sound currentBGM;

    private static AudioManager instance;

    void Awake()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.loop = s.loop;
            s.source.volume = PlayerPrefs.GetFloat(s.type.ToString(), 1f) * s.initialVolume;
            s.source.playOnAwake = false;
        }
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s != null)
        {
            if (s.type == SoundType.BGM) //bgm music
            {
                if (currentBGM != null)
                {
                    if(currentBGM.name == name)
                    {
                        Debug.Log("Already playing " + name);
                        return;
                    }
                }
                Stop();
                currentBGM = s;
            }
            s.source.volume = PlayerPrefs.GetFloat(s.type.ToString(), 1f) * s.initialVolume;
            s.source.Play();
            Debug.Log("now playing: " + s.name);
        }
        else
        {
            Debug.LogWarning("error in audio. tried playing " + name);
        }
    }

    public void Stop()
    {
        if (currentBGM != null)
        {
            currentBGM.source.Stop();
            currentBGM = null;
        }
    }

    public void Mute()
    {
        if (currentBGM != null)
        {
            currentBGM.source.mute = true;
        }
    }

    public void Unmute()
    {
        if (currentBGM != null)
        {
            currentBGM.source.mute = false;
        }
    }

    public void TransitionBGM(string name, float transitionTime)
    {
        StartCoroutine(transition(name, transitionTime));
    }

    public void PlayTemporary(string name) // play a sfx temporarily by itself
    {
        StartCoroutine(PlayTemp(name));
    }

    public void UpdateBGMVolume()
    {
        currentBGM.source.volume = PlayerPrefs.GetFloat("BGM", 1f) * currentBGM.initialVolume;
    }

    IEnumerator PlayTemp(string name) // only for SFX
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s != null)
        {
            if (currentBGM != null)
            {
                if (s.type == SoundType.SFX)
                {
                    StartCoroutine(slowPause(0.2f));
                    s.source.volume = PlayerPrefs.GetFloat("SFX", 1f) * s.initialVolume;
                    s.source.Play();
                    while (s.source.isPlaying)
                    {
                        yield return null;
                    }
                    StartCoroutine(slowPlay(0.3f));
                }
                else
                {
                    Debug.Log("tried playing a bgm. Not allowed");
                }
            }
            else
            {
                Debug.Log("no song is currently playing");
            }
        }
        else
        {
            Debug.Log("no song is found");
        }
        yield return null;
    }

    IEnumerator slowPlay(float timeMax)
    {
        if (currentBGM != null)
        {
            if (!currentBGM.source.isPlaying)
            {
                float t = 0;
                float partition = timeMax / parts;
                currentBGM.source.Play();
                while (t < timeMax)
                {
                    currentBGM.source.volume = t / timeMax * PlayerPrefs.GetFloat("BGM", 1f) * currentBGM.initialVolume;

                    t += partition;
                    yield return new WaitForSecondsRealtime(partition);
                }

            }
            else
            {
                Debug.Log("already playing");
            }
        }
        else
        {
            Debug.Log("nothing to slow play");
        }
        yield return null;
    }

    IEnumerator slowPause(float timeMax) // only for bgm music
    {
        if (currentBGM != null)
        {
            float t = 0;
            float partition = timeMax / parts;

            while (t < timeMax)
            {
                currentBGM.source.volume = (1 - t / timeMax) * PlayerPrefs.GetFloat("BGM", 1f) * currentBGM.initialVolume;

                t += partition;
                yield return new WaitForSecondsRealtime(partition);
            }
            currentBGM.source.Pause();
        }
        else
        {
            yield return null;
            Debug.Log("Tried pausing, but no bgm is playing");
        }
    }

    IEnumerator transition(string name, float transitionMax)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s != null) // song specified
        {
            float t = 0; //time
            float partition = transitionMax / parts;
            if (currentBGM != null) // not empty
            {
                //Debug.Log("currentBGM not null");
                if (currentBGM != s) // not same song transitioned into
                {

                    s.source.volume = 0;
                    //Debug.Log("currentBGM not same song");
                    s.source.Play();
                    while (t < transitionMax)
                    {
                        //Debug.Log("currentBGM " + t + "\ntransition" + s.volume + " \noldSong" + currentBGM.volume);
                        //get volume for each. for each partition, increase volume of sound s, decrease volume of current bgm

                        s.source.volume = t / transitionMax * PlayerPrefs.GetFloat("BGM", 1f) * s.initialVolume;
                        currentBGM.source.volume = (1 - t / transitionMax) * PlayerPrefs.GetFloat("BGM", 1f) * currentBGM.initialVolume;

                        t += partition;
                        yield return new WaitForSecondsRealtime(partition);
                    }
                    currentBGM.source.Stop();
                    currentBGM = s;
                }
                else //same song. no need for transition
                {
                    yield return null;
                }
            }
            else
            {
                s.source.volume = 0;
                s.source.Play();
                while (t < transitionMax)
                {
                    s.source.volume = t / transitionMax * PlayerPrefs.GetFloat("BGM", 1f) * s.initialVolume;
                    t += partition;
                    yield return new WaitForSecondsRealtime(partition);
                }
                currentBGM = s;
            }
        }
        else // song specified is null
        {
            Debug.Log("tried to transition to " + name);
            yield return null;
        }
    }
}
