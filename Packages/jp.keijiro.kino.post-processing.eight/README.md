KinoEight
=========

![gif](https://i.imgur.com/KJ4pgJ3.gif)
![gif](https://i.imgur.com/gSs1Lc4.gif)

**KinoEight** is a custom post-processing effect that gives an 8 bit-ish style
to renders.

System requirements
-------------------

- Unity 2019.3
- HDRP 7.1

How To Install
--------------

This package uses the [scoped registry] feature to resolve package
dependencies. Please add the following sections to the manifest file
(Packages/manifest.json).

[scoped registry]: https://docs.unity3d.com/Manual/upm-scoped.html

To the `scopedRegistries` section:

```
{
  "name": "Keijiro",
  "url": "https://registry.npmjs.com",
  "scopes": [ "jp.keijiro" ]
}
```

To the `dependencies` section:

```
"jp.keijiro.kino.post-processing.eight": "1.0.0"
```

After changes, the manifest file should look like below:

```
{
  "scopedRegistries": [
    {
      "name": "Keijiro",
      "url": "https://registry.npmjs.com",
      "scopes": [ "jp.keijiro" ]
    }
  ],
  "dependencies": {
    "jp.keijiro.kino.post-processing.eight": "1.0.0",
    ...
```

Eight Color effect
------------------

![eight color](https://i.imgur.com/gqqSnl6.png)

The **Eight Color** is a color reduction effect with an eight-color palette.
You can use the Dithering option to soften bandings with a low-resolution
dithering pattern. You can also use the Downsampling option to pixelate the
input image.

Tiled Palette effect
--------------------

![tiled palette](https://i.imgur.com/qJ6OWwl.png)

Many of the old 8-bit consoles/computers manage palettes per small (like 8x8 or
16x16) tiles. This limitation introduces an artifact called [attribute clash].

[attribute clash]: https://en.wikipedia.org/wiki/Attribute_clash

The Tiled Palette effect imitates this artifact. It splits the screen into 8x8
blocks and applies two given palettes. It compares how match they are and
select the best-matching one.

You can use the dithering and downsampling options as well. There is also a
Glitch parameter that adds random glitches to the output image.

![glitch](https://i.imgur.com/Pnl5a3Q.png)
