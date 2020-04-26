# Sound Effects

## Loading Sound Effects

Sound effects must be registered with the SoundEffect Manager using a chosen object key and providing the path to the sound effect file.

```csharp
vxAudioManager.LoadSoundEffect(Content, key, "path/to/soundeffect");
```

## Playing Sound Effects
You can play a sound effect simply by calling:
```csharp
vxAudioManager.PlaySound(this, key, volume);
```