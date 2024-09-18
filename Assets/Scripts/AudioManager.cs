using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField]
    private AudioMixer mixer;

    [Space, Header("2D"), SerializeField]
    private int total2DAS;
    [SerializeField]
    GameObject actions2dASObj;
    private AudioSource[] actions2dAS;

    [Space, Header("3D"), SerializeField]
    private int total3DAS;
    [SerializeField]
    GameObject actions3dASObj;
    private AudioSource[] actions3dAS;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
            return;
        }

        instance = this;

        actions2dAS = new AudioSource[total2DAS];
        actions3dAS = new AudioSource[total3DAS];

        AudioMixerGroup mixerGroup = mixer.FindMatchingGroups("SFX")[0];
        for (int i = 0; i < total2DAS; i++)
        {
            actions2dAS[i] = actions2dASObj.AddComponent<AudioSource>();
            actions2dAS[i].playOnAwake = false;
            actions2dAS[i].outputAudioMixerGroup = mixerGroup;
        }

        for (int i = 0; i < total3DAS; i++)
        {
            actions3dAS[i] = actions3dASObj.AddComponent<AudioSource>();
            actions3dAS[i].playOnAwake = false;
            actions3dAS[i].outputAudioMixerGroup = mixerGroup;
        }
    }

    public AudioSource GetUnused2dAS() 
    {
        foreach (AudioSource item in actions2dAS)
        {
            if (!item.isPlaying)
            {
                return item;
            }
        }
        return null;
    }

    public AudioSource GetUnused3dAS()
    {
        foreach (AudioSource item in actions3dAS)
        {
            if (!item.isPlaying)
            {
                return item;
            }
        }
        return null;
    }

    public void Play2dOneShotSound(AudioClip _clip, string mixerGroup, float _volume = 1, float _minPitch = 0.75f, float _maxPitch = 1.25f)
    {
        AudioSource _as = GetUnused2dAS();
        PlayOneShotSound(_as, _clip, mixerGroup, _minPitch, _maxPitch, _volume);
    }

    public void Play3dOneShotSound(AudioClip _clip, string mixerGroup, float _radius, Vector2 _pos, float _minPitch = 0.75f, float _maxPitch = 1.25f, float _volume = 1)
    {
        AudioSource _as = GetUnused3dAS();
        _as.minDistance = _radius;
        _as.maxDistance = _radius * 5;
        _as.gameObject.transform.position = new Vector3(_pos.x, _pos.y, -10);
        _as.spatialBlend = 1;
        PlayOneShotSound(_as, _clip, mixerGroup, _minPitch, _maxPitch, _volume);
    }

    private void PlayOneShotSound(AudioSource _as, AudioClip _clip, string mixerGroup, float _minPitch = 0.75f, float _maxPitch = 1.25f, float _volume = 1)
    {
        if (_as != null)
        {
            _as.outputAudioMixerGroup = mixer.FindMatchingGroups(mixerGroup)[0]; 
            _as.loop = false; 
            _as.pitch = Random.Range(_minPitch, _maxPitch); 
            _as.volume = _volume; 
            _as.PlayOneShot(_clip); 
        }
    }
    
    public void PlayOneShotRandomSound2d(AudioClip[] _clips, string mixerGroup, float _volume = 1, float _minPitch = 0.75f, float _maxPitch = 1.25f) 
    {
        Play2dOneShotSound(_clips[Random.Range(0, _clips.Length)], mixerGroup, _volume, _minPitch, _maxPitch);
    }
    public void PlayOneShotRandomSound3d(AudioClip[] _clips, string mixerGroup, float _radius, Vector2 _pos, float _minPitch = 0.75f, float _maxPitch = 1.25f, float _volume = 1)
    {
        Play3dOneShotSound(_clips[Random.Range(0, _clips.Length)], mixerGroup, _radius, _pos, _minPitch, _maxPitch, _volume);
    }
   
    public AudioSource Play2dLoop(AudioClip _clip, string mixerGroup, float _minPitch = 0.75f, float _maxPitch = 1.25f, float _volume = 0.7f) 
    {
        AudioSource _as = GetUnused2dAS();
        
        PlayLoopSound(_as, _clip, mixerGroup, _minPitch, _maxPitch, _volume);

        return _as;
    }
    public AudioSource Play3dLoop(AudioClip _clip, string mixerGroup, float _radius, Vector2 _pos, float _minPitch = 0.75f, float _maxPitch = 1.25f, float _volume = 0.4f) 
    {
        AudioSource _as = GetUnused3dAS();
        _as.minDistance = _radius;
        _as.maxDistance = _radius * 5;
        _as.gameObject.transform.position = new Vector3(_pos.x, _pos.y, -10);
        PlayLoopSound(_as, _clip, mixerGroup, _minPitch, _maxPitch, _volume);
        return _as;
    }


    public void PlayLoopSound(AudioSource _as, AudioClip _clip, string mixerGroup, float _minPitch = 0.75f, float _maxPitch = 1.25f, float _volume = 0.4f)
    {   
        if (_as != null)
        {
            _as.outputAudioMixerGroup = mixer.FindMatchingGroups(mixerGroup)[0];
            _as.loop = true;
            _as.pitch = Random.Range(_minPitch, _maxPitch);
            _as.volume = _volume;
            _as.clip = _clip;
            _as.Play();
        }
    }

    public void StopLoopSound(AudioSource _as) 
    {
        if (_as == null)
            return;

        _as.loop = false;
        _as.clip = null;
        _as.Stop();
    }

    public IEnumerator FadeOutSFXLoop(AudioSource source, float  fadeSpeed = 0.05f)
    {
        if(source != null && source.gameObject)
        {
            yield return new WaitUntil(() => (source.volume -= fadeSpeed) <= 0);
            StopLoopSound(source);
        }
        else
            yield return null;
    }
}
