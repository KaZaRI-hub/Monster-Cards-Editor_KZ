Monster Card Editor_KZ

This editor lets you modify and save MonsterCards .card text, images, BGM, and JSON, with layer previews and image post-processing tools.
This tool is not intended to interfere with the game's reroll system, so card attack power, race, skills, and similar gameplay values cannot be edited.
DLC features are paid game content and are intentionally not editable. Please set DLC-related options when creating the card in-game.

[Main Features]
- Open MonsterCards .card files and inspect or edit internal text, image bytes, BGM bytes, and advanced JSON.
- Select ES3Defaults.asset to reference game save settings. The default password setting is also supported.
- Edit card file name, card name, main text fields, and flavor text.
- Attack power, race, skills, and DLC-related values are excluded from editing to respect game balance and paid content.
- Generate flavor text automatically.
  Format: RACE / ELE / DESC / Owner
  Owner uses the creatorName field first.
- View linked target information in the Special Link tab.
- Filter the card list.
  Toggle rarity sorting or name sorting for Korean, English, and Japanese text.
- Replace or export image1Bytes, image2Bytes, and image3Bytes.
- Replace or export BGM bytes.
- Replaced images are handled from the original PNG bytes. When no edits are active, the original bytes are preserved without unnecessary re-encoding.
- Shared image controls are available for image layers 1/2/3.
  DOT styles: 512, 258, 128, 64, 32, 16
  Chromatic shift, Hue, Saturation, center-based Scale
- Each image layer can have its own Post 1 / Post 2 / Post 3 effects.
  Lens Flare, Bloom, Depth of Field, Blur, Film Grain, Noise, Vignette, Contrast,
  Monochrome, CRT Scanlines, Pixelation, Halftone, Glitch, RGB Split, TV Static, Interlace
- Image preview popup is supported.
  image3 is the background, image2 is the middle layer, and image1 is the top layer.
- Card frame preview toggle is supported.
  The image display area remains visible, while the outer frame area is shown with an 80% gray overlay.
- View and apply the full JSON in the Advanced JSON tab.
- On save, the .card file is encrypted/compressed again and verified by decrypt/decompress/JSON parse.
- After saving a card, the Scale value automatically returns to 100%.

[Basic Usage]
1. Create a card in-game.
2. Enter the card name, leave the artwork blank, and save it.
3. Launch the editor and select the card folder or use the default folder.
4. Select ES3Defaults.asset if needed.
5. Choose the .card file from the card list.
6. Edit images, descriptions, BGM, post-processing, or JSON.
7. Press Save Card to write the .card file.

[Notice]
- Always back up the original .card file before editing.
- Attack power, race, skills, and DLC features cannot be edited with this tool.
- The creator is not responsible for any disadvantage caused by editing .Json data.
- Please avoid actions that may offend others or break fairness for everyone.
- This is an unofficial editor. It may stop working if a game update changes the save format.
- The creator is not responsible for any disadvantage caused by using this tool.
