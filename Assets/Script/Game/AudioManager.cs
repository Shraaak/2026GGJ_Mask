using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    //数据结构
    [System.Serializable]
    public class Sound
    {
        public string name;      // 声音名字（调用用）
        public AudioClip clip;   // 音频
        [Range(0f, 1f)]
        public float volume = 1f;
        public bool loop = false;

        [HideInInspector]
        public AudioSource source;
    }

    public Sound[] sounds;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // 为每个 Sound 创建一个 AudioSource
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.loop = s.loop;
            s.source.playOnAwake = false;
            s.source.spatialBlend = 0f; // 2D 音效
        }
    }

    // ======================
    // 对外接口
    // ======================

    /// <summary>
    /// 播放声音（循环音适合用这个）
    /// </summary>
    public void Play(string name)
    {
        Sound s = GetSound(name);
        if (s == null) return;

        s.source.Play();
    }

    /// <summary>
    /// 停止声音
    /// </summary>
    public void Stop(string name)
    {
        Sound s = GetSound(name);
        if (s == null) return;

        s.source.Stop();
    }

    /// <summary>
    /// 播放一次音效（最常用）
    /// </summary>
    public void PlayOneShot(string name)
    {
        Sound s = GetSound(name);
        if (s == null) return;

        s.source.PlayOneShot(s.clip);
    }

    /// <summary>
    /// 根据名字找到对应的 Sound
    /// </summary>
    public Sound GetSound(string name)
    {
        foreach (Sound s in sounds)
        {
            if (s.name == name)
                return s;
        }

        Debug.LogWarning("没找到声音：" + name);
        return null;
    }
}