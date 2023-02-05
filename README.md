# Project-Shard-V2
## Design Principles
Project Shard is a work-in-progress digital card game and roguelike. Inspiration has been drawn from the best competitive card games including Magic: The Gathering, Legends of Runeterra, and Hearthstone as well as deck builders such as Slay the Spire and Dominion. The core design principles are as follows:
- Gameplay is optimized for a single player experience
- Despite being a card game, the digital space is fully embraced
- Content bloat is kept to a minimum
  - Fewer, but more synergistic, cards in the player deck
  - Fewer cards in play
  - Minimal emphasis on phases and priority (e.g. Hearthstone model)
- Summonable units are incorporated without compromising "content bloat"
- NPC AI is at the forefront of design, incorporating cutting edge techniques if (and only if) they are useful
- There is a broad space for player exploration
- Fantasy setting with many different lore-rich sub settings
- Stained glass aesthetic for cards and UI

## Mechanics

### Deck Cycling
The game is tuned to ensure that the player cycles through their deck multiple times in an encounter. 
- Instead of drawing a single card every turn, the player cycles their whole hand and draws a new one
  - Mechanics exist to allow a player to "memorize" cards and keep them in their hand between turns
- Each time the player cycles through their deck, their strength increases
  - More resources are available to play cards
    - Card draw becomes a form or resource ramp
  - More cards in their deck become available
- If the player's deck increases beyond a certain size (~12 cards), some cards will randomly be remove from their deck in the first few cycles
  - Allows the player to not be excessively punished for having a larger deck
  
### Card Cycling
Unlike many card games, the hand is discarded each turn and a new hand is drawn. This feature leads to many interesting side effects:
- Cards are usually "use or lose" so the player is incentivised to build a highly synergistic deck
  - Some cards will be allowed to be "memorized" so as not to completely disallow the concept of "combo" strategies
- Card draw (while still powerful) isn't as strong as in traditional card games
In order to make this system less punishing, many cards (particularly "late game" cards) will also have a "cycle" power which triggers if the card is discarded at the end of the player's turn. 

### Unit Influence
Units are powerful cards that stay on the field and provide the player with offense, defense, and utility. In order to balance units' power with respect to single-use cards like spells, units have their own associated resource called Influence. At the end of each turn, if the player doesn't have enough influence over their units, units must be discarded from play. This mechanic has the following properties:
- Prevents the play field from becoming too cluttered with units
- Requires the player to think about what extent units are part of their deck strategy
- The player must choose between deploying many small units or just a couple large or synergistic units
