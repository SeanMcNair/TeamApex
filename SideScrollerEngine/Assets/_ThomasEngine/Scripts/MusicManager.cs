﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{

    [SerializeField]
    private bool dontDestroyOnLoad;

    [Header("Music")]
    [SerializeField]
    private AudioSource musicSource;
    [SerializeField]
    private AudioClip BGMusic;
    [SerializeField]
    private AudioClip gameOverAmbience;

    [Header("Stings")]
    [SerializeField]
    private AudioSource stingSource;
    [SerializeField]
    private AudioClip gameOverLose;
    [SerializeField]
    private AudioClip gameOverWin;

    void Start()
    {
        if (dontDestroyOnLoad)
        {
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void PlayBackgroundMusic()
    {
        StartCoroutine(FadeIn(BGMusic));
    }

    public void PlayGameOverLoseMusic()
    {
        Debug.Log("playing.....");
        stingSource.clip = gameOverLose;
        stingSource.Play();
        StartCoroutine(FadeOut(gameOverAmbience));
    }

    public void PlayGameOverWinMusic()
    {
        stingSource.clip = gameOverWin;
        stingSource.Play();
        StartCoroutine(FadeOut(gameOverAmbience));
    }

    public void SetPitch(float _pitch)
    {
        musicSource.pitch = _pitch;
        stingSource.pitch = _pitch;
    }

    IEnumerator FadeOut(AudioClip _nextClip)
    {
        //fade out current clip
        float t = 1;
        while (t > 0.0f)
        {
            t -= Time.deltaTime;
            musicSource.volume = t;
            yield return new WaitForEndOfFrame();
        }
        musicSource.volume = 0.0f;

        //start fade in of next clip
        StartCoroutine(FadeIn(_nextClip));
    }

    IEnumerator FadeIn(AudioClip _clip)
    {
        musicSource.volume = 0.0f;
        musicSource.clip = _clip;
        musicSource.Play();
        float t = 0.0f;
        while (t < 1)
        {
            t += Time.deltaTime;
            musicSource.volume = t;
            yield return new WaitForEndOfFrame();
        }
    }

}
