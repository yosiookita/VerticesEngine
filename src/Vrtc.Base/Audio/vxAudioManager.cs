#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using VerticesEngine.Audio.Exceptions;
using VerticesEngine.Utilities;


#endregion

namespace VerticesEngine.Audio
{


    /// <summary>
    /// Audio manager keeps track of what 3D sounds are playing, updating
    /// their settings as the camera and entities move around the world, and
    /// automatically disposing sound effect instances after they finish playing.
    /// </summary>
    public static class vxAudioManager //: Microsoft.Xna.Framework.GameComponent
    {
        #region -- Settings --
        //Audio
        [vxAudioSettingsAttribute("MainVolume")]
        public static float MainVolume = 0.5f;

        [vxAudioSettingsAttribute("SFxVolume")]
        public static float SoundEffectVolume = 0.5f;

        [vxAudioSettingsAttribute("MusicVolume")]
        public static float MusicVolume = 0.5f;

        #endregion

        #region Fields

        // The listener describes the ear which is hearing 3D sounds.
        // This is usually set to match the camera.
        public static AudioListener Listener
        {
            get { return listener; }
        }

        static AudioListener listener = new AudioListener();


        // The emitter describes an entity which is making a 3D sound.
        static AudioEmitter emitter = new AudioEmitter();

        // Keep track of all the 3D sounds that are currently playing.
        public static List<ActiveSound> active3DSounds = new List<ActiveSound>();

        static vxEngine Engine;


        #endregion


        internal static void Init(vxEngine engine)
        {
            Engine = engine;

            // Set the scale for 3D audio so it matches the scale of our game world.
            // DistanceScale controls how much sounds change volume as you move further away.
            // DopplerScale controls how much sounds change pitch as you move past them.
            SoundEffect.DistanceScale = 20000;
            SoundEffect.DopplerScale = 2.999951f;
            SoundEffect.MasterVolume = 1;
        }


        /// <summary>
        /// Unloads the sound effect data.
        /// </summary>
        public static void OnDispose()
        {
			//try
			//{
			Clear();
                    //foreach (SoundEffect soundEffect in soundEffects.Values)
                    //{
                    //    soundEffect.Dispose();
                    //}

                    //soundEffects.Clear();

            //}
            //finally
            //{
            //    base.Dispose(disposing);
            //}
        }

        public static void Clear()
        {
            try
            {
                foreach (ActiveSound ac in active3DSounds)
                {
                    ac.Instance.Stop();
                    ac.Instance.IsLooped = false;
                    ac.Instance.Dispose();
                    ac.Instance.Volume = 0;
                }
            }
            catch(Exception ex) { vxConsole.WriteException("vxAudioManager", ex); }
        }



        /// <summary>
        /// Music Song Collections
        /// </summary>
        public static class Music
        {
            public static List<vxSong> Songs = new List<vxSong>();

            //public static List<Song> Songs = new List<Song>();
            public static int CurrentSongIndex = 0;
        }

        #region Sound Effect Handling

        static SortedDictionary<string, SoundEffect> SoundEffects = new SortedDictionary<string, SoundEffect>();


        public static void LoadSoundEffect(ContentManager Content, object key, string Path)
        {
            SoundEffects.Add(key.ToString(), Content.Load<SoundEffect>(Path));
        }

        public static SoundEffect GetSound(object key)
        {
            return SoundEffects[key.ToString()];
        }


        public static SoundEffectInstance CreateInstance(object key)
        {
            return SoundEffects[key.ToString()].CreateInstance();
        }

        public static void Dispose()
        {
            foreach (var sndfx in SoundEffects)
            {
                sndfx.Value.Dispose();
            }
            SoundEffects.Clear();
        }


        public static void Stop()
        {
            foreach (var sndfx in SoundEffects)
            {
                sndfx.Value.Dispose();
            }
            soundEffectsToPlay.Clear();
        }



        static Queue<vxSoundEffectInfo> soundEffectsToPlay = new Queue<vxSoundEffectInfo>();






        public static void PlaySound(object sender, object key, float Volume = 1, float Pitch = 0, bool UseTransitionAlpha = true)
        {
            if(soundEffectsToPlay.Count < MaxSoundsToQueue)
                soundEffectsToPlay.Enqueue(new vxSoundEffectInfo(sender, key, Volume, Pitch, UseTransitionAlpha));
        }

        public static void PlaySound3D(object sender, object key, bool isLooped, Vector3 position, float pitch = 0)
        {
            if (soundEffectsToPlay.Count < MaxSoundsToQueue)
                soundEffectsToPlay.Enqueue(new vxSoundEffectInfo(sender, key, Volume, pitch, true));
        }

        #endregion

        public static int MaxSoundsToQueue = 200;


        static int playPerUpdateCount = 4;

