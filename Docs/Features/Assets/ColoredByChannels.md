## Colored by Channels Shader (PBR) ##

This shader will color pixel with colors from properties **RedColor**, **GreenColor** and **BlueColor** depending on the texture's pixel color.  

Exact formula is **`RedColor * color.red_channel + GreenColor * color.green_channel + BlueColor * color.blue_channel`**.  

This shader supports only 3 colors. For most uses this is enough, but some do require more, and that shader becomes inapplicable. Use unique textures instead.  

**Main usage: character colors.**  