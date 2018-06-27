# Vertices Engine Tempaltes

<a href="https://twitter.com/intent/follow?screen_name=virtexedge">
        <img src="https://img.shields.io/twitter/follow/virtexedge.svg?style=social&logo=twitter"
            alt="follow on Twitter"></a>

is an in-house developed game engine based in and built from the ground up in C#. It uses @MonoGame as it's back end allowing it to run cross platform on PC, OSX, Linux, iOS and Android.

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
<!--## Real-time Surface and Water Reflections
![Reflections](https://virtexedgedesign.files.wordpress.com/2015/10/reflections.png)-->

## Crepuscular Rays(God Rays)
![Crepuscular Rays](https://virtexedgedesign.files.wordpress.com/2015/10/godrays.png)

## Depth of Field
![Depth of Field](https://farm2.staticflickr.com/1476/25396320090_422ec688b0_z.jpg)

## Cascade Shadow Mapping
![Cascade Shadow Mapping](https://virtexedgedesign.files.wordpress.com/2015/10/shadowmaps.png)

## Extendable in-game Sandbox for Rapid Level design
![Sandbox](https://virtexedgedesign.files.wordpress.com/2015/10/sandbox.png)

## Integrated Debug System, Viewer and Console
![Debug](https://virtexedgedesign.files.wordpress.com/2015/10/debug.png)

## Customizable and Sinkable GUI system
![GUI](https://virtexedgedesign.files.wordpress.com/2015/10/gui.png)

# Other features
* Modular design allowing for smaller and more efficient 2D and 3D applications.
* Touch screen, gamepad and traditional keyboard-mouse support.

# 3rd Party Library Integration
* 3D Physics through platform independent fork of BEPU Physics.
* 2D Physics provided by Farseer Physics library.
* Networking support through Lidgren Networking Library.
