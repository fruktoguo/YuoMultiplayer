using System;
using System.Collections.Generic;
using DG.Tweening;
using ET;
using UnityEngine;
using YuoTools.Main.Ecs;
using Object = UnityEngine.Object;

namespace YuoTools.Main.Ecs
{
    public class SoundManager : YuoComponentGet<SoundManager>
    {
        [Header("正在播放的AudioSource")] public List<SoundData> Sounds = new List<SoundData>();

        [Header("休眠的AudioSource")] public List<SoundData> SoundPools = new List<SoundData>();

        [Header("背景音效")] public SoundData Bg;

        public class SoundData
        {
            public string name = "null";
            public float SoundVolume = 1;
            public AudioSource source;
            public bool Stop = false;
        }

        [Header("最大缓存Source数量")] public int MaxSourceNum = 8;
        [SerializeField] private float bgValue = 1;
        [SerializeField] private float soundValue = 1;

        public void SetBgVolume(float f)
        {
            f.Clamp(1);
            bgValue = f;
            Bg.source.volume = f * Bg.SoundVolume;
        }

        public void SetSoundVolume(float f)
        {
            f.Clamp(1);
            soundValue = f;
            foreach (var item in Sounds)
            {
                item.source.volume = f * item.SoundVolume;
            }
        }

        public async void PlaySound(AudioClip clip, float value = 1)
        {
            if (clip == null) return;
            SoundData asTemp;
            if (SoundPools.Count > 0)
            {
                asTemp = SoundPools[0];
                SoundPools.Remove(asTemp);
            }
            else
            {
                asTemp = new SoundData() { source = gameObject.AddComponent<AudioSource>() };
            }

            asTemp.source.Stop();
            asTemp.source.clip = clip;
            asTemp.source.loop = false;
            asTemp.source.playOnAwake = false;
            asTemp.source.volume = soundValue * value;
            asTemp.SoundVolume = value;
            asTemp.source.Play();
            Sounds.Add(asTemp);

            asTemp.Stop = false;
            var timer = clip.length;
            while (timer > 0 && !asTemp.Stop)
            {
                timer -= Time.deltaTime;
                await YuoWait.WaitFrameAsync();
            }
            asTemp.Stop = true;
            asTemp.source.Stop();
            Sounds.Remove(asTemp);
            SoundPools.Add(asTemp);
        }

        public void StopAllSound()
        {
            for (int i = 0; i < Sounds.Count;)
            {
                Sounds[0].source.Stop();
                Sounds[0].Stop = true;
                SoundPools.Add(Sounds[0]);
                Sounds.RemoveAt(0);
            }

            MaxSourceNum.Clamp();
            if (SoundPools.Count > MaxSourceNum)
            {
                for (int i = MaxSourceNum; i < SoundPools.Count;)
                {
                    Object.Destroy(SoundPools[i].source);
                    SoundPools.RemoveAt(MaxSourceNum);
                }
            }
        }

        public void PlayBg(AudioClip clip, float value = 1, bool loop = true)
        {
            if (Bg.source.clip != clip)
            {
                Bg.source.clip = clip;
                Bg.source.Play();
            }

            Bg.source.loop = loop;
            Bg.SoundVolume = value;
            Bg.source.volume = bgValue * value;
        }

        public void PauseBg()
        {
            Bg.source.Pause();
        }

        public void StopBg()
        {
            Bg.source.Stop();
            Bg.source.clip = null;
        }

        public void ResumeBg()
        {
            Bg.source.Play();
        }

        public void PauseSound()
        {
            foreach (var item in Sounds)
            {
                item.source.Pause();
            }
        }

        public void ResumeSound()
        {
            foreach (var item in Sounds)
            {
                item.source.Play();
            }
        }

        [NonSerialized] public GameObject gameObject;
    }

    public class SoundManagerSystem : YuoSystem<SoundManager>, IAwake
    {
        public override string Group => "Main/Sound";

        protected override void Run(SoundManager component)
        {
            component.gameObject = new GameObject("SoundManager");
            Object.DontDestroyOnLoad(component.gameObject);
            component.Bg = new SoundManager.SoundData() { source = component.gameObject.AddComponent<AudioSource>() };
            component.Bg.source.playOnAwake = false;
        }
    }
}