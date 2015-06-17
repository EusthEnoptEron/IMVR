# IMVR

This project aims to create a tool that allows managing media files -- mainly pictures -- in a way that leverages the strengths of both the Leap Motion and the Oculus Rift.

## File Structure

- `dist`: distribution folder containing binaries
- `doc`: contains documentation
- `notes`: miscellaneous notes made during development
- `src`: contains the Unity source code

## Installing

Before you start, clone the repository to your HDD.

### Building AuxiliaryTools

The AuxiliaryTools can be compiled without a problem on any modern computer that has .NET Framework 4 installed.

1. Open `src\AuxiliaryTools\AuxiliaryTools.sln`
2. Select `Release` configuration
3. Build Solution

### Indexing Media Library

In order to have some data displayed, you'll need to index your music files first:

1. Open / Create `dist\library.conf`
2. Add absolute paths to your media folders
3. Run `IMVR_Indexer.exe`
4. (Optional) Copy `dist\IMVR_Data\StreamingAssets\IMDB.bin` to `src\Application\Assets\StreamingAssets\IMDB.bin`

### Building IMVR

The main application (made with Unity) is a bit trickier to build. Unity 5 must be installed (tested with 5.0.0f4).

1. Download the [large files](https://github.com/EusthEnoptEron/IMVR/releases/download/v1.0/Assets.zip)
2. Extract contents into `src\Application\Assets`
3. Launch Unity
4. Open `src\Application`
5. Load scene `Scenes\Galaxy.unity`
6. Run in extended mode
7. (Optional) Build the project into `dist`