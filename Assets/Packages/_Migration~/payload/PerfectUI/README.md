# Perfect UI

Basic uGUI building blocks: elements that know how to show and hide themselves, the button/toggle/text
family built on top of them, and a grid layout group that can size its cells to fit.

Namespace: `PerfectCore.PerfectUI` (editor code: `PerfectCore.PerfectUI.Editor`).

## UiElement

`UiElement` is the base of every element here. It owns a small state machine — `Hidden`, `Showing`,
`Shown`, `Hiding` — and plays an animation on each transition:

```csharp
element.Show();              // plays the show animation
element.Hide(OnHidden);      // plays the hide animation, then invokes the callback
element.ShowInstantly();     // skips the animation
element.Toggle(isOn);
```

Events: `Showing`, `Shown`, `Hiding`, `Hidden`. State: `IsShown`, `IsHidden`, `IsInTransition`,
`IsShownAndActiveInHierarchy`.

Animations are supplied through the `Animations` field — a `[SerializeReference]` of
`IShowHideAnimations` (from Perfect Core) picked in the inspector via a type dropdown. Perfect UI does
not depend on any tweening library: implement the interface with whatever you already use. With no
animation assigned, transitions are instant.

## Elements

| Element | What it adds |
| --- | --- |
| `UiText` | A `TMP_Text` reference |
| `UiButton` | A `Button`, a background `Image` and `IInteractable` |
| `UiIconButton`, `UiTextButton`, `UiIconTextButton` | Icon and/or text on top of `UiButton` |
| `UiToggle` | A `Toggle` and a background `Image` |
| `UiIconToggle`, `UiIconTextToggle` | Icon and/or text on top of `UiToggle` |

## Interactability

`IInteractable` exposes `IsInteractable` plus an `InteractabilityChanged` event, so visuals can react to
a button becoming disabled instead of polling it.

`ObservableButton` is a `Button` that raises that event when Unity changes its interactable state
internally — assign it instead of the stock `Button` when you want the event to fire in every case.

`UiInteractabilityHandler` is the base for components that respond to those changes;
`UiShadowHandler` is the ready-made one, toggling a `Shadow` with the interactable state.

## FlexibleLayoutGroup

A `GridLayoutGroup` whose cell size can be derived instead of fixed. Each axis picks its own mode:

- **Fixed** — use `cellSize` as authored.
- **Expand** — divide the available space by the configured column/row count, minus padding and spacing.
- **RowSize / ColumnSize** — mirror the other axis, which is how you keep cells square.

Asking both axes to mirror each other is a circular dependency; in that case both fall back to the
authored `cellSize`. The component writes the computed size through the private backing field, so
driving a layout never leaves prefab overrides behind.

## Requirements

Unity 2019.3 or newer, TextMeshPro, and Perfect Core.

## License

Copyright © 2026 Bogdan Nikolayev. All Rights Reserved.
