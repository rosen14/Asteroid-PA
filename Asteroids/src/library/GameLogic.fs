namespace library

open System
open Types
open Constants
open Utility
open Dynamics
open Collision
open Actions

module GameLogic =

    let startGame (game: Game) =
        let newShip =
            { game.Ship with
                Pos = (0.5, aspect_ratio / 2.0)
                Ang = 0.0
                Vel = (0.0, 0.0)
            }
        Playing {   
            Ship = newShip
            Asteroids = []
            Bullets = []
            Saucer = None
            Score = 0
            Lives = Three
        }

    let getScoreByAsteroids (asteroid: Asteroid) =
        match asteroid.Size with
        | Small -> 200
        | Medium -> 100
        | Large -> 50

    let spawnAsteroids (game: Game) (num: int) = 
        // Spawn asteroids at the start of the level
        // num: number of asteroids to spawn
        let randAst ()=
            // Generate a random Large asteroid located at the edges of the screen
            let x = trueModulo ((rand.NextDouble() - 0.5) * aspect_ratio / 2.0) aspect_ratio
            let y = trueModulo ((rand.NextDouble() - 0.5) * 0.5)  1.0
            let theta = float (rand.Next(16)) * Math.PI / 8.0
            let vel = polarToCartesian (maxAsteroidVel * 0.5) theta 
            { Pos = (x, y); Vel = vel; Size = Large }

        let newAsts = List.init num (fun x -> randAst())
        { game with Asteroids = game.Asteroids @ newAsts }

    let spawnSaucer (game: Game) = 
        let saucerSize = 0.1

        let randInBand fraction = 
            // genera número random en una franja de ancho "fracion" centrada en 0.5
            (rand.NextDouble() - 0.5) * fraction + 0.5

        let newSaucer = 
            match rand.Next(2) with
            | 0 -> {Pos = (0.0, randInBand 0.9); Dir = Right; Size = saucerSize}
            | 1 -> {Pos = (aspect_ratio, randInBand 0.9); Dir = Left; Size = saucerSize}
        {game with
            Saucer = Some newSaucer}
            
    let updateShip (game: Game) (input: Input) = 
        let ship = game.Ship
        let asts = game.Asteroids
        let bulls = game.Bullets
        let saucer = game.Saucer
        let astShipColls =
            asts
            |> List.filter (fun x -> checkCollisionShipAsteroid ship x)
        let bullShipColls =
            bulls
            |> List.filter (fun x -> checkCollisionShipBullet ship x)

        let saucerShipColls = checkCollisionShipSaucer ship saucer

        let aliveStatus = (astShipColls = [] && bullShipColls = [] && not saucerShipColls)
        let newVel = accelerateShip ship input
        let newPos = newPosShip ship
        let newAngle = trueModulo (ship.Ang + deltaAngle input) (2.0 * Math.PI)
        
        match aliveStatus with
        | true ->   let newShip = { ship with Pos = newPos; Vel = newVel; Ang = newAngle }
                    Playing { game with Ship = newShip }
        | false ->  match game.Lives with
                    | One -> Gameover
                    | _ -> Playing { game with Lives = nextLives game.Lives }

    let updateBullets (game: Game) (input: Input) =
        let ship = game.Ship
        let astList = game.Asteroids
        let bullList = game.Bullets
        let saucer = game.Saucer

        let saucerBullets = saucerShoot saucer bullList           // Bullets fired by the enemy saucer
        let shipBullets = shootBullet ship bullList input         // Bullets fired by the player
        let newBullets = 
            saucerBullets @ shipBullets
                |> List.filter (fun x -> isBullDestroyed x astList = false)         // Destroy bullet on collision with asteroid
                |> List.filter (fun x -> checkCollisionShipBullet ship x)      // Destroy bullet on collision with ship
                |> List.filter (fun x -> checkCollisionSaucerBullet saucer x)  // Destroy bullet on collision with saucer
                |> moveAndClearBullets      // Move remaining bullets and clear those that exceeded maximum range
        { game with Bullets = newBullets }

    let updateAsteroids (game: Game) =
        let ship = game.Ship
        let astList0 = game.Asteroids
        let bullList = game.Bullets

        // Separate destroyed asteroids from those still alive
        let astDeadList, astAliveList =
            astList0
            |> List.partition (fun x -> isAstDestroyed bullList x )

        // Split destroyed asteroids; remove if small, split into two smaller ones otherwise
        let astSplitList =
            astDeadList
            |> List.collect (fun x -> astSplit x)
        
        let earnedScore =
            match astDeadList with
            | [] -> 0
            | _ -> astDeadList
                    |> List.map (fun x -> getScoreByAsteroids x)
                    |> List.sum

        let astList1 = astAliveList @ astSplitList
        let newAsts =
            astList1
            |> List.map (fun x -> {x with Pos = newPosAsteroid x})
        { game with Asteroids = newAsts; Score = game.Score + earnedScore }

    let updateSaucer (game: Game) =
        let saucer = game.Saucer

        let bullSaucerColls =
            game.Bullets
            |> List.filter (fun x -> checkCollisionSaucerBullet saucer x)

        let newSaucer = 
            match saucer with
            | Some s when List.isEmpty bullSaucerColls -> Some { s with Pos = moveSaucer s }
            | _ -> None

        let earnedScore = 
            match List.length bullSaucerColls with
            | 0 -> 0
            | _ -> 200

        { game with Saucer = newSaucer; Score = game.Score + earnedScore }

    let checkLevelFinished (game: Game) =
        // Check if the level is finished 
        let asts = game.Asteroids
        let saucer = game.Saucer
        (asts = [] && Option.isNone saucer)

    let checkGameOver (gamestate: Gamestate) = 
        // Check game over
        match gamestate with
        | Gameover -> true
        | Playing _ -> false