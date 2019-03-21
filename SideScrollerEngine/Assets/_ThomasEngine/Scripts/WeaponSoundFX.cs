using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSoundFX : MonoBehaviour 
{

    private enum PlayType { Sequence, Random }

    [SerializeField]
    private AudioSource weaponSource;
    [SerializeField]
    private AudioClip[] fireSounds;
    [SerializeField]
    private PlayType firePlayMode = PlayType.Random;
    private int curFireInd;
    

    public void PlayFireSound()
    {
        PlayVocalClipFromArray(fireSounds, firePlayMode, curFireInd, out curFireInd);     
    }

    void PlayVocalClipFromArray(AudioClip[] _clips, PlayType _playMode, int _ind, out int _curInd)
    {
        _curInd = 0;

        if (_playMode == PlayType.Random)
            weaponSource.clip = _clips[Random.Range(0, _clips.Length)];
        else
        {
            weaponSource.clip = _clips[_ind];
            _ind++;

            if (_ind > _clips.Length - 1)
                _ind = 0;

            _curInd = _ind;
        }

        weaponSource.Play();
    }


}

