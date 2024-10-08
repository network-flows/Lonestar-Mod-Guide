# Patching (Editing Code)

[简体中文](Patch.md) English

This page tells you how to modify functions in the game.

Before you start, make sure you have Microsoft Visual Studio installed on your machine. 

- VS2022 or higher versions recommended
- Download [here](https://visualstudio.microsoft.com/downloads/)
- Choose .Net Desktop dev kit, and go along with default settings.

Download TutorialMod from releases, and open the project with Visual Studio.

## Adding dependencies

### Necessary dependencies
When opening the project for the first time, you may meet some compilation errors. That's resulted from dependency missing. All needed dependencies are located at `LONESTAR\LONESTAR_Data\Managed` (`LONESTAR` directory can be found in Steam > Library > LONESTAR > Settings > Manage > Browse Local Files)

Make a `lib` folder in project directory (It should be in the same folder as the .sln file), and copy the following files to `lib`

- 0Harmony.dll
- Assembly-CSharp.dll
- LeanLocalization.dll
- Unity.TextMeshPro.dll
- UnityEngine.AudioModule.dll
- UnityEngine.CoreModule.dll
- UnityEngine.dll
- UnityEngine.UIModule.dll

Build the project and it should proceed successfully now. Target dll can be found at `bin/Release/TutorialMod.dll`.

### Other dependencies

If you need other dlls or third-party dependencies, right click on the `References` in the Solution Explorer view. If there is no Solution Explorer view, open one from the `View` tab on the top menu.

![dependencies.png](../images/dependencies.png)

Click `Browse` to add references.

![dependencies2.png](../images/dependencies2.png)

Choose the needed files and click OK, and it's done.

## The UserMod class
Your assembly should have exactly one class inheriting `UserMod`. See the `TutorialMod` class for example.

```
using HarmonyLib;
using Mods;

namespace TutorialMod
{
    public class TutorialMod : UserMod
    {
        public override void OnLoad()
        {
            base.OnLoad();
            UnityEngine.Debug.Log("Successfully Loaded [" + this.modID + "] at " + this.path);
        }
    }
}
```

### void OnLoad()

Called when your mod is loaded. Calling `Onload()` will automatically load every patching classes in this assembly, as well as other parts like units, treasures, translations, etc. in a predefined order. It's usually enough to use the default `OnLoad` method, but if you want to do something different (Eg. attaching scripts, or only patching part of the assembly), you may override `OnLoad`.

### void OnAllModLoad(IReadOnlyList\<UserMod\> mods)

Called when all mods are loaded. You can check how many mods are active and what exactly they are. It's OK to leave blank.

### Some useful fields

Here are some useful fields of UserMod you may use when writing your mod.

- assembly (Assembly): A reference to this assembly.
- harmony (Harmony): The Harmony instance used to patch your mod.
- path (string): Full path to the root directory of your mod.
- modID, info.description, info.displayName,... (string): Info about your mod. (everything defined in your `mod.json`)
- info.is_dev (bool): Whether this mod is located in the `dev` directory.
- info.order (int): The order your mod is loaded. Smaller order loads first.

## Writing patches and adding new content

### Changing game logic

There is an example of how to write patches in `Patch_WantedProcess` class. To learn more about Harmony patches and annotations, please refer to [Harmony docs](https://harmony.pardeike.net/articles/intro.html).

### Adding new content

Refer to the classes `Skill_CustomAbility` and `Skill_Animated` to learn how to add new abilities to your units. To make other mods also recognize your contents, **all contents should be under the namespace same as your modID**.

Note that most event triggers are caught by the corresponding interfaces. Some interfaces catch very similar behaviors, but have differences on timing. Take battle start effects as an example: Unit upgrades triggers at IBattleStartPre (Upgrade Device), slot addition triggers at IBattleStart (Eternal Slot), while slot degrading triggers at IBattleStartEnd (Breakthrough). So these effects will always trigger in order regardless of their positions on the ship.

## Mod Settings
You may add custom settings to your mod. (eg. to enable/disable some features) Simply add a field named `config` to your class and and annotate its members with the corresponding attributes:

```
public class TutorialMod : UserMod
{
    public class TutorialModConfig
    {
        public int other;

        [StringField("title", "some tooltips")]
        public string stringField = "default value";
        ...
    }
    public static TutorialModConfig config = new TutorialModConfig();
    ...
}
```

The config class will be stored to `config.json` under the mod folder. (No worry about overwriting issues. This file is excluded when you update the mod.) Fields not annotated will not show up in the settings windows, but will nevertheless be saved. The config object should not contain non-json-serializable fields.

## Building and adding your .dll to your mod

Build your project in Release mode, and the target dll will be found at `bin/Release/YourModName.dll`. Add target dll along with third-party dependencies (if any) to the root directory of your mod. (Name of the dll doesn't matter.) **Do NOT add any dlls that already exist in the game**. Launch the game and your patches should work now.