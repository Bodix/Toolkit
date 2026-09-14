# Perfect Core

The shared foundation every Perfect Core package is built on. It is deliberately small: no third-party
dependencies, no engine modules beyond `UnityEngine`, nothing that a game is forced to adopt.

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

### Animation contracts

`IAnimation` and `IShowHideAnimations` are the interfaces UI elements use to play show/hide transitions
without depending on any particular tweening library. Implement them with DOTween, LitMotion, Unity's
own animation system, or plain coroutines.

### Comment

`Comment` is an editor-only note you can attach to a GameObject to explain why it is set up the way it
is. It compiles to nothing in a player build.

## Requirements

Unity 2019.3 or newer. No other packages required.

## License

Copyright © 2026 Bogdan Nikolayev. All Rights Reserved.
