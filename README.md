# OpenCiv3

**OpenCiv3** is an open-source, standalone-first 4X strategy project built with Godot and C#. It began from an OpenCiv3/C7 compatibility codebase, but the current direction of this fork is to evolve it into an independently maintainable, Civ3-inspired strategy game. Civilization III compatibility remains available as an auxiliary import path rather than the definition of the main product.

***OpenCiv3 is under active development and currently in an early pre-alpha state.*** It is a rudimentary playable game but lacking many mechanics and late-game content, and errors are likely. Keep up with our development for the latest updates and opportunities to contribute!

- [OpenCiv3.org](https://www.openciv3.org)
- [CivFanatics subforum](https://forums.civfanatics.com/forums/civ3-future-development.604/)
- [Discord](https://discord.gg/uwxUuWhM89)

> [!NOTE]
> OpenCiv3 is not affiliated with civfanatics.com,
> Firaxis Games, BreakAway Games, Hasbro Interactive, Infogrames Interactive,
> Atari Interactive, or Take-Two Interactive Software.
> All trademarks are property of their respective owners.

## Status

The latest stable version can be downloaded from the [Releases page](https://github.com/C7-Game/Prototype/releases). Current information on installation and features can always be found on the [project homepage.](https://www.openciv3.org/)

OpenCiv3 is in a pre-alpha state. It is a rudimentary playable game but lacking many mechanics and late-game content, and errors are likely. Keep up with our development for the latest updates and opportunities to contribute.

## Contributing

Find the project interesting and want to contribute?  See [Contributing](https://github.com/C7-Game/Prototype/wiki/Contributing) on our Wiki for more information! At the moment, additional developer support is the most-needed asset, but all sorts of help (art, writing, project management, playtesting) could be useful.

To set up a working development environment, see [Developing and Setting Up IDEs](https://github.com/C7-Game/Prototype/wiki/Developing-and-Setting-Up-IDEs).

For this fork's local planning and engineering documents, start with [doc/README.md](doc/README.md). The current repository branching rules are documented in [doc/git-branching-strategy.md](doc/git-branching-strategy.md).

## What are those subfolders?

- Blast - An Apache-2.0 library for decompressing PKWare DCL, kept as a legacy compatibility dependency for Civ3 BIQ/SAV import
- C7 - The Godot client for the game
- C7Engine - The gameplay engine and AI logic
- C7GameData - The serialized game data model used by saves and runtime conversion
- ConvertCiv3Media - Legacy media import helpers for Civilization III compatibility workflows
- EngineTests - Tests for engine and data logic
- QueryCiv3 - Legacy Civilization III BIQ/SAV reader used by compatibility import paths
