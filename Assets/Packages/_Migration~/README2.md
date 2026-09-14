# Second migration pass

Run `migrate.ps1` first. This one continues from where it stopped.

```powershell
cd D:\Projects\My\Toolkit

# 1. Look at the plan. Nothing is touched.
powershell -ExecutionPolicy Bypass -File .\Assets\Packages\_Migration~\migrate2.ps1

# 2. Close Unity, commit your work, then:
powershell -ExecutionPolicy Bypass -File .\Assets\Packages\_Migration~\migrate2.ps1 -Apply
```

## What it does

**Into Perfect UI** — everything left in `Scripts/Runtime/Components/UI/Elements`: `UiSlider`,
`UiProgressBar`, `UiQuantitySlider`, `UiQuantitySelector`, `UiFoldable`, `UiTabsGroup`,
`UiConfirmationDialog` (+ its payload), `UiQuantityText`, `UiTimerText`, `UiEnumIndicator`,
`UiProcessIndicator`. Plus the whole `Prefabs/UI` folder, `Dependencies/uLayout`, and the art those
prefabs need: `Fonts/Rubik`, `Textures/Cirlces`, `Textures/Icons`.

**Into Perfect Core** — `Dependencies/NaughtyAttributes` (forked, see below), `Timer` (because
`UiTimerText` drives one), and the back-navigation contracts `IBackNavigationHandler` /
`IBackNavigationService` (because `UiTabsGroup` and `UiConfirmationDialog` implement them).

**Nothing above `Elements` moves.** `SafeArea`, `ScrollRectViewportFix`, `ImageAspectRatioFitter`,
`DisableButtonAfterClick`, `GifImage`, `FpsCounter` and the whole `Interpolators` folder stay in
Evolunity, as asked.

Finally it deletes every folder left empty in Evolunity, together with its `.meta`.

## The NaughtyAttributes fork

Assembly and namespace are renamed:

| Before | After |
| --- | --- |
| `NaughtyAttributes.Core` | `PerfectCore.NaughtyAttributes` |
| `NaughtyAttributes.Editor` | `PerfectCore.NaughtyAttributes.Editor` |
| `NaughtyAttributes.Test` | `PerfectCore.NaughtyAttributes.Test` |
| `namespace NaughtyAttributes` | `namespace PerfectCore.NaughtyAttributes` |

Without this, a customer who already has NaughtyAttributes — it is free, popular, and vendored inside
plenty of other assets — would get a duplicate assembly name and a project that does not compile.
The license is MIT, so the fork is allowed; keep the attribution.

Every `using NaughtyAttributes;` in the project and every asmdef reference is updated to match.

## Two things to check afterwards

1. **`NaughtyInspector` is global.** It is registered for `UnityEngine.Object` with
   `editorForChildClasses`, so shipping it inside Perfect Core means every buyer gets NaughtyAttributes'
   inspector for every component in their project. That is the trade-off of bundling it.

2. **`NaughtyAttributes` has no LICENSE file** in this copy — only `README.html`. MIT requires the
   license text to travel with redistributed code. Add `LICENSE` (MIT, Copyright © Denis Rizov) into
   `PerfectCore/Dependencies/NaughtyAttributes/` before publishing.

## Also worth knowing

- `uLayout` keeps its assembly name `uLayout`, so the same collision is possible there, just far less
  likely. Say the word and the next pass forks it the same way.
- `Aspect Ratio Bounds.asset` stays in `Evolunity/Prefabs/UI`: its script lives in `Interpolators`,
  which is not moving. No prefab references it.
- `Fonts/Fredoka` stays in Evolunity — no prefab uses it.

## If something goes wrong

Everything is under git. `git checkout .` in the root and in each submodule puts it back. The script
never deletes a file — it only moves, renames, rewrites, and removes folders that became empty.
