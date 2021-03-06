# unity-game-playground
Sample project (almost a "game") created in 4 days inside Unity.

On master there's attempt to implement a working multiplayer aspect of the game.
While photonless branch has a significant less build size (resulting in faster game load & better fps).

## Development

Boot it up in Unity and you are free to develop!
Made with Unity in version 2019.4 - please use that one or higher.

## Build & Running

Build with WebGL inside Unity. Or just use what's inside *Build* directory in this repository.
Once you decide to run the game, you need to host via server of some sort, then visit using client that has WebGL supporting (ie. Chrome). 

It can be done using ie. "http-server":

```
npm install -g http-server
cd Build
http-server --port 8080
```

And your game is running on http://127.0.0.1:8080 !

For something like "production" I highly suggest using **at least** pm2 to ensure "http-server" is being restarted on any failure:

```
npm install -g pm2
pm2 start $(which http-server) --name unity-game-playground -- --port 8080 -d false
```


## Example

Latest singleplayer version: http://51.77.59.187:8881

Latest multiplayer: http://51.77.59.187:8882

Game hosted on above links might be outdated, since I won't update it regulary (or it also might be down! :D)


