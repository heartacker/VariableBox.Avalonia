# VariableBox

<p align="center">

Wanna help

> 欢迎任何人士帮忙支持并让这个简单控件变得更好，我们需要你们。

> welcome any one to help and make this simple control better, we need you.

</p>

[![GitHub stars](https://img.shields.io/github/stars/heartacker/VariableBox.Avalonia?style=for-the-badge)](https://github.com/heartacker/VariableBox.Avalonia)
[![GitHub release](https://img.shields.io/github/v/release/heartacker/VariableBox.Avalonia?style=for-the-badge)](https://github.com/heartacker/VariableBox.Avalonia/releases)
[![Nuget](https://img.shields.io/nuget/v/VariableBox.Avalonia?style=for-the-badge)](https://www.nuget.org/packages/VariableBox.Avalonia)
![Nuget](https://img.shields.io/nuget/dt/VariableBox.Avalonia?style=for-the-badge)
[![Nuget](https://img.shields.io/nuget/v/VariableBox.Avalonia.Themes.Semi?style=for-the-badge)](https://www.nuget.org/packages/VariableBox.Avalonia.Themes.Semi)
![Nuget](https://img.shields.io/nuget/dt/VariableBox.Avalonia.Themes.Semi?style=for-the-badge)

<p align="center">
    <img src="./assets/light_demo.png" alt="drawing" width="150" />
</p>

VariableBox is a UI library for building cross-platform UIs with Avalonia UI.

![Demo](./assets/light_demo.png)

## Feature

### NumericalUpDown

- all numerical type support
- spinning updown support
- get (read) /set (write) support
- rich formatting support like `hex`, `dec` and `bin`
- drag support, you can use mouse to drag
- mouse scroll support
- shortcut and arrow key support
  - <kbd>Esc</kbd> for cancel editing
  - <kbd>Enter</kbd> for trigger
  - <kbd>up</kbd> for increase
  - <kbd>down</kbd> for decrease
  - <kbd>alt+left</kbd> for read
  - <kbd>alt+right</kbd>/<kbd>alt+enter</kbd>for trigger (force) write
- identify support
  - `*` for editing
  - **red** <font color=red>*</font> for error input
  - **green** <font color=green>*</font> for right input

### EnumerationUpDown

- [ ] todo

## How to use

### VariableBox

Add nuget package:

```bash
dotnet add package VariableBox.Avalonia
```

You can now use Ursa controls in your Avalonia Application.

```xml
<Window
    ...
    xmlns:v="VariableBox"
    ...>
    <StackPanel Margin="20">
        <v:VariableBoxUInt Value="{Binding Value}" 
            FormatString="X8"
            HeaderContent="0x"
            ParsingNumberStyle="AllowHexSpecifier"
            Step="2"
            IsEnableEditingIndicator="True"
            />
    </StackPanel>
</Window>
```

### VariableBox.Avalonia.Themes.Semi

To make Ursa controls show up in your application, you need to reference to a theme package designed for VariableBox.

- `VariableBox.Avalonia.Themes.Semi` is a theme package for Ursa inspired by Semi Design.
   >you need to `add package Semi.Avalonia` frist

- also `VariableBox.Avalonia.Themes.Semi` is compatible with `<SimpleTheme/>`
- `VariableBox.Avalonia.Themes.Semi` is NOT compatible with `<FluentTheme/>`

You can add it to your project by following steps.

1. Add nuget package:

```bash
dotnet add package Semi.Avalonia
dotnet add package VariableBox.Avalonia.Themes.Semi
```

2. Include Styles in application:

```xml
<Application...
    xmlns:v-semi="using:VariableBox.Avalonia.Themes.Semi"
    ....>

    <Application.Styles>
        <!-- compatible theme -->

        <!-- 1. SimpleTheme -->
        <SimpleTheme/>

        <!-- 2. Semi, recommand -->
        <StyleInclude Source="avares://Semi.Avalonia/Themes/Index.axaml" />

        <!-- set this theme -->
        <v-semi:SemiTheme Locale="zh-CN"/>
    </Application.Styles>
```
