# Sandbox
Item's are registered and instantiated by the sandbox system by setting the appropriate attribute tags.

## Registering Items

```csharp
[vxRegisterAsSandboxEntityAttribute("item name", "category_key", "sub_category_key", "path/to/main/asset")]

// or

[vxRegisterAsSandboxEntityAttribute("item name", "category_key", "sub_category_key", x, y, width, height)]
```
Where the last 4 numbers are spritesheet dimensions.

An example below in Chaotic Workshop:
```csharp

    [vxRegisterAsSandboxEntityAttribute("Boulder", ContentPackType.BaseGame, ItemTypeGroup.BallsAndBoxes, 96, 0, 96, 96)]
	public class Ball_Boulder : BaseGameSandboxItem
	{
        ...
```

## Registering Particles

You can simply register particles by tagging them with the following attribute:
```csharp
    [vxRegisterAsSandboxParticleAttribute("Explosion Particle", ParticleTypes.Explosion, 10)]
    public class ExplosionParticle : vxParticle2D
	{
        ...
```