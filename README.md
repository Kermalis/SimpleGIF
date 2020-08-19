# SimpleGIF is a super-simple library to read GIF files and each frame of their animation.

* You can read the decompressed bitmap without converting it to a pixel format
* You can get the colors of each frame's palette, so you can draw the decompressed bitmap yourself if you wish
* You can use this library to convert the decompressed bitmap to a regular bitmap of a specific pixel format, so you don't need to do any drawing yourself
* You can browse the GIF contents through objects without doing any drawing

This library uses .NET Standard 2.0.

----
# SimpleGIF uses:
* [EndianBinaryIO](https://github.com/Kermalis/EndianBinaryIO)

----
This project is made possible by [XamlAnimatedGif](https://github.com/XamlAnimatedGif/XamlAnimatedGif) and [What's in a GIF](http://www.matthewflickinger.com/lab/whatsinagif/index.html).
XamlAnimatedGif is the only C# library that I found that reliably reads GIF files, but I was not using XAML for my application, so I ported over the GIF-specific code here.
XamlAnimatedGif decodes the GIFs on the fly, which I also did not want (due to high CPU usage with just a few GIFs), so this library lets you get each/every frame already decoded as you wish.