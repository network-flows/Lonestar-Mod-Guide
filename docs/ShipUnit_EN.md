# Unit

This page tells you how to add new units or modify existing units in your mod. All the steps can be found in `TutorialMod`.

## Example A: Starting from a Basic Core

Add `ShipUnit.csv` to `Content` directory. As seen in the file structure below.

```
└─TutorialMod
    ├─Content    
    │   ├─ShipUnit.csv
    │   └─...(Other Files)
    ├─Animations
    ├─Images
    └─...(Other Files)
```

Each line in `ShipUnit.csv` represents a new Unit you add. It's advised to use csv editors like MS Excel instead of plain text editors like notepad. However, some csv editors may cause encoding issues. If you encounter any encoding issues, try change the encoding to UTF-8.

Create your first unit by copying 2 rows of `unitA` in `TutorialMod``. Each row represents a level of a unit. This unit does nothing, but we can add more things later.

Note: LoneStar uses the combination of `ID` and `modID` to identify an item. That means units from different mods can share a same `ID` (their mods are different), but treasures and units in a same mod can't have the same `ID` (they refer to different things).

### Example B: Translating Your Unit & Modifying Parameters

`unitB` is a unit with translated texts. You may notice that some string fields have two versions. Take `Name` for example: you may leave `Name` blank and fill in `Name_` to set your Unit name for all languages (see `unitA`), or set `Name` and add translation to each language respectively (See `unitB`). See Translation part for more info.

Even if you write no code, you may paste existing effects to your unit. To use the effect of `Triangular Core`, set `SkillPath` field of your unit to `Skill_TrianglePowerCannon`. If you want to modify the amount of Power when TriPowered, set `Args` field accordingly.

### Example C: Adding Cool Abilities & Referencing Other Units

`unitC` is a unit with some cool effects. You may set `SkillPath` to some new effects, and implement them in your `.dll` file. (See Patching Part for more info)

To reference a vanilla game unit in description, use `<UNIT>ID|Lv</UNIT>`, to reference a modded unit, use `<MODUNIT>modID|ID|Lv</MODUNIT>`. Units should not reference themselves or create a reference loop.

To use the appearance of an existing unit, (Eg. Patch Device), set `SpritePath` to that of the target unit (`device_equip_cannon`). If left blank, your units will use a default sprite instead. You may also create your own in the next example.

### Example D: Animate Your Units

`unitD` is a unit with different sprites and animations. 

`SpritePath` indicates the appearance when your units appear as an item. (In your inventory, as a reward, etc.) To use your own image, add images of size 100x100 to the `Image` folder and reference them in `SpritePath` field.

`AnimPath` indicates the animation when your units are on the ship. Lonestar supports Spriter animations. 



### Detailed Explanation of Each Field
- ID: String. Must be **unique** in your mod. Upgraded units share the same ID with their downgraded version.
- Lv: Can be 1 or 2, (or 3+ if your unit can be upgraded multiple times). 
- UnlockLv: 0 to always unlock, or unlocks at the set ship Lv (0-10, hidden in game and different from association Lv)
- Rare: 0, 1, or 2
- GainType: 0
- PowerSlot: List of int. 0 for White, 1 for Blue, 2 for Orange.
- EquipLimit: 1
- Description_: String. The description of the unit.