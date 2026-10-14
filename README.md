Here the main point is to experiment with code, so expect some mixing approaches and patterns.
The gist of gameplay: break endless levels of shards (Cell.cs) via mouse slice-motion, get resources for that, spend them on bouncing balls or upgrades, fight incrementaly powerful bosses to get a place on a leaderboard.


https://github.com/user-attachments/assets/27de5d72-0b2b-4b1c-9f46-eaac1ecb8e10


## Tech notes
- not in repo: Odin Inspector, R3, Polymorphic Grid, few art packages
- one of the main goals is to find such code setup so we have some engine-agnostic code + engine code is good enough to support possible creation of experimentation scenes by game-designer or testers and similar in-playmode changes in editor via built-in inspector tools (including runtime changing, removing, adding components + manually constructing, deleting and drag'g'dropping entities)
- 3 asmdefs aka code-layers: Pure C# model + Unity Core "presenter-bridge" + Unity View - the last one suppose to only observe the other two; Core for now sees the internals of Pure, but im  leaning to disabling that
- Runtime nested class is a sign of unity-serialized outer entity
- interactions exist at least for describing parts of gameplay just via filenames - still reflecting how to properly utilize them beyond that
