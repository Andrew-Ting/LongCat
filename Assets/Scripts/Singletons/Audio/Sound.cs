using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;

    public SoundType type;
    public bool loop;

    [HideInInspector]
    public AudioSource source;
    [HideInInspector]
    public float volume;

    public override string ToString()
    {
        return name;
    }
}

public enum SoundType
{
    SFX,
    BGM
}
