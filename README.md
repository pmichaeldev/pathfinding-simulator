# ABOUT
A Unity3D game that simulates path finding using various algorithms.

Developed by Philip Michael.


# COMPILATION
This game requires Unity 5 or higher for smooth compilation and run.

Simply open MainScene.unity and press Play if you'd like to test it.

Otherwise, please build it for Desktop. This is not compatible with mobile or tablet platforms.


# SCENES
This game consists of two scenes. The default scene is the MainScene, where you simply click to move to a node based on algorithm choice.

The second scene is the ChaseScene. This scene simulates a character being chased by another. 

For controls, please refer below.


# CONTROLS
The game will load at a default algorithm state.

To change it, please follow the on-screen instructions, i.e.:

- Press '1' to change the pathfinding algorithm to use Dijkstra's.

- Press '2' to change the pathfinding algorithm to use A* with Euclidean Distance heuristic.

- Press '3' to change the pathfinding algorithm to use a Cluster algorithm that uses A* with Euclidean Distance heuristic.

- Press 'T' to change the floor's appearance from the default of showing the cluster zones, to simply the textured flooring.

In order to set a goal node, please click anywhere in the main scene's level and it will find the closest node to your click.


What is not mentioned in the in-game overlay is to press the following to change scenes (or levels):

- 'Q' to load (or reload) the MainScene without chasing.

- 'W' to load (or reload) the ChaseScene which includes chasing.


# ASSETS CREDITS

- Knight: https://assetstore.unity.com/packages/3d/characters/humanoids/strong-knight-83586

- Skybox: https://assetstore.unity.com/packages/2d/textures-materials/sky/skybox-4183

- Textures: https://assetstore.unity.com/packages/2d/textures-materials/brick/high-quality-bricks-walls-49581