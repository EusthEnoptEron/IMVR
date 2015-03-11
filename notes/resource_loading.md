# Resource Loading

Loading textures turned out to be trickier than anticipated. **Unity is unable to load textures asynchronously.** Period. It *is* possible when using Resources (`LoadAsync`) or when using AssetBundles -- both of which cannot be generated at runtime.

On the bright side, loading small-sized power-of-two textures (256x256) seems to be tolerably fast, about 1-5ms each. 512x512 takes about 7-15ms.