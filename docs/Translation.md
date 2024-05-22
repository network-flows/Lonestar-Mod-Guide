# 添加翻译
简体中文 [English](Translation_EN.md)

本文主要介绍如何在模组中添加翻译。所有例子都能在`TutorialMod`中找到。

## 文件结构

TutorialMod的文件结构大致长这样。这里暂时忽略其他文件，只展开翻译相关的文件。

```
└─Tutorial
    ├─Translation    
    │   ├─ChineseSimplified
    │   │   ├─TranslationPart1.csv
    │   │   ├─TranslationPart2.csv
    │   │   └─...
    │   ├─English
    │   │   └─Translation.csv  
    │   └─CustomLanguage
    │       └─Translation.csv
    └─...(Other Files)
```

翻译文本位于`Translation`目录。每个子目录都代表一种语言。孤星猎人目前支持中文`ChineseSimplified`和英文`English`两种语言，你也可以后续添加新的语言。

每个子目录可能包含一个或多个CSV文件。每个文件包含对应语言的一部分翻译内容。CSV文件的名称没有限制也没有特殊作用，不过它们能帮你辨别每个文件对应哪些翻译内容。

CSV文件的每一行应该符合这样的格式：
```
<Key>;"<Value>"
```
例如
```
YourModName/Unit/YourUnitName;"模组核心"
```

推荐使用纯文本编辑器而不是Excel来编辑这些CSV文件，因为Excel可能会导致编码格式问题。如果你遇到编码错误或者文本显示不正常的问题，可以尝试把这些CSV文件保存为UTF-8格式。

## 添加新语言

在`Translation`目录下新建一个名为`CustomLanguage`的目录，然后向其中添加一个翻译文件`Translation.csv`，向其中写入以下内容：

```
LanguageCommon/CustomLanguage;"My Language"
```

一般格式为：

```
LanguageCommon/<LanguageID>;"<LanguageDisplayName>"
```

上面那行告诉孤星猎人把新语言添加到语言选项中。其中`<LanguageID>`需要与你在`Translation`目录下创建的文件夹的名称相同。**运行孤星猎人**，就能在设置中找到新语言选项。

![Translation1](../images/Translation1.png)

运行游戏后，孤星猎人将在`Mods/Dev`目录下创建一个名为`TranslationAutoComplete`的文件夹，其中包含缺少翻译词条的文本及其默认的翻译。下一步是将这些文本从英文翻译成新语言，并将翻译内容添加到**你的CSV文件**中。注意：修改`TranslationAutoComplete`中的文件是无效的，它们将在每次运行游戏时刷新。

即使你不完全翻译也是可以的；任何未翻译的文本都会变成英文。但还是推进翻译所有文本。

## 修改/补全现有的翻译

和添加新语言的操作十分类似。在`English`或`ChineseSimplified`文件夹中加入CSV文件，然后向其中添加所需的翻译内容。

你甚至可以把你的文本翻译成另一个模组中添加的语言。只要你的模组中不出现`LanguageCommon/<LanguageID>;"<LanguageDisplayName>"`，该语言就不会出现在语言选择栏中。也就是说你的翻译只在对应的语言模组激活时才会起效。