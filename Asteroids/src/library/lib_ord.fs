namespace library

module Types =
    type Size =
        | Large
        | Medium
        | Small
        
    type Lives =
        | Three
        | Two
        | One

    type Ship = {
        Pos: float * float
        Vel: float * float
        Ang: float
        Size: float    // el radio
    }

    type Asteroid = {
        Pos: float * float
        Vel: float * float
        Size: Size
    }

    type Bullet = {
        Pos: float * float
        Ang: float
        Range: float
    }

    type Dir =
        | Left
        | Right

    type Saucer = {
        Pos: float * float
        Dir: Dir
        Size: float
    }

    type Rotation =
        | Positive
        | Negative
        | Zero

    type Input = {
        Thrust: bool
        Shoot: bool
        Rot: Rotation
    }

    type Game = {
        Ship: Ship
        Asteroids: Asteroid list
        Bullets: Bullet list
        Saucer: Saucer option
        Score: int
        Lives: Lives
    }

module Constants =
    let fps = 24
    let aspect_ratio = 1.5
    let rand = System.Random()   // inicializo instancia de Random()
    let maxAsteroidVel = 20.0

module Utility =
    open Types
    open Constants

    let sizeMap (size: Size) = 
        match size with
        | Large -> 0.04
        | Medium -> 0.02
        | Small -> 0.01

    let min_abs x y = 
        min (abs x) (abs y)

    let distance (pos1: float * float) (pos2: float * float) = 
        let dx1 = fst pos1 - fst pos2
        let dy1 = snd pos1 - snd pos2

        let dx2 = min_abs dx1 (aspect_ratio - dx1)
        let dy2 = min_abs dy1 (1.0 - dy1)
        Math.Sqrt(dx2 ** 2.0 + dy2 ** 2.0)

    let cartesianToPolar (vec: float * float) =
        let (x, y) = vec                               
        let r = Math.Sqrt(x ** 2.0 + y ** 2.0)
        let theta = Math.Atan2(y, x)
        r, theta

    let polarToCartesian (r: float) (theta: float) =
        let x = r * Math.Cos(theta)
        let y = r * Math.Sin(theta)
        (x, y)

    let trueModulo (x: float) (modulo: float) =
        let y = x % modulo
        match y with
        | _ when y < 0.0 -> y + modulo
        | _ -> y

module Collision =
    open Types
    open Utility

    let checkCollisionShipAsteroid (ship: Ship) (asteroid: Asteroid) = 
        let shipAstDistance = distance ship.Pos asteroid.Pos
        let radiiSum = sizeMap asteroid.Size + ship.Size
        shipAstDistance < radiiSum

    let checkCollisionShipBullet (ship: Ship) (bullet: Bullet) =
        let shipBulletDistance = distance bullet.Pos ship.Pos
        shipBulletDistance < ship.Size

    let checkCollisionBulletAsteroid (bullet: Bullet) (asteroid: Asteroid) =
        let bulletAsteroidDistance = distance bullet.Pos asteroid.Pos
        bulletAsteroidDistance < sizeMap asteroid.Size

    let checkCollisionShipSaucer (ship: Ship) (saucer: Saucer option) = 
        match saucer with
        | None -> false
        | Some s -> 
            let shipSaucerDistance = distance ship.Pos s.Pos
            let radiiSum = s.Size + ship.Size
            shipSaucerDistance < radiiSum

    let checkCollisionSaucerBullet (saucer: Saucer option) (bullet: Bullet) =
        match saucer with
        | None -> false
        | Some s ->
            let saucerBulletDistance = distance bullet.Pos s.Pos
            saucerBulletDistance < s.Size

module Gameplay =
    open Types
    open Constants
    open Utility
    open Collision

    let nextLives (life: Lives) =
        match life with
        | Three -> Two
        | Two -> One

    let astSplit (ast: Asteroid) = 
        let spawn2RandAsts (pos: float * float) (size: Size) =

            let randVel () : float * float =
                let r = rand.NextDouble() * maxAsteroidVel
                let ang = float (rand.Next(16)) * Math.PI / 8.0
                polarToCartesian r ang

            let vel1 = randVel ()
            let vel2 = randVel ()
            let randAsts : Asteroid list = [{ Pos = pos; Vel = vel1; Size = size };
                                            { Pos = pos; Vel = vel2; Size = size }]
            randAsts

        match ast.Size with
        | Large -> spawn2RandAsts ast.Pos Medium
        | Medium -> spawn2RandAsts ast.Pos Small 
        | Small -> []

    let moveBullet (bullet: Bullet) = 
        let Vx = -bulletsVelocity * Math.Cos(bullet.Ang)
        let Vy = -bulletsVelocity * Math.Sin(bullet.Ang)

        let newPosX = trueModulo ((fst bullet.Pos) + Vx / (float fps)) aspect_ratio
        let newPosY = trueModulo ((snd bullet.Pos) + Vy / (float fps)) 1.0
        let newPosition = (newPosX, newPosY)
        
        let newRange = bullet.Range + bulletsVelocity / (float fps)

        let newBullet = 
            { bullet with
                Pos = newPosition
                Range = newRange
            }
        newBullet

    let moveAndClearBullets (bullets: Bullet list) = 
        bullets
        |> List.map moveBullet
        |> List.filter (fun b -> b.Range <= maxBullRange)

module Actions =
    open Types
    open Constants
    open Utility
    open Gameplay

    let accelerateShip (ship: Ship) (input: Input) =
        let shipAcc = 1.0 / (float fps)
        let shipDesacc = 0.5 / (float fps)
        let shipMaxVel = 20.0 / (float fps)
        
        let velocityDelta (ship: Ship) (thrust: bool) =
            let (velR, velTheta) = cartesianToPolar ship.Vel
            match thrust with
            | true -> (shipAcc * Math.Cos(ship.Ang), shipAcc * Math.Sin(ship.Ang))
            | false -> (-shipDesacc * Math.Cos(velTheta), -shipDesacc * Math.Sin(velTheta))
        
        let velDelta = velocityDelta ship input.Thrust

        let newVelocity =
            let velFinalUnnorm = tupleAdd ship.Vel velDelta
            match input.Thrust with
            | true ->   match cartesianToPolar velFinalUnnorm with
                        | (r, _) when r <= shipMaxVel -> velFinalUnnorm
                        | _ -> velFinalUnnorm |> renormalizeVelocity
            | false ->
                        match cartesianToPolar velDelta with
                        | (r, _) when r = 0.0  -> (0.0, 0.0)
                        | _ -> velFinalUnnorm

        newVelocity

    let shootBullet (ship: Ship) (bullets: Bullet list) (input: Input) = 
        let fire (ship: Ship) (bullets: Bullet list) =
            let newBullet = 
                {
                    Pos = (fst ship.Pos + ship.Size * Math.Cos(ship.Ang),
                            snd ship.Pos + ship.Size * Math.Sin(ship.Ang))
                    Ang = ship.Ang
                    Range = 0.0
                }
            bullets @ [newBullet]
        
        match input.Shoot with
        | true -> fire ship bullets
        | false -> bullets

module GameLogic =
    open Types
    open Constants
    open Utility
    open Gameplay

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
                    | _ -> Playing { game with Lives = nextLives ship.Lives }

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