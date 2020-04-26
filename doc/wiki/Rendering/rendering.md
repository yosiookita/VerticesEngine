 # Rendering Pipeline
 The rendering pipeline works by the following method:

 # Camera.Render();

 Each Camera will then render, calling their post processing stack.

 ## SetViewPort();
 The camera set's the graphics devices' viewport to it's bounds

 ## Renderer.Prepare();
 The renderer does a prep pass creating masks and maps which will be used later on.

 ## Renderer.DrawScene();
 This draws the scene with the default scene renderer.

 ## Renderer.ApplyPostProcess();
 This draws the required Post Process for the in the stack

 ## Finalise
 The camera draws it's result to a render texture which will then be composited at the very end by the Engine.