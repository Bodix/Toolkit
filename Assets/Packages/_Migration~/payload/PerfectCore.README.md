# Perfect Core

The shared foundation every Perfect Core package is built on.

Namespace: `PerfectCore` (editor code: `PerfectCore.Editor`).
Because it sits at the root of the namespace tree, packages such as `PerfectCore.PerfectUI` and
`PerfectCore.PerfectInventory` see these types without any `using` directive.

## What's inside

### Data assets

`DataAsset` is a `ScriptableObject` that carries a stable, human-readable `Id` in `folder:name` form.
The ID is generated on first validation and can be rebuilt from the inspector's context menu
("Regenerate ID"). Use it for anything a save file or a config has to reference by name rather than by
object reference — items, quests, upgrades.

```csharp
public class ItemConfig : DataAsset
{
    [SerializeField] private Sprite _icon;

    public Sprite Icon => _icon;
}
```

`Database<T>` is a `ScriptableObject` list of data assets with `GetById` lookup and duplicate-ID
reporting.

### Event bus

`IEventBus` / `EventBus` — an in-memory, thread-safe publish/subscribe bus. Subscribing, unsubscribing
and nested publishing are all safe during dispatch, and an exception in one handler never stops the
others.

```csharp
eventBus.Subscribe<ItemCollected>(OnItemCollected);
eventBus.Publish(new ItemCollected(item, amount));
```

### Type selector

`[TypeSelector]` turns a `[SerializeReference]` field into a searchable dropdown of every concrete
`[Serializable]` type derived from the field's type. `[TypeSelectorName("...")]` overrides how a type is
labelled in that dropdown.

```csharp
[SerializeField, SerializeReference, TypeSelector]
private List<QuestObjective> _objectives = new List<QuestObjective>();
```

A type shows up in the dropdown when it is public (or public nested), non-abstract, non-generic, marked
`[Serializable]`, and not derived from `UnityEngine.Object`.

### Timer

`Timer` is a `MonoBehaviour` countdown that disables itself while idle, so an unused timer costs
nothing per frame. It ticks on `Update` or `FixedUpdate`, can be paused and resumed, and reports
progress both through callbacks and through events.

```csharp
timer.Start(60f, onComplete: () => Debug.Log("Time is up"));
timer.Pause();
timer.Resume();
```

Perfect UI's `UiTimerText` binds a label straight to one.

### Animation contracts

`IAnimation` and `IShowHideAnimations` are the interfaces UI elements use to play show/hide transitions
without depending on any particular tweening library. Implement them with DOTween, LitMotion, Unity's
own animation system, or plain coroutines.

### Back navigation contracts

`IBackNavigationHandler` and `IBackNavigationService` describe a back-button stack: a handler consumes
the back action and stops it propagating, and the service raises `QuitRequested` when nothing consumed
it. Perfect Core ships only the contracts — the implementation belongs to your game, where the input
system and the scene structure are known.

### Comment

`Comment` is an editor-only note you can attach to a GameObject to explain why it is set up the way it
is. It compiles to nothing in a player build.

## Bundled third-party code

`Dependencies/NaughtyAttributes` — a fork of [NaughtyAttributes](https://github.com/dbrizov/NaughtyAttributes)
by Denis Rizov, MIT licensed. Its assemblies and namespace are renamed to
`PerfectCore.NaughtyAttributes` so that a project which already contains the original keeps compiling.

Note that its `NaughtyInspector` is registered for `UnityEngine.Object` with `editorForChildClasses`,
so it draws the inspector for every MonoBehaviour and ScriptableObject in the project, not just ours.

## Requirements

Unity 2019.3 or newer. No other packages required.

## License

Copyright © 2026 Bogdan Nikolayev. All Rights Reserved.
Bundled third-party code keeps its own license, included alongside it.
