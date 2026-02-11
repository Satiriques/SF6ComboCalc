## Description

Allows you to calculate the damage of a combo by passing the numpad notation.   
`calculator "chun-li" "5HK > ss.MK > 28HK"`   
`==> 2520`

This software won't tell you if a combo is possible in game or not.

---

The solution contains 4 projects:

**SF6ComboCalculator** : the code that does the calculation  
**SF6ComboCalculator.Console** : wrapper that allows you to use it with a CLI  
**SF6ComboCalculate.Data** : contains the raw data used to calculate the combo damage  
**SF6ComboCalculate.Tests** : unit tests 

If you want to use it in your .NET program, you only need SF6ComboCalculator and it's dependencies. If you just want to use it in the console, use the Console project, or make your own.

This software is a work in progress.

---
## Usage

## Compiling

You need the dotnet 8.0 sdk.  
Windows:  
Fedora:  `dnf install`

`dotnet build`

## Testing

`dotnet test`

## Contribution

If you want to make some changes, feel free to make a PR.  
A good way to contribute is by adding combos to test in `SF6ComboCalculator.Tests/test_data/2.0111`

### Adding new tests

Every test case added MUST be validated ingame.

## Algorithm

Most of the info can be found at https://wiki.supercombo.gg/w/Street_Fighter_6/Game_Data

