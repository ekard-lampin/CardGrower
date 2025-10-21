v0.0.0-init =======================================================================================
- Setup project.

v0.1.0-world ======================================================================================
- Setup basic map and wall generator.

v0.2.0-player =====================================================================================
- Added input manager.
- Added player movement controller.

v0.3.0-overgrowth =================================================================================
- Added overgrowth generation at map time.

v0.4.0-till =======================================================================================
- Added till method to tiles that removes any non-wall placement and turns the tile into farmland.
- Added farmland material.
- Added visual indicator to represent when the player is looking at a tile.

v0.5.0-cards ======================================================================================
- Added card data structs.
- Added booster pack and shop logic.
- Added card rarity and quality logic.

v0.6.0-deck =======================================================================================
- Added deck view.
- Added player deck to store cards.

v0.7.0-card-select-and-use ========================================================================
- Added card selection logic.
- Added basic card usage logic.

v0.8.0-planting-and-growing =======================================================================
- Added logic to plant seeds.
- Added logic for plants to grow.
- Added logic for plants to be harvested and added to the player's deck.

v0.9.0-selling ====================================================================================
- Added logic to sell cards.
- Added logic to keep track of player finances.
- Added logic to gatekeep purchases beyond player finances.

v0.10.0-menu ======================================================================================
- Added start menu.
- Added options menu.
- Added slider control for basic options.
- Added in booster and tool functionality from accidentally deleted v0.10.0-mvp branch.

v0.11.0-tutorial ==================================================================================
- Added dialogue window logic.
- Added tutorial manager and dialogue manager.
- Added dialogue code infrastructure.
- Added tutorial system to walk the player through game loop.
- Added gatekeeper logic to block certain player actions until they have completed certain tutorial stages.

v0.12.0-intro-cutscene ============================================================================
- Filled out surroundings during game to make the world feel more lifelike.
- Filled out start menu to make it feel more lifelike.
- Added intro cutscene.

v0.13.0-audio =====================================================================================
- Added audio sources and clips for world and player interactions.
- Added cloud system.
- Revamped plant objects to be affected by light.
- Balanced audio and growth times.

v0.14.0-save-and-load =============================================================================
- Added export of player progress to json file.
- Added import of player progress from json file.
- Polished input logic to be more familiar.