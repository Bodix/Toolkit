# Perfect Core / Perfect UI extraction

This folder ends with `~`, so Unity ignores it completely. Delete it once you are happy with the
result.

## What it does

1. Moves 31 source files out of `Evolunity` into two new packages, **together with their `.meta`
   files** — every GUID survives, so no prefab, scene or asset loses a script reference.
2. Creates `Assets/Packages/PerfectCore` and `Assets/Packages/PerfectUI` (manifests, asmdefs,
   readmes).
3. Renames `Toolkit.Inventory` → `PerfectInventory` and `Toolkit.Quests` → `PerfectQuests`
   (via `git mv`, so the submodules stay registered).
4. Renames every namespace and assembly definition to the `PerfectCore.*` scheme.
5. Adds `PerfectCore` / `PerfectCore.PerfectUI` references to every other assembly that used
   Evolunity, so the rest of the project keeps compiling.
6. Rewrites the `ns:` / `asm:` entries that `[SerializeReference]` data stores in `.asset`,
   `.prefab` and `.unity` files, and fixes the `[MovedFrom]` attributes to point at the identity
   the data was written under.

## How to run it

```powershell
cd D:\Projects\My\Toolkit

# 1. Look at the plan. Nothing is touched.
powershell -ExecutionPolicy Bypass -File .\Assets\Packages\_Migration~\migrate.ps1

# 2. Close Unity, commit or stash your work, then:
powershell -ExecutionPolicy Bypass -File .\Assets\Packages\_Migration~\migrate.ps1 -Apply
```

The script refuses to start if it cannot find `Assets\Packages\Evolunity`, and warns you if
`Temp\UnityLockfile` suggests Unity is still open.

## The resulting layout

| Package | Assembly | Namespace | Depends on |
| --- | --- | --- | --- |
| PerfectCore | `PerfectCore`, `PerfectCore.Editor` | `PerfectCore` | nothing |
| PerfectUI | `PerfectCore.PerfectUI`, `…UI.Editor` | `PerfectCore.PerfectUI` | Perfect Core, TextMeshPro |
| PerfectInventory | `PerfectCore.PerfectInventory`, `…UI`, `…Editor` | `PerfectCore.PerfectInventory` | Perfect Core, Perfect UI |
| PerfectQuests | `PerfectCore.PerfectQuests`, `…UI`, `…Inventory`, `…VContainer`, `…Example` | `PerfectCore.PerfectQuests` | Perfect Core, Perfect UI, Perfect Inventory, VContainer |
| Evolunity | `Evolunity.Runtime`, `Evolunity.Editor` | `Bodix.Evolunity.*` | Perfect Core, Perfect UI |

The core package sits at the root of the namespace tree on purpose: `PerfectCore.PerfectUI` and
`PerfectCore.PerfectInventory` are children of `PerfectCore`, so C# resolves `DataAsset`,
`EventBus`, `TypeSelector` and the animation interfaces without a single `using`.

## What moves

**Into Perfect Core** — `TypeSelectorAttribute`, `TypeSelectorNameAttribute`, `DataAsset`,
`Database<T>`, `EventBus`, `IEventBus`, `IAnimation`, `IShowHideAnimations`, `Comment`, plus the
editor side: `AttributePropertyDrawer`, `TypeSelectorDrawer`, `TypeSelectorDropdown`,
`TypeSelectorUtility`, `SerializedPropertyExtensions`, `CommentEditor`.

**Into Perfect UI** — `UiElement`, `UiElementState`, `IInteractable`, `ObservableButton`,
`UiInteractabilityHandler`, `UiShadowHandler`, the button family (`UiButton`, `UiIconButton`,
`UiTextButton`, `UiIconTextButton`), `UiText`, the toggle family (`UiToggle`, `UiIconToggle`,
`UiIconTextToggle`), `FlexibleLayoutGroup` and its editor.

Everything else stays in Evolunity and now depends on the two new packages.

## Three files are rewritten, not just moved

They carried dependencies the new packages must not inherit. Their `.meta` (and GUID) still move
with them, so nothing breaks:

- **`DataAsset.cs`** — dropped NaughtyAttributes (`[Button]` → `[ContextMenu("Regenerate ID")]`,
  reachable from the inspector's context menu) and wrapped the bare `using UnityEditor;` in
  `#if UNITY_EDITOR`. That `using` was a latent player-build error.
- **`AttributePropertyDrawer.cs`** — `AsString(...)` → `string.Join`, dropping
  `Bodix.Evolunity.Extensions`.
- **`CommentEditor.cs`** — `IsNullOrEmpty()` and `Enum<T>.Parse` → BCL equivalents.

`QuestExampleFiller.cs` also loses its NaughtyAttributes `[Button]` in favour of
`[ContextMenu(nameof(Fill))]`, so the shipped sample has no third-party dependency.

## If something goes wrong

Everything the script touches is under git. `git checkout .` in the root and in each submodule
puts it back. The script never deletes a file — it only moves, renames and rewrites.
