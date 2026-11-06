# Slot Machine

## Game Overview
- A simple Slots Machine game. Select the amount of gold to place as a bet and pull the lever. Get more gold from winning combinations.

- Starting balance is 1000G. Game is over once balance becomes insufficient (less than 10G).

- Press the button labelled "R" to reset the slot reels to starting position.

## WebGL Build Instructions
The WebGL build of this game can be played on Unity Play: https://play.unity.com/en/games/34c6c6c1-49d5-4685-8f9a-90f872d2e721/slot-machine

Recommended to play game in Full Screen.

## Bonus Features
- New symbols on the reels other than the ones provided

- Reset position button

- Bonus UI features

## Thought Process and Approach
I first setup the required sprites and UI into the scene along with animators wherever required.

Then I created the scripts to implement interaction, behaviour and game logic. This included interaction with the pull lever (with animation), buttons and popups/dialog boxes, how the objects should move on the reels, UI updates to manage and display balance, machine states and messages, and game logic to calculate payouts based on the combination achieved.

I then added the symbols as scriptable objects that will appear in a randomized sequence on the reel stops (with fixed weights/number of times each symbol can appear), and possible winning combinations along with payout multipliers for each. These payout multipliers are adjusted for each combo to be fair and ensure ~96% RTP.

Finally, I fixed a few bugs that occurred with message display and reel movement, and added a pay table display.
