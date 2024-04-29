# Getting Started

This page tells you the file structure of your mod, and introduces the tools you may need to create your mod. A template mod can be found in `TutorialMod`.

Note that you don't necessarily need any coding in your mod, although some advanced features are only possible if you have C# knowledge. 

## What you can mod
- Add a new language and its translations (No code involved)
- Add new contents (Some involves code)
    - Units/Treasures/Talents/Events.
    - Translate descriptions to different languages.
    - Use an existing ability or create your own.
    - Create images for your units, or use an existing one.
    - Add animations to your Units.
    - Modify existing content.
- Modify game logic (Involves code)

## Disassemble the game (Optional)
If your mod involves code, you may need to disassemble the game first. There are tools that help you do this job. You can get game assets with `AssetRipper` ([Link](https://github.com/AssetRipper/AssetRipper)) and view code with `Dnspy` ([Link](https://github.com/dnSpy/dnSpy)).

In AssetRipper: File > Open Folder > Select `LONESTAR` directory (Find the path in Steam, as shown in the picture below)

![Alt text](../images/Start1.png)

Then Export > Export All Files, and you get the assets of LoneStar.

Some folders that may help in your make your mod:
```
└─ExportedProject
    └─Assets
        ├─Scripts
        │   └─Assembly-CSharp // Game code
        └─Resources
            ├─csv  // Data for Units, Treasures, etc.
            ├─sprites // Image assets
            └─textmap // Translation
```

In Dnspy: File > Open > Open `LONESTAR/LONESTAR_Data/Managed/Assembly-CSharp.dll` (`LONESTAR` folder can be found in Steam as shown in the above picture)

Dnspy don't extract assets of the game, but it supports code searching and game logic modifications, which helps a lot in understanding code mechanics.


## Mod Structure
All mods are located in `%APPDATA%/../LocalLow/Shuxi/LONESTAR/Mods` directory. 
```
└─Mods
    ├─Dev  // mods in development, always active
    │   ├─Mod1
    │   ├─Mod2
    │   └─TranslationAutoComplete  // Translation related
    ├─Steam // mods from Steam Workshop
    │   └─TutorialMod  // Tutorial mod
    └─mod_settings.json  // List of mods and their activation status
```

The structure of a Lonestar mod: (Take TutorialMod for example): 

```
└─TutorialMod
    ├─Animations  // Animations in Spriter format
    ├─Images
    ├─Content  // Units, Treasures, etc. in csv format
    │   ├─ShipUnit.csv
    │   └─Treasure.csv
    ├─Translation
    │   ├─English
    │   ├─ChineseSimplified
    │   └─CustomLanguage
    ├─mod.json  // Important info of the mod
    └─TutorialMod.dll  // Code
```

Note that everything except `mod.json` is optional. If your mod contains no animations or images, it's OK to delete corresponding directories.

## mod.json
Start your mod by placing `mod.json` in `Dev`, or a subdirectory of it. This file contains the information about your mod, and is the only necessary file of your mod. A `mod.json` looks like this: 

```
{
    "modID" : "TutorialMod",
    "displayName" : "Tutorial Mod",
    "description" : "Template and Tutorial Mod for LoneStar",
    "version" : "1.0.0",
    "author" : ""
}
```

- modID: (Necessary) ID of your mod, must be unique. 
- displayName: (Optional) Name displayed on the mod screen.
- description: (Optional) Description displayed on the mod screen.
- version: (Optional) Version displayed on the mod screen.
- author: (Optional) Author displayed on the mod screen.

If multiple mods with the same modID are detected, only the first is used. Note that dev mods are always loaded first. So if you have a same steam mod, they will be replaced by their dev version.

## Testing your mod
There are tools that help you debug your mods, like [BepInEx](https://github.com/BepInEx/BepInEx) or [UnityExplorer](https://github.com/sinai-dev/UnityExplorer). They are available in Github.

Also there is a mod `loadout` ([link](../Loadout)) that may help you debug your units. And a mod `TutorialMod` ([link](../TutotialMod) & [Code](../TutorialMod_code)) that is used as an example in this document.

## Publish your mod
To-be-implemented