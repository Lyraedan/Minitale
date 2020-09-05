using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationPlayer : MonoBehaviour
{

    public float delay = 5f;
    public bool randomiseStartingIndex = false;
    [Tooltip(@"Current/Starting index")] public int currentIndex = 0;
    public Texture[] frames;
    private float playbackTimer = 0;
    private Renderer texture;
    [HideInInspector] public bool isPlayingAnim = true;
    private bool init = false;

    public void Init()
    {
        texture = GetComponent<Renderer>();
        if (randomiseStartingIndex) currentIndex = Random.Range(0, frames.Length);
        if (currentIndex > frames.Length && !randomiseStartingIndex) currentIndex = 0;
        else if (currentIndex > frames.Length && randomiseStartingIndex) currentIndex = Random.Range(0, frames.Length);
        UpdateTexture();
        init = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(init)
            PlayAnimation();
    }

    public void PlayAnimation()
    {
        if (isPlayingAnim)
        {
            playbackTimer += 1 * Time.deltaTime;
            if (playbackTimer >= delay)
            {
                currentIndex++;
                currentIndex %= frames.Length;
                UpdateTexture();
                playbackTimer = 0;
            }
        }
    }

    private void UpdateTexture()
    {
        texture.material.mainTexture = frames[currentIndex];
    }

    public Texture GetFrame(int frame)
    {
        return frames[frame];
    }

    public Texture GetTexture()
    {
        return texture.material.mainTexture;
    }
}
