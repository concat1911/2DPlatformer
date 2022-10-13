namespace ML.Sound
{
    using UnityEngine;
	using UnityEngine.Pool;
	using UnityEngine.Audio;
    using System.Collections.Generic;

    public class SoundManager : MonoBehaviour
    {
        private static SoundManager instance;

        [SerializeField] bool muted = false;
	    [SerializeField] GameObject controllerPrefab;
	    [SerializeField] AudioClip curBGMusic;
	    
	    [Header("AudioMixers")]
	    [SerializeField] private AudioMixerGroup masterMixer;
	    [SerializeField] private AudioMixerGroup musicMixer;
	    [SerializeField] private AudioMixerGroup vfxMixer;
	    [SerializeField] private AudioMixerGroup defaultMixer;
	    
	    public AudioMixerGroup MasterMixer => masterMixer;
	    public AudioMixerGroup MusicMixer => musicMixer;
	    public AudioMixerGroup VFXMixer => vfxMixer;
	    public AudioMixerGroup DefaultMixer => defaultMixer;
	    
        public static bool Muted => instance.muted;

        [SerializeField] List<SoundController> activeSound = new List<SoundController>();

        IObjectPool<SoundController> soundPool;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;

	        soundPool = new ObjectPool<SoundController>(CreateSoundController, OnTakeSoundController, OnReleaseSoundController);
            
	        DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            for(int i = 0; i < activeSound.Count; i++)
            {
                if( activeSound[i].IsFinished() )
                {
                    soundPool.Release(activeSound[i]);
                    activeSound.RemoveAt(i);
                    i--;
                }
            }
        }

        SoundController CreateSoundController()
        {
            GameObject controllerObj = Instantiate(controllerPrefab, transform);
            SoundController controller = controllerObj.GetComponent<SoundController>();

            return controller;
        }

        void OnTakeSoundController(SoundController soundController)
        {
            soundController.gameObject.SetActive(true);
        }

        void OnReleaseSoundController(SoundController soundController)
        {
            soundController.gameObject.SetActive(false);
        }

	    public static bool Play3D(AudioClip clip, Vector3 pos, float volume = 1f, float pitch = 1f, SoundType soundType = SoundType.Default)
        {
            if (instance.muted) return false;
            if (clip == null) return false;

            SoundController controller = instance.soundPool.Get();
            controller.transform.position = pos;
	        controller.Play3D(clip, volume, pitch, soundType);

            instance.activeSound.Add(controller);

            return true;
        }

        public static bool Play2D(AudioClip clip, float volume = 1f, float pitch = 1f, SoundType soundType = SoundType.Default)
        {
            if (instance.muted) return false;
            if( clip == null ) return false;

	        SoundController controller = instance.soundPool.Get();
            
	        controller.SetAudioMixerGroup(instance.GetAudioMixer(soundType));
	        controller.Play2D(clip, volume, pitch, soundType);

            instance.activeSound.Add(controller);

            return true;
        }

        public static SoundController GetSoundController()
        {
            return instance.soundPool.Get();
        }

        public static void MuteSound(bool isMute)
        {
            instance.muted = isMute;
        }
        
	    AudioMixerGroup GetAudioMixer(SoundType soundType)
	    {
	    	switch(soundType){
	    		
	    	default:
	    	case SoundType.Default:
		    	return defaultMixer;
	    	case SoundType.VFX:
		    	return vfxMixer;
	    	case	SoundType.BGMusic:
		    	return musicMixer;
		    
	    	}
	    }
    }
}
