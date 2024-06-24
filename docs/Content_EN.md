# Adding Other Contents

[简体中文](Content.md) English

This page tell you the details on how to add other contents to your mod. If you haven't read [Units](ShipUnit_EN.md) page, please read that first.

## What content can be added to your mod:

The following checked items can be easily added to your mod with csv (similar to Units). The unchecked ones are working in progress (WIP) and cannot be added in such a way. However, you may still add them by writing [Patches](Patch_EN.md). 

- [x] [Units](ShipUnit_EN.md)
- [x] [Treasures](#Treasures)
- [x] [Talents](#Talents)
- [x] [Pilots](#Pilots)
- [ ] Player Ships
- [ ] Enemy Ships
- [x] [Emergency Events](#BattleEvents)
- [x] [Keywords & Buffs](#Keywords)
- [x] [Events & Call For Supports](#Events)
- [ ] Laser Skins

## Common fields
These are common fields that most types of contents share.

- ID: Must be unique in your mod.
- Name/Name_: Display name of this item.
- UnlockLV: 
    - In most cases: Association Lv required to enable this item. (0-70)
    - For Units: Ship Lv required to enable this item. (0-10)
- InGame: Set to `FALSE` to disable* this item.
- Pros: This item is enabled exclusive to listed ships.
    - Mega-ship counts as the original ship.
    - If element numerical, refers to a ship in vanilla game;
    - If element in the format: `<ModID>.<ShipID>`, refers to a ship in another mod;
    - Otherwise: element refers to a ship in this mod.
- WeightOffset: Set this value to make this item appear X% more or less frequently.
- Description/Description_: Item descriptions.
- Args: Arguments of ability.
- SkillPath: Name of the ability class.
- SpritePath: Image of this item.

\*Disabled: Can't appear in shop or random rewards, but may appear as a fixed reward of an event or in a historical save. Doesn't appear in the encyclopedia.

## Treasures <a id="Treasures"></a>

- File: Content/Treasure.csv
- Example ID: Vulcan
- Sprite Size: 120x120

Fields exclusive to this item:

- Rare: 0 for common, 1 for rare, 2 for legendary.
- GainType: 
    - 0: Enabled.
    - 1: Only available from shops.
    - 2: Only available from events.
    - 3: Only available from talents.
    - 4: Only appear as initial items.
    - 5: Disabled.
- OnlyTalent: This treasure is enabled only if target talent is present. (Making it an pilot exclusive treasure.)
    - If numerical, refers to a talent in vanilla game;
    - If in the format: `<ModID>.<TalentID>`, refers to a talent in another mod;
    - Otherwise: refers to a talent in this mod.
- Genera: Categorize some treasures to smaller groups for tag searching. 
- BattleRelate: Whether this is related to battle. (There is an option to hide battle-irrelevant treasures during battles.)
- AddIn: This treasure is disabled until you finish the first Xth galaxy. If set to 0, available from the start.
- RemoveOut: This treasure is disabled when you finish the Xth galaxy. If set to 0, available till the end.

## Emergency Events<a id="BattleEvents"></a>

- File: Content/BattleEvent.csv
- Example ID: TopRain
- Sprite Size: 128x128. 
    - The image is white and transparent by default.
    - The image has corners cut when displayed, as is shown in the image below. The displayed area is an octagon.

![battleEvent_t.png](../images/battleEvent_t.png)

Fields exclusive to this item:

- Effect/Effect_, ExtraInfo: (See the image below)

![battleEvent2.png](../images/battleEvent2.png)

- BanEnemyIDs: This event won't appear when facing listed enemies. 
    - If element numerical, refers to an enemy in vanilla game;
    - If element in the format: `<ModID>.<EnemyID>`, refers to an enemy in another mod;
    - Otherwise: element refers to an enemy in this mod.
    - Eg. adding 511 to BanEnemyIDs will make `TopRain` no longer appear at Bomber, preventing it from being too trivial.

- BanPhases: This event won't appear in listed battles. 
    - 0 = first battle (1st battle), 11 = final boss (12th battle)
    - Boss battles have no emergency events.
    - Eg. Adding 8, 9, 10 to BanPhases will make `TopRain` no longer appear in the last galaxy, where it has very little effect.

## Talents <a id="Talents"></a>

- File: Content/Talent.csv
- Example ID: CoinGain
- Sprite Size: 100x100

Fields exclusive to this item:

- Type: 1 for Random Talents, 2 for Inherent Talents
- InProgress: set to FALSE to make this talent only available at game start. (Eg. Shifting Block)
- TalentBanList: This talent conflicts with listed talents.
    - If element numerical, refers to a talent in vanilla game;
    - If element in the format: `<ModID>.<TalentID>`, refers to a talent in another mod;
    - Otherwise: element refers to a talent in this mod.
    - Relationship is mutual.
- BanEnemyList: This talent conflicts with listed enemies.
    - Same as above.

## Keywords & Buffs <a id="Keywords"></a>

- File: Content/Keyword.csv
- Example ID: DeathRattle (Only a showcase, effects not correctly implemented.)
- Sprite Size: 40x40

Fields exclusive to this item:

- Keyword/Keyword_: A set of keywords to be highlighted. 
    - Each of them should be embraced by "\*" and separated by ";" 
    - Eg. "\*DeathRattle\*;\*Death rattle\*"
- KeywordDes: Creates a tooltip.
    - If left blank, there will be no tooltips. (Only highlights the keyword.)
- Color: Color of the highlighted keyword.
- ImageGroup, Image: The icon attached to the keyword.
    - If both blank, no icon.
    - If ImageGroup is not blank, use the icon from vanilla game.
    - If ImageGroup is blank, use the icon provided by your mod.
    
![deathrattle.png](../images/deathrattle.png)

### Create Buffs from Keywords
If you want to create a buff from a keyword and attach it to units (like power), simply inherit `Buff` and set its `keyWordString` to your `Keyword`. (In this case you must use a translated `Keyword` instead of `Keyword_`.) 

## Events & Call For Supports  <a id="Events"></a>

- File: Content/EncounterEvents.csv
- Example: Gamble

A full event may contain multiple pages. (Eg. Choosing an option may enter another page.) Each event page may have multiple options. Each option may have multiple scripts, (Eg. Star Coin gain at the cost of HP lose) and may have chances of triggering different consequences. (Eg. winning or losing a gamble)

The event structure is as follow:
```
└─Event, indicated by EventGroup (Eg. gambling event)
    └─Event Page, indicated by ID (Eg. in one bet, bet 5 coins or leave)
        └─Option, each line is an option. (Eg. bet 5 coins)
            ├─ScriptGroup (lose 5 coins)
            │   └─Script (lose 5 coins)
            └─ScriptGroup (win 10 coins or lose the bet)
                ├─Script (50%: close the event)
                └─Script (50%: win 10 coins, go next page)
```

Fields exclusive to this item:

- ID: Options on a same **event page** should have the same ID.
- EventGroup: Options of a same **full event** should have the same EventGroup.
- Days: Days spent on this event. 0 if this is a special event. (Call for support or reward event)
- EnterType: Set this field to 1 if this is a bad event (as a result of the previous choice). This is optional.
    - Have a special effect upon entering. 
    - Usually used in a bad roll when event involves probability. (Eg. failure in a gamble, HP lose on a risky attempt)
- Tags: The event type to display on vacation choice screen. Each vacation event should has at least 1 tag.
    - 1: Units
    - 2: Treasures
    - 3: Star Coins
    - 4: Upgrade
    - 5: Load Limit (Not used)
    - 6: Energy Source
    - 7: Repair
    - 8: Shop
    - 9: Shield
    - 10: Modification
    - 11: Fuel
    - 12: Swap
    - 13: End Vacation (Not used)
- RaceSpecial: This option is exclusive to a race.
    - 1: Human
    - 2: Mech
    - 3: Beastkin
    - 4: Treant
- OptionDes/OptionDes_: Description of this option.
- OptionScript1-3: Class name of this script
- Args1-3: Arguments of this script.
- Weight1-3: Chances of this script to be chosen in a  ScriptGroup.
- Group1-3: Which ScriptGroup this script belongs to. 
    - Exactly 1 script of each ScriptGroup will be executed
    - Eg: if there are 3 scripts: S1 (Group1 = 1), S2 (Group2 = 2, Weight2 = 40), and S3 (Group3 = 2, Weight3 = 60), then S1 will be executed first, then SB (40%) or SC (60%) will be executed.
- Jump1-3: Jumps to a new page when this script finishes. 
    - If left empty, end this event.
    - If multiple scripts are executed, the last non-empty jump will take effect.
    - Some scripts may have multiple Jump targets. (Eg. when trading a Unit for star coins, if cancelled at the unit choice screen, will jump to a different page, offering no star coins and showing a different description.) 
    - The jump target can also be controlled with code by setting `option.jumpID`.

### Writing your own scripts

Inherit the `OptionContent` Class and implement the  following functions:

- `OnOptionInit`: Process description text, lock the reward, hide this option, etc. 
- `Check`: Check preconditions of this option. Return false to disable this option. (Eg. when you have not enough star coins to pay)
- `Do`: Execute this script.

If you have correctly extracted the game files in [Getting Started](Start_EN.md#disassemble-the-game-optional), there are many examples that will help you design your events. Note that the field types of extracted csv file are slightly different from that of TutorialMod. (Eg. ID in vanilla game is int instead of string) Please use TutorialMod's version.

### Turning an event into a Call for Support

Similar to adding contents like events or treasures.

- File: Content/BountyEvents.csv
- Example: Gamble
- Sprite Size: 50x50

The ID of the Call for Support should be identical to that of the event entrance.

### Notes on Events
- All events should include a safe exit to prevent looping and no choice situations.
- Call for support events and reward events should cost 0 days and mark InGame to `FALSE`. Vacation events should cost 2, 3, or 4 days.
- When call for support events finishes, use `OC_SetBountyFinished` to consume the support chance and trigger treasure abilities.
- Call for support events should have an option to exit the event without consuming the chance and also without any side effects. (i.e. to exit without calling `OC_SetBountyFinished`)
- As a result of UI arrangement, each event page can only have at most 6 options (including hidden options) or 5 shown options (including disabled options).

## Pilots <a id="Pilots"></a>

- File: Content/Pilot.csv
- Example ID: Pip
- Sprite Size: 512x512

Fields exclusive to this item:
- PilotTitle/PilotTitle_ & PilotName/PilotName_: Two parts of this pilot's name.
- Race: race of this pilot.
    - 0: Human
    - 1: Mech
    - 2: Beastkin
    - 3: Treant
    - 9: All
- Talent: Inherent talent of this pilot. Can be a talent in  vanilla game (numerical), from another mod (`<ModID>.<TalentID>`), or in your mod (otherwise).
- RandomSlotNum: This pilot is spawned with that many random talents. 
- Remark: Remark of this pilot's talent, or write anything you like. Has no effect.
- PilotMonologue: Controls the talking behavior of this pilot.
    - To create monologue, inherit the `PilotMono` class and write code on when and what to speak.
    - PilotMono acts very much like a treasure. The difference is its event listeners are added manually instead of automatically.
    - Examples see `PM_<PilotID>` (Eg. `PM_10000`) in [extracted game files](Start_EN.md#disassemble-the-game-optional).
    - If left blank, there will be no talking at all.
