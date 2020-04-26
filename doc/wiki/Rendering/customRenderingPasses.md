# Creating Custom Post Processes

You can create custom post processes and rendering passes by adding them to the Renderer/Camera.

```csharp
foreach (var Camera in Cameras)
{
    // Add rendering pass before main pass
    Effect sdwhShader = SceneContent.Load<Effect>("Shaders/EntityShadows");
    Camera.Renderer.RenderingPasses.Insert(0, new ShadowPostProcess(Camera.Renderer, sdwhShader));
    Camera.Renderer.RenderingPasses.Insert(0, new BackgroundPostProcess(Camera.Renderer, sdwhShader));

    // add secondary pass at the end of the current rendering pass list
    Effect diaShader = SceneContent.Load<Effect>("Shaders/EntityDiagram");
    DiagramPostProcess = new DiagramPostProcess(Camera.Renderer, diaShader);
    Camera.Renderer.RenderingPasses.Add(DiagramPostProcess);
}
```