# Vertices Engine Tempaltes

<a href="https://twitter.com/intent/follow?screen_name=virtexedge">
        <img src="https://img.shields.io/twitter/follow/virtexedge.svg?style=social&logo=twitter"
            alt="follow on Twitter"></a>

The Vertices Engine is the in-house developed game engine from @VirtexEdgeDesign & @rtroe based in and built from the ground up in C#. It uses @MonoGame as it's back end allowing it to run cross platform on PC, OSX, Linux, iOS and Android.

There are recently released Nuget packages which can be found through the badge links below so that users can play around with Vertices on their own machines.
           

# Status

| Platform | Build Status                   | Nuget Package|
|----------|--------------------------------|--|
| DesktopGL| ![alt text][buildOGL] |[![nugetOGL](https://img.shields.io/badge/nuget-released-green.svg)](https://www.nuget.org/packages/Virtex.Lib.Vrtc.GL/)|
| Android  | ![alt text][buildAdr] |[![nugetOGL](https://img.shields.io/badge/nuget-beta-blue.svg)](https://www.nuget.org/packages/Virtex.Lib.Vrtc.Android/)|
| iOS      | ![alt text][buildIOS] |[![nugetOGL](https://img.shields.io/badge/nuget-tbd-orange.svg)](#)|
| DirectX  | ![alt text][buildDrX] |[![nugetOGL](https://img.shields.io/badge/nuget-tbd-orange.svg)](#)|


[buildxna]: https://img.shields.io/badge/build-depreciated-lightgray.svg
[buildDrX]: https://img.shields.io/badge/build-tbd-orange.svg
[buildOGL]: https://img.shields.io/badge/build-passing-green.svg
[buildAdr]: https://img.shields.io/badge/build-passing-green.svg
[buildIOS]: https://img.shields.io/badge/build-passing-green.svg

[nugetSuccess]: https://img.shields.io/badge/nuget-released-green.svg
[nugetbeta]: https://img.shields.io/badge/nuget-beta-blue.svg
[nugetTBD]: https://img.shields.io/badge/nuget-comingsoon-orange.svg
[nugetNA]: https://img.shields.io/badge/nuget-deprecetated-lightgray.svg

# Features

## Cross Platform
Vertices runs on PC, OSX, Linux, iOS and Android and is coming to consoles soon.

## Extendable in-game Sandbox for Rapid Level design
A number of Vertices games are sandbox based, and so the engine is built an integrated in-game editor to add, modify and change a game on the fly.
![sandbox](https://virtexedgedesign.github.io/VerticesEngine/imgs/features/sandbox.png)

## Customizable and Sinkable GUI system
Vertices supports a number of GUI elements from basic buttons and toolbars to more advanced Ribbon Bars and Property Grib controls.
![gui](https://virtexedgedesign.github.io/VerticesEngine/imgs/features/gui.png)

## Integrated Debug System and Console
To help with game development and profiling, Vertices comes with a number of debug profilers and tools.
![Debug](https://virtexedgedesign.github.io/VerticesEngine/imgs/features/debug.png)

## Farseer and BEPU physics library support

#Rendering Pipeline
There is a more indepth look at the renderering pipeline over at Virtex's main site here.

## Deferred Renderer
![Deferred Renderer](https://virtexedgedesign.github.io/VerticesEngine/imgs/renderpipeline/deferred.png)

## Crepuscular Rays(God Rays)
![Crepuscular Rays](https://virtexedgedesign.github.io/VerticesEngine/imgs/renderpipeline/godrays.png)

## Depth of Field
![Depth of Field](https://virtexedgedesign.github.io/VerticesEngine/imgs/renderpipeline/depthoffield.png)

## Cascade Shadow Mapping
![Cascade Shadow Mapping](https://virtexedgedesign.github.io/VerticesEngine/imgs/renderpipeline/shadows.png)

## Screen Space Reflections
![Reflections](https://virtexedgedesign.github.io/VerticesEngine/imgs/renderpipeline/ssr.png)

## SSAO
![ssao](https://virtexedgedesign.github.io/VerticesEngine/imgs/renderpipeline/ssao.png)

# Other features
* Modular design allowing for smaller and more efficient 2D and 3D applications.
* Touch screen, gamepad and traditional keyboard-mouse support.

# 3rd Party Library Integration
* 3D Physics through platform independent fork of BEPU Physics.
* 2D Physics provided by Farseer Physics library.
* Networking support through Lidgren Networking Library.
