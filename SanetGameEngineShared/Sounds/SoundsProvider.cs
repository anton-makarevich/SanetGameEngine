using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace Sanet.XNAEngine.Sounds
{
    public static class SoundsProvider
    {
        #region Events
        public static event Action SoundsFinished;
        #endregion

        #region Fields
        static List<string> _soundFiles = new List<string>();
        static List<string> _songFiles = new List<string>();
        static Dictionary<string, SoundEffect> _soundEffects = new Dictionary<string, SoundEffect>();

        static Dictionary<string, Song> _songs = new Dictionary<string, Song>();

        static Dictionary<string, SoundEffectInstance> _playingSounds = new Dictionary<string, SoundEffectInstance>();

        static Song _backgroundSong;

        static float _songVolume = 0.9f;

        static Queue<SoundEffectInstance> _sheduledSounds;

        static bool _IsSoundEnabled = true;
        static bool _IsMuted = false;
        static bool _IsMusicEnabled = true;
        #endregion

        #region Properties
        public static bool IsSoundEnabled
        {
            get
            {
                return _IsSoundEnabled;
            }
            set
            {
                _IsSoundEnabled = value;
                if (!IsSoundEnabled)
                    StopAllSounds();
            }
        }
        
        public static bool IsMuted
        {
            get
            { return _IsMuted; }
            set
            {
                _IsMuted = value;
                if (value)
                {
                    PauseSong();
                    StopAllSounds();
                }
                else
                {//this should resume music
                    if (_IsMusicEnabled)
                        IsMusicEnabled = true;
                }
            }
        }

        public static bool IsLoaded { get; set; }

        public static float SongVolume
        {
            get
            {
                return _songVolume
#if WP8
                    *2.5f
#endif
;
            }
            set
            {
                if (Microsoft.Xna.Framework.Media.MediaPlayer.GameHasControl)
                {
                    Microsoft.Xna.Framework.Media.MediaPlayer.Volume = value;
                }
                _songVolume = value;
            }
        }
        
        public static bool IsMusicEnabled
        {
            get
            {
                return _IsMusicEnabled;
            }
            set
            {
                _IsMusicEnabled = value;
                if (value && !_IsMuted && _backgroundSong != null)
                {
                    if (Microsoft.Xna.Framework.Media.MediaPlayer.GameHasControl)
                    {
                        Microsoft.Xna.Framework.Media.MediaPlayer.Stop();
                        Microsoft.Xna.Framework.Media.MediaPlayer.Volume = SongVolume;
                        try
                        {
                            Microsoft.Xna.Framework.Media.MediaPlayer.Play(_backgroundSong);
                        }
                        catch (Exception)
                        {

                        }

                        Microsoft.Xna.Framework.Media.MediaPlayer.IsRepeating = true;
                    }
                }
                else
                {
                    if (Microsoft.Xna.Framework.Media.MediaPlayer.GameHasControl)
                    {
                        Microsoft.Xna.Framework.Media.MediaPlayer.Stop();
                    }

                }
            }
        }

        public static bool IsSoundLoaded(string sound)
        {
            return _soundEffects.ContainsKey(sound.ToLower());
        }
        #endregion

        #region  Methods
        public static bool IsPlaying(string sound)
        {
            return _playingSounds.ContainsKey(sound) && _playingSounds[sound].State == SoundState.Playing;
        }

        /// <summary>
        /// play sound by its file name
        /// </summary>
        public static void PlaySound(string filename)
        {
            PlaySound(filename, false, false, 1.0f);
        }

        public static void PlaySound(string filename, bool asInstance, bool isLooped)
        {
            PlaySound(filename, asInstance, isLooped, 1.0f);
        }

        /// <summary>
        /// play sound by its file name as instance
        /// works only with soundinstances
        /// </summary>
        public static void PlaySound(string file, bool asInstance, bool isLooped, float volume)
        {
            if (!IsSoundEnabled || IsMuted || file==null)
                return;
            file = file.ToLower();
            if (asInstance)
            {
                
                if (!_playingSounds.ContainsKey(file))
                {
                    var seinstance = _soundEffects[file].CreateInstance();
                    _playingSounds.Add(file, seinstance);
                    seinstance.IsLooped = isLooped;
                    seinstance.Volume = volume;
                    seinstance.Play();
                }
                
                
            }
            else
            {
                try
                {


                    if (_soundEffects.ContainsKey(file))
                    {
                        var effect = _soundEffects[file];
                        if (effect != null)
                            effect.Play();
                       
                    }

                }
                catch (Exception ex)
                {
                    Debug.WriteLine( ex.Message);
                }
            }
        }

        /// <summary>
        /// Stops sound (if it's playing)
        /// </summary>
        /// <param name="file"></param>
        public static void StopSound(string file)
        {
            if (_playingSounds.ContainsKey(file))
            {
                _playingSounds[file].Stop();
                _playingSounds.Remove(file);
            }
        }

        /// <summary>
        /// Attempt to play sounds one by one
        /// </summary>
        /// <param name="files"></param>
        public static void PlaySounds(List<string> files)
        {
            PlaySound(files[0]);
            if (files.Count > 0)
            {
                _sheduledSounds = new Queue<SoundEffectInstance>();
                foreach (var file in files)
                {
                    if (_soundEffects.ContainsKey(file))
                    {
                        var seinstance = _soundEffects[file].CreateInstance();
                        if (!_playingSounds.ContainsKey(file))
                            _playingSounds.Add(file, seinstance);
                        else
                            _playingSounds[file] = seinstance;
                        _sheduledSounds.Enqueue(seinstance);
                    }
                }
            }
        }

        public static void PlaySong(string songName)
        {

            if (IsMuted)
                return;
            if (_songs.ContainsKey(songName))
            {
                var playingSong = _songs.FirstOrDefault(x => x.Value == _backgroundSong).Key;
                if (playingSong == songName)
                    return;
                _backgroundSong = _songs[songName];
                IsMusicEnabled = _IsMusicEnabled;
            }
        }

        public static void ResumeSong()
        {
            if (IsMuted)
                return;
            if (Microsoft.Xna.Framework.Media.MediaPlayer.GameHasControl)
            {
                Microsoft.Xna.Framework.Media.MediaPlayer.Resume();
            }
        }
        public static void PauseSong()
        {
            if (Microsoft.Xna.Framework.Media.MediaPlayer.GameHasControl)
            {
                Microsoft.Xna.Framework.Media.MediaPlayer.Pause();
            }
        }


        public static void AddSound(string soundName)
        {
            if (!_soundFiles.Contains(soundName))
                _soundFiles.Add(soundName);
        }

        public static void AddSong(string soundName)
        {
            _songFiles.Add(soundName);
        }

        public static void LoadContent(ContentManager contentManager)
        {

            foreach (var sound in _soundFiles)
                _soundEffects.Add(sound.ToLower(), LoadSound(contentManager, sound));


            foreach (var sound in _songFiles)
                _songs.Add(sound, LoadSong(contentManager, sound));

        }

        static SoundEffect LoadSound(ContentManager contentManager, string sound)
        {
            return contentManager.Load<SoundEffect>("Sounds/" + sound);
        }

        public static void AddSoundAndLoad(ContentManager contentManager, string sound)
        {
            AddSound(sound);
            _soundEffects.Add(sound.ToLower(), LoadSound(contentManager, sound));
            
        }

        static Song LoadSong(ContentManager contentManager, string song)
        {
            return contentManager.Load<Song>("Songs/" + song.ToLower());
        }

        public static void LoadStep(ContentManager contentManager)
        {
            if (_soundFiles.Any())
            {
                var sound = _soundFiles.First();
                _soundEffects.Add(sound.ToLower(), LoadSound(contentManager, sound));
                _soundFiles.Remove(sound);
            }
            else
            {
                if (_songFiles.Any())
                {
                    var sound = _songFiles.First();
                    _songs.Add(sound, LoadSong(contentManager, sound));
                    _songFiles.Remove(sound);
                }
                else
                    IsLoaded = true;
            }
        }

        public static void StopAllSounds()
        {
            foreach (var sound in _playingSounds.Values)
            {
                //if (sound.State == SoundState.Playing)
                sound.Stop();
            }
            _sheduledSounds = null;
        }

        public static void Update()
        {
            if (_sheduledSounds != null && _sheduledSounds.Count > 0)
            {
                if (_sheduledSounds.Peek().State == SoundState.Stopped)
                {
                    if (_sheduledSounds.Count > 1)
                    {
                        _sheduledSounds.Dequeue();
                        _sheduledSounds.Peek().Play();

                    }
                    else
                    {
                        _sheduledSounds = null;
                        if (SoundsFinished != null)
                            SoundsFinished();
                    }
                }
            }
        }
        #endregion
    }
}
