# Media Types

## Images

[Easily done for JPEG and PNG files](http://answers.unity3d.com/questions/432655/loading-texture-file-from-pngjpg-file-on-disk.html). Mipmaps generated? Maybe check [this link](http://answers.unity3d.com/questions/10292/how-do-i-generate-mipmaps-at-runtime.html).

## Music

Can be done very dynamically using NAudio. See my implementation.

## Videos

Very hairy matter. Unity Pro comes with a MovieTexture that can be used for such cases but it doesn't work at runtime with any format other than ogg. There are a few 3rd party libraries, but it's unknown whether they work at runtime.

[AVI Player](https://www.assetstore.unity3d.com/en/#!/content/15580) combined with [NReco Video Converter](http://www.nrecosite.com/video_converter_net.aspx) might make it possible, where the latter needs a re-compile without the GZipStream. AVI Player only supports MJPEG files, which the converter can produce!  