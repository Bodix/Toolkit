# Rules for working with DOTween

### 1. Always use `.SetLink(gameObject)` when creating a tween.

This way, when deleting an object, you will avoid a situation where the tween tries to access a non-existent object:

```text
The object of type 'Transform' has been destroyed but you are still trying to access it.
Your script should either check if it is null or you should not destroy the object.
```

[Documentation](https://dotween.demigiant.com/documentation.php?api=SetLink)

### 2. If you cache your tween in a variable (for example in `tween` variable) so you can stop it if needed, always add `.OnKill(() => tween = null)`.

This will ensure that the "Recycle Tweens" feature in DOTween works correctly. Do this even if you are not using this feature. If you don't do this and then decide to improve your project's performance by enabling the "Recycle Tweens" feature, you may have difficulty finding all the cached tweens in your project to fix this. Do this right away.

[Documentation](https://dotween.demigiant.com/documentation.php?api=Init)