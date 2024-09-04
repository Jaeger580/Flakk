using UnityEngine;

public enum VoiceOverLines
{
    VO_INTRO,
    VO_MANUAL,
    VO_EXIT_CONSOLE,
    VO_DECK_DESC,
    VO_TERMINAL_DESC,
    VO_CONTRACT_DESC,
    VO_CONTRACT_START,
    VO_LOSS,
    VO_WIN,
    VO_NEXT_WAVE,
    VO_WEAKPOINT_DEATH
}

public class VoiceOverPlayer : MonoBehaviour
{
    static public VoiceOverPlayer instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void PlayLine(VoiceOverLines line)
    {//Fastest solution I could think of that still uses the audio manager. Otherwise I could just attach a bunch of audio sources lol
        //switch (line)
        //{
        //    case VoiceOverLines.VO_INTRO:
        //        AudioManager.instance.Play(MagicStrings.VO_INTRO, AudioManager.instance.VOSounds);
        //        break;
        //    case VoiceOverLines.VO_MANUAL:
        //        AudioManager.instance.Play(MagicStrings.VO_MANUAL, AudioManager.instance.VOSounds);
        //        break;
        //    case VoiceOverLines.VO_EXIT_CONSOLE:
        //        AudioManager.instance.Play(MagicStrings.VO_EXIT_CONSOLE, AudioManager.instance.VOSounds);
        //        break;
        //    case VoiceOverLines.VO_DECK_DESC:
        //        AudioManager.instance.Play(MagicStrings.VO_DECK_DESC, AudioManager.instance.VOSounds);
        //        break;
        //    case VoiceOverLines.VO_TERMINAL_DESC:
        //        AudioManager.instance.Play(MagicStrings.VO_TERMINAL_DESC, AudioManager.instance.VOSounds);
        //        break;
        //    case VoiceOverLines.VO_CONTRACT_DESC:
        //        AudioManager.instance.Play(MagicStrings.VO_CONTRACT_DESC, AudioManager.instance.VOSounds);
        //        break;
        //    case VoiceOverLines.VO_CONTRACT_START:
        //        AudioManager.instance.Play(MagicStrings.VO_CONTRACT_START, AudioManager.instance.VOSounds);
        //        break;
        //    case VoiceOverLines.VO_LOSS:
        //        AudioManager.instance.Play(MagicStrings.VO_LOSS, AudioManager.instance.VOSounds);
        //        break;
        //    case VoiceOverLines.VO_WIN:
        //        AudioManager.instance.Play(MagicStrings.VO_WIN, AudioManager.instance.VOSounds);
        //        break;
        //    case VoiceOverLines.VO_NEXT_WAVE:
        //        AudioManager.instance.Play(MagicStrings.VO_NEXT_WAVE, AudioManager.instance.VOSounds);
        //        break;
        //    case VoiceOverLines.VO_WEAKPOINT_DEATH:
        //        AudioManager.instance.Play(MagicStrings.VO_WEAKPOINT_DEATH, AudioManager.instance.VOSounds);
        //        break;
        //    default:
        //        break;
        //}
    }

    public void OverridePlayLine(VoiceOverLines line)
    {//Fastest solution I could think of that still uses the audio manager. Otherwise I could just attach a bunch of audio sources lol
        //switch (line)
        //{
            //case VoiceOverLines.VO_INTRO:
            //    AudioManager.instance.OverridePlay(MagicStrings.VO_INTRO, AudioManager.instance.VOSounds);
            //    break;
            //case VoiceOverLines.VO_MANUAL:
            //    AudioManager.instance.OverridePlay(MagicStrings.VO_MANUAL, AudioManager.instance.VOSounds);
            //    break;
            //case VoiceOverLines.VO_EXIT_CONSOLE:
            //    AudioManager.instance.OverridePlay(MagicStrings.VO_EXIT_CONSOLE, AudioManager.instance.VOSounds);
            //    break;
            //case VoiceOverLines.VO_DECK_DESC:
            //    AudioManager.instance.OverridePlay(MagicStrings.VO_DECK_DESC, AudioManager.instance.VOSounds);
            //    break;
            //case VoiceOverLines.VO_TERMINAL_DESC:
            //    AudioManager.instance.OverridePlay(MagicStrings.VO_TERMINAL_DESC, AudioManager.instance.VOSounds);
            //    break;
            //case VoiceOverLines.VO_CONTRACT_DESC:
            //    AudioManager.instance.OverridePlay(MagicStrings.VO_CONTRACT_DESC, AudioManager.instance.VOSounds);
            //    break;
            //case VoiceOverLines.VO_CONTRACT_START:
            //    AudioManager.instance.OverridePlay(MagicStrings.VO_CONTRACT_START, AudioManager.instance.VOSounds);
            //    break;
            //case VoiceOverLines.VO_LOSS:
            //    AudioManager.instance.OverridePlay(MagicStrings.VO_LOSS, AudioManager.instance.VOSounds);
            //    break;
            //case VoiceOverLines.VO_WIN:
            //    AudioManager.instance.OverridePlay(MagicStrings.VO_WIN, AudioManager.instance.VOSounds);
            //    break;
            //case VoiceOverLines.VO_NEXT_WAVE:
            //    AudioManager.instance.OverridePlay(MagicStrings.VO_NEXT_WAVE, AudioManager.instance.VOSounds);
            //    break;
            //case VoiceOverLines.VO_WEAKPOINT_DEATH:
            //    AudioManager.instance.OverridePlay(MagicStrings.VO_WEAKPOINT_DEATH, AudioManager.instance.VOSounds);
            //    break;
            //default:
            //    break;
        //}
    }
}
