using System.Collections.Generic;
using UnityEngine;

public class RhythmManager : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource music;

    [Header("UI")]
    public RectTransform ring;

    [Header("Timing")]
    public float appearOffset = 1.3f;     // 提前出现时间
    public float perfectWindow = 0.12f;   // PERFECT 判定
    public float badWindow = 0.22f;       // BAD 判定
    private float speedMultiplier = 1f;       // 速度添加倍率

    [Header("Ring Size")]
    public float startSize = 300f;
    public float targetSize = 80f;

    // ===== 内部状态 =====
    List<float> beats = new List<float>();
    int index = 0;

    float currentBeat = -1f;
    bool active = false;

    void Start()
    {
        LoadCSV();

        if (music == null || ring == null)
        {
            Debug.LogError("Music 或 Ring 没有绑定！");
            return;
        }

        music.Play();
        ring.gameObject.SetActive(false);
    }

    void Update()
    {
        appearOffset *= speedMultiplier;

        float songTime = music.time;

        // 1️⃣ 生成新的节奏点
        if (!active && index < beats.Count &&
            songTime >= beats[index] - appearOffset)
        {
            SpawnRing(beats[index]);
            index++;
        }

        if (!active) return;

        // 2️⃣ 圆环缩放
        UpdateRing(songTime);

        // 3️⃣ 按键判定
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Judge(songTime);
            return;
        }

        // 4️⃣ 漏按 MISS
        if (songTime > currentBeat + badWindow)
        {
            Miss_NoInput();
        }
    }

    // ===== 核心逻辑 =====

    void SpawnRing(float beatTime)
    {
        currentBeat = beatTime;
        active = true;

        ring.sizeDelta = Vector2.one * startSize;
        ring.gameObject.SetActive(true);
    }

    void UpdateRing(float songTime)
    {
        float t = Mathf.Clamp01(
            1f - (currentBeat - songTime) / appearOffset
        );

        float size = Mathf.Lerp(startSize, targetSize, t);
        ring.sizeDelta = Vector2.one * size;
    }

    void Judge(float songTime)
    {
        float diff = Mathf.Abs(songTime - currentBeat);

        if (diff <= perfectWindow)
        {
            Debug.Log("PERFECT");
        }
        else if (diff <= badWindow)
        {
            Debug.Log("BAD");
        }
        else
        {
            Debug.Log("MISS (Timing)");
        }

        EndBeat();
    }

    void Miss_NoInput()
    {
        Debug.Log("MISS (No Input)");
        EndBeat();
    }

    void EndBeat()
    {
        active = false;
        ring.gameObject.SetActive(false);
    }

    // ===== CSV =====

    void LoadCSV()
    {
        TextAsset csv = Resources.Load<TextAsset>("beat");

        if (csv == null)
        {
            Debug.LogError("Resources/beat.csv 没找到！");
            return;
        }

        string[] lines = csv.text.Split('\n');

        for (int i = 0; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            if (float.TryParse(lines[i], out float time))
                beats.Add(time);
        }
    }

    //用于控制音乐速度
    public void SetSpeedMultiplier(float value)
    {
        speedMultiplier = value;
    }
}