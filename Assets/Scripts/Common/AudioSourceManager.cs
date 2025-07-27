using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourceManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource m_audioSourceBgm = null;

    [SerializeField]
    private AudioSource m_audioSourceSe = null;

    [SerializeField]
    private List<AudioClip> m_soundBgmList = new List<AudioClip>();

    [SerializeField]
    private List<AudioClip> m_soundSeList = new List<AudioClip>();

    public enum BGM_NUM : int
    {
        BATTLE_1 = 0,
        HOME_1,
        DECK_1,
    }

    private static AudioSourceManager instance = null;
    public static AudioSourceManager Instance()
    {
        return instance;
    }

    public void OnEnable()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void PlayOneShot(int index, bool isBgm = false)
    {
        OptionData optionData = new OptionData();
        optionData.IsFileExists();
        optionData.LoadTxt();

        if (isBgm)
        {
            m_audioSourceBgm.Stop();
            m_audioSourceBgm.clip = m_soundBgmList[index];
            switch (index)
            {
                case 0:
                    m_audioSourceSe.volume = optionData.battleBgmVolume;
                    break;
                case 1:
                    m_audioSourceSe.volume = optionData.homeBgmVolume;
                    break;
                case 2:
                    m_audioSourceSe.volume = optionData.deckBgmVolume;
                    break;
            }
            m_audioSourceBgm.Play();
            return;
        }
        m_audioSourceSe.volume = optionData.seVolume;
        m_audioSourceSe.PlayOneShot(m_soundSeList[index]);
    }
}
