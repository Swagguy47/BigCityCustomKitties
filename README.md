# Big City Custom Kitties

A simple BepInEx plugin to allow custom textures and eye colors to be loaded into: Little Kitty Big City for customizing the player cat. 

--------------------------------
### Requirements:
- BepInEx (https://github.com/BepInEx/BepInEx/releases/tag/v5.4.23.2)
--------------------------------
### Installation:

1. Locate your game folder containing the game's executable `Little Kitty, Big City.exe`.
   - This is where you will extract the contents of the following zips.
2. Extract the contents of [BepInEx_win_x64_5.4.23.2.zip](https://github.com/BepInEx/BepInEx/releases/download/v5.4.23.2/BepInEx_win_x64_5.4.23.2.zip) to your game folder. 
3. Extract the contents of [BCCustomKitties_v1.2.0.zip](https://github.com/Swagguy47/BigCityCustomKitties/releases/download/v.1.2.0/BCCustomKitties_v1.2.0.zip) to your game folder. 
4. Test that the installation worked by:
     - Renaming `/LittleKittyBigCity/Skins/Current.png` to `default.png`
     - Moving `/Skins/extras/Vet.png` to `/Skins/Current.png`
     - After loading into the game, the cat should be orange

--------------------------------
### How to use:

Place a *png* into the "Skins" folder of your game install named "Current"

Please ensure any reskin's resolution is: 2048 x 2048 pixels

Edit the EyeColors.txt file with your own hex codes to recolor your eyes

--------------------------------
### Misc notes:

You can press F1 in game to reload texture & colors at any time. Useful to testing.

additional textures are included in the "Skins/extras" folder and belong to other cats in the world, 
though some have visual issues when applied to the player.

"extras/PaintingMesh.fbx" is an unrigged model of the player cat for use in making your own textures.

The default texture & eye colors match the original appearance. Don't let that confuse you when first testing the mod.

--------------------------------

### Previews:

![image](https://github.com/Swagguy47/BigCityCustomKitties/assets/67041649/a4fb3b22-5e5d-4707-bbb6-c65e92885dd3)

![image](https://github.com/Swagguy47/BigCityCustomKitties/assets/67041649/0ee754be-f2e9-410d-a03f-0786e5ae5030)

![image](https://github.com/Swagguy47/BigCityCustomKitties/assets/67041649/c5e39423-a87a-43cf-bbd0-91d9ac64386b)
