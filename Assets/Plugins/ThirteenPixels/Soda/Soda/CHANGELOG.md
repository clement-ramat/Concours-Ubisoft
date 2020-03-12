## [1.2.2] - 2019-12-02
- Prevent cyclic/recursive invocation in SodaEvents
- Let ScriptableObject editors display additional serialized fields

## [1.2.1] - 2019-11-08
- Redesigned icons
- Added changelog

## [1.2.0] - 2019-10-26
- Removed the need to pass a reference to the listener to SodaEvent.AddResponse for debugging
- Added uint support to DisplayInsteadInPlaymodeAttribute

## [1.1.0] -
- Enabled editor classes to work for subclasses of their targets, removing the need for editor subclasses
- Changed class creation templates to not include editor classes anymore
- Fixed CreateAssetMenu menuNames for RuntimeSets (including template)
- Added default implementations for GlobalGameObjectWithComponentCacheBase.TryCreateComponentCache and RuntimeSetBase.TryCreateElement
- Added package.json for package manager support