        static float Volume = 1;
        /// <summary>
        /// Updates the state of the 3D audio system.
        /// </summary>
        public static void Update(GameTime gameTime)
        {
            #region Update Sound Effects
            vxConsole.WriteInGameDebug("AUDIO MANAGER", soundEffectsToPlay.Count);
            for (int i = 0; i < playPerUpdateCount; i++)
            {
                if (soundEffectsToPlay.Count > 0)
                {
                    var sndInfo = soundEffectsToPlay.Dequeue();

                    // Now play this key
                    try
                    {
                        // Max the Volume at 1.
                        Volume = MathHelper.Min(sndInfo.Volume, 1);

                        // Create the Instance of the Sound Effect.
                        SoundEffectInstance sndEfctInstnc = SoundEffects[sndInfo.key.ToString()].CreateInstance();

                        // Set the Volume
                        sndEfctInstnc.Volume = Volume * vxAudioManager.SoundEffectVolume * (sndInfo.UseTransitionAlpha ? Engine.CurrentScene.TransitionAlpha : 1);

                        // Set the Pitch
                        sndEfctInstnc.Pitch = sndInfo.Pitch;

                        // Play It
                        sndEfctInstnc.Play();

                    }
                    catch (Exception ex)
                    {
                        vxConsole.WriteException(typeof(vxAudioManager), new vxSoundEffectException(sndInfo.sender, sndInfo.key, ex));
                    }
                }
                #endregion

                // Loop over all the currently playing 3D sounds.
                int index = 0;

                while (index < active3DSounds.Count)
                {
                    ActiveSound activeSound = active3DSounds[index];

                    if (activeSound.Instance.State == SoundState.Stopped)
                    {
                        // If the sound has stopped playing, dispose it.
                        activeSound.Instance.Dispose();

                        // Remove it from the active list.
                        active3DSounds.RemoveAt(index);
                    }
                    else
                    {
                        // If the sound is still playing, update its 3D settings.
                        Apply3D(activeSound);
                    }

                    index++;
                }
            }
        }


        /// <summary>
        /// Triggers a new 3D sound Based off of Entity
        /// </summary>
		public static SoundEffectInstance Play3DSound(SoundEffect soundEffect, bool isLooped, vxEntity3D emitter)
        {
            ActiveSound activeSound = new ActiveSound();

            // Fill in the instance and emitter fields.
            activeSound.Instance = soundEffect.CreateInstance();
            activeSound.Instance.IsLooped = isLooped;

            activeSound.Emitter = emitter;

            // Set the 3D position of this sound, and then play it.
            Apply3D(activeSound);

            activeSound.Instance.Volume = vxAudioManager.SoundEffectVolume;

            activeSound.Instance.Play();


            // Remember that this sound is now active.
            active3DSounds.Add(activeSound);

            return activeSound.Instance;
        }


        public static SoundEffectInstance Play3DSound(SoundEffect soundEffect, bool isLooped, Vector3 position)
        {
            return Play3DSound(soundEffect, isLooped, position, 1);
        }

        /// <summary>
        /// Triggers a new 3D sound Based off of Entity Position.
        /// </summary>
        public static SoundEffectInstance Play3DSound(SoundEffect soundEffect, bool isLooped, Vector3 position, float pitch)
        {
            ActiveSound activeSound = new ActiveSound();

            // Fill in the instance and emitter fields.
            activeSound.Instance = soundEffect.CreateInstance();
            activeSound.Instance.IsLooped = isLooped;

            //vxEntity emit = new vxEntity(position);
            //emit.World.Translation = position;
            activeSound.Emitter = new vxEntity3D(Engine.GetCurrentScene<vxGameplayScene3D>(), position/100);

            // Set the 3D position of this sound, and then play it.
            emitter.Position = activeSound.Emitter.WorldTransform.Translation;
            emitter.Forward = activeSound.Emitter.WorldTransform.Forward;
            emitter.Up = activeSound.Emitter.WorldTransform.Up;
            emitter.Velocity = activeSound.Emitter.Velocity;
            
            activeSound.Instance.Apply3D(listener, emitter);
            
            activeSound.Instance.Volume = vxAudioManager.SoundEffectVolume;

            activeSound.Instance.Play();

            // Remember that this sound is now active.
            active3DSounds.Add(activeSound);

            return activeSound.Instance;
        }


        /// <summary>
        /// Updates the position and velocity settings of a 3D sound.
        /// </summary>
        private static void Apply3D(ActiveSound activeSound)
        {
            emitter.Position = activeSound.Emitter.WorldTransform.Translation/10;
            emitter.Forward = activeSound.Emitter.WorldTransform.Forward;
            emitter.Up = activeSound.Emitter.WorldTransform.Up;
            emitter.Velocity = activeSound.Emitter.Velocity;

			SoundEffect.DistanceScale = 2000000;
			SoundEffect.DopplerScale = 0.51f;
			SoundEffect.MasterVolume = 1;
            activeSound.Instance.Apply3D(listener, emitter);
        }


        /// <summary>
        /// Internal helper class for keeping track of an active 3D sound,
        /// and remembering which emitter object it is attached to.
        /// </summary>
        public class ActiveSound
        {
            public SoundEffectInstance Instance;
			public vxEntity3D Emitter;
        }
    }
}