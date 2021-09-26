# FortnitePorting [![Discord](https://discordapp.com/api/guilds/866821077769781249/widget.png?style=shield)](https://discord.gg/bEdZwRJ5Ba)

A tool created to speed up the Fortnite porting process to Blender.

## Installation

To use FortnitePorting, you need to have **[Blender 2.93.0+](https://www.blender.org/download/)** and **[.NET 5.0](https://dotnet.microsoft.com/download/dotnet/thank-you/runtime-desktop-5.0.10-windows-x64-installer)** installed.
* **[Download the latest release](https://github.com/halfuwu/FortnitePorting/releases/latest)**
* Extract all the files to a folder on your computer
* Install the **FortnitePortingAddon.zip** addon in Blender

## Setup

The Blender addon is located in the sidebar of the viewport.
* Navigate to the **FortnitePorting** panel
* Set your *Config File* to the **config.json** file wherever you extracted it
* Set your *Paks Directory* to your Fortnite installation's paks folder
* Press the **Update Keys/Mappings** button

## Usage

### Exporting

* Select the type of [Cosmetic or Item](#export-types) you want to export
* Enter the Name, File Path, or Cosmetic ID
* Press **Launch Exporter**

### Shader Defaults

The **Shader Defaults** are the default values for certain shader options
* Subsurface Intensity
* Subsurface Radius


### Importing
* Choose your Import settings
  * Import Materials
  * Use Reoriented Bones
  * Automatic Parenting
  * Use Quad Topology
* The [Shader Defaults](#shader-defaults) can also be adjusted to your artistic liking 
* Select the **Processed File** of the item you want to import
* Press **Import**

## Export Types

FortnitePorting has many types of cosmetics and files to export

**Name or Cosmetic ID:**
* Characters
* Backblings
* Pets
* Gliders
* Pickaxes
* Emotes

**Name Only**:
* Vehicles
  
**File Path Only:**
* Weapons
* Meshes

