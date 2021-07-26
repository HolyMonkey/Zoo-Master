using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GlobalVolume : MonoBehaviour
{
    [SerializeField] private SoundType _type;
    [SerializeField] [Range(_min, _max)] private float _volumeValue;

    private const float _min = 0f;
    private const float _max = 1f;

    private bool _muted;

    public SoundType Type => _type;

    private float _volume
    {
        get
        {
            if (_muted)
                return 0;
            else
                return _volumeValue;
        }
    }
    public float Volume
    {
        set
        {
            if (value > _max || value < _min)
            {
                throw new System.Exception("Volume value out of range");
            }
            _volumeValue = value;
            VolumeChanged?.Invoke(_volume);
        }
    }

    public event UnityAction<float> VolumeChanged;
    public event UnityAction<bool> Muted;

    public enum SoundType
    {
        Music,
        InGameSounds
    }

    private void OnValidate()
    {
        VolumeChanged?.Invoke(_volume);
    }

    public static bool TryFind(SoundType type, out GlobalVolume foundVolume)
    {
        var volumes = FindObjectsOfType<GlobalVolume>();
        foundVolume = null;
        foreach (var volume in volumes)
        {
            if (volume.Type == type)
            {
                if(foundVolume == null)
                {
                    foundVolume = volume;
                }
                else
                {
                    throw new System.Exception("Multiple copies of GlobalVolume of this type found");
                }
            }
        }
        return foundVolume != null;
    }

    public void SwitchMute()
    {
        SetMute(!_muted);
    }

    private void SetMute(bool mute)
    {
        _muted = mute;
        VolumeChanged?.Invoke(_volume);
        Muted?.Invoke(_muted);
    }
}
