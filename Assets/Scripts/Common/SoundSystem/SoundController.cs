namespace ML.Sound
{
	using UnityEngine;
	using UnityEngine.Audio;

    public class SoundController : MonoBehaviour
    {
        [SerializeField] AudioSource audioSrc;
        int finishFrameCount = 0;
	
	    public void SetAudioMixerGroup( AudioMixerGroup mixerGroup) 
	    {
	    	audioSrc.outputAudioMixerGroup = mixerGroup;
	    }
	
        public bool IsFinished()
        {
            if (!audioSrc.isPlaying)
            {
                finishFrameCount++;
                if (finishFrameCount > 1)
                {
                    audioSrc.clip = null;
                    return true;
                }
            }

            return false;
        }

	    public void Play3D(AudioClip audio, float volume = 1f, float pitch = 1f, SoundType type = SoundType.Default )
        {
            audioSrc.volume = volume;
            audioSrc.pitch = pitch;
            audioSrc.spatialBlend = 0.5f;

            audioSrc.PlayOneShot(audio);
            finishFrameCount = 0;
        }

	    public void Play2D(AudioClip audio, float volume = 1f, float pitch = 1f, SoundType type = SoundType.Default)
        {
            audioSrc.volume = volume;
            audioSrc.pitch = pitch;
            audioSrc.spatialBlend = 0f;

            audioSrc.PlayOneShot(audio);
            finishFrameCount = 0;
        }
    }
}
