namespace library.Tests

open NUnit.Framework
open library.Types
open library.GameLogic
open library.Constants
open library.Dynamics

[<TestFixture>]
module GameLogicTest =
    
    [<Test>]
    let ``spawn 4 large asteroides`` () =
        let game = {
            Ship = {Pos = (0.5, aspect_ratio/2.); Vel = (0., 0.); Ang = 0.; Size = 0.1}
            Asteroids = []
            Bullets = []
            Saucer = Some {Pos = (0.1, 0.5); Dir = Left; Size = 0.1}
            Score = 0
            Lives = Three
        }
        let newGame = spawnAsteroids game 4
        let nroAsts = newGame.Asteroids.Length
        Assert.AreEqual(nroAsts, 4)

    [<Test>]
    let ``updateShip gameover by asteroid collision`` () =
        let game = {
            Ship = {Pos = (0.1, 0.5); Vel = (0., 0.); Ang = 0.; Size = 0.1}
            Asteroids = [{Pos = (0.1, 0.5); Vel = (0.1, 0.2); Size = Large}]
            Bullets = []
            Saucer = None
            Score = 0
            Lives = One
        }
        let input = {Thrust = false; Shoot = false; Rot = Negative}
        
        let newGameState = updateShip game input

        let expectedGameState = Gameover
        Assert.AreEqual(expectedGameState, newGameState)

    [<Test>]
    let ``updateShip gameover by bullet collision`` () =
        let game = {
            Ship = {Pos = (0.1, 0.5); Vel = (0., 0.); Ang = 0.; Size = 0.1}
            Asteroids = []
            Bullets = [{ Pos = (0.1+0.05, 0.5); Ang = 0.0; Range = 10.0 }]
            Saucer = None
            Score = 0
            Lives = One
        }
        let input = {Thrust = false; Shoot = false; Rot = Negative}
        
        let newGameState = updateShip game input

        let expectedGameState = Gameover
        Assert.AreEqual(expectedGameState, newGameState)

    [<Test>]
    let ``updateShip gameover by saucer collision`` () =
        let game = {
            Ship = {Pos = (0.1, 0.5); Vel = (0., 0.); Ang = 0.; Size = 0.1}
            Asteroids = []
            Bullets = []
            Saucer = Some {Pos = (0.1+0.199, 0.5); Dir = Left; Size = 0.1}
            Score = 0
            Lives = One
        }
        let input = {Thrust = false; Shoot = false; Rot = Negative}
        
        let newGameState = updateShip game input

        let expectedGameState = Gameover
        Assert.AreEqual(expectedGameState, newGameState)

    [<Test>]
    let ``updateShip loses a life due to asteroid`` () =
        let game = {
            Ship = {Pos = (0.1, 0.5); Vel = (0., 0.); Ang = 0.; Size = 0.1}
            Asteroids = [{Pos = (0.1, 0.5); Vel = (0.1, 0.2); Size = Large}]
            Bullets = []
            Saucer = Some {Pos = (0.1, 0.5); Dir = Left; Size = 0.1}
            Score = 0
            Lives = Two
        }
        let input = {Thrust = false; Shoot = false; Rot = Negative}
        
        let newGameState = updateShip game input

        let getLives gamestate =
            match gamestate with 
            | Playing game -> Some game.Lives
            | Gameover -> None 

        let expectedLives = One
        Assert.AreEqual(Some expectedLives, getLives newGameState)

    
    
    [<Test>]
    let ``bullets get destroyed by asteroid and ship`` () =
        let game = {
            Ship = {Pos = (0., 0.); Vel = (0., 0.); Ang = 0.; Size = 0.1}
            Asteroids = [{Pos = (0.1, 0.5); Vel = (0.1, 0.2); Size = Large}]
            Saucer = None
            Bullets = [{ Pos = (0.1, 0.5); Ang = 0.0; Range = 10.0 };
                       { Pos = (0.0, 0.0); Ang = 0.0; Range = 10.0 };
                       { Pos = (0.5, 0.5); Ang = 0.0; Range = 10.0 }]
            Score = 0
            Lives = Two
        }
        let input = {Thrust = false; Shoot = false; Rot = Negative}
        
        let newGame = updateBullets game input

        let expectedBulletList = [moveBullet { Pos = (0.5, 0.5); Ang = 0.0; Range = 10.0 }]
        
        Assert.AreEqual(expectedBulletList, newGame.Bullets)
    
    [<Test>]
    let ``large asteroid is split and a small one is removed`` () =
        let game = {
            Ship = {Pos = (0., 0.); Vel = (0., 0.); Ang = 0.; Size = 0.1}
            Asteroids = [{Pos = (0.1, 0.5); Vel = (0.1, 0.2); Size = Large};
                         {Pos = (0.5, 0.5); Vel = (0.1, 0.2); Size = Small}]
            Saucer = None
            Bullets = [{ Pos = (0.1, 0.5); Ang = 0.0; Range = 10.0 };
                       { Pos = (0.5, 0.5); Ang = 0.0; Range = 10.0 }]
            Score = 0
            Lives = Two
        }
        
        let newGame = updateAsteroids game
        
        let expectedAsteroidsSize = [Medium; Medium]
        let actualAsteroidsSize =
            newGame.Asteroids
            |> List.map (fun x -> x.Size)

        let expectedScore = 50 + 200

        Assert.AreEqual(expectedAsteroidsSize, actualAsteroidsSize)
        Assert.AreEqual(expectedScore, newGame.Score)
    
    [<Test>]
    let ``update game without saucer`` () =
        let game = {
            Ship = {Pos = (0., 0.); Vel = (0., 0.); Ang = 0.; Size = 0.1}
            Asteroids = [{Pos = (0.1, 0.5); Vel = (0.1, 0.2); Size = Large};
                         {Pos = (0.5, 0.5); Vel = (0.1, 0.2); Size = Small}]
            Saucer = None
            Bullets = [{ Pos = (0.1, 0.5); Ang = 0.0; Range = 10.0 };
                       { Pos = (0.5, 0.5); Ang = 0.0; Range = 10.0 }]
            Score = 0
            Lives = Two
        }
        
        let newGame = updateSaucer game
        
        Assert.AreEqual(None, newGame.Saucer)

    [<Test>]
    let ``check score when destroying a saucer`` () =
        let game = {
            Ship = {Pos = (0., 0.); Vel = (0., 0.); Ang = 0.; Size = 0.1}
            Asteroids = []
            Saucer = Some {Pos = (0.1, 0.5); Dir = Left; Size = 0.1}
            Bullets = [{ Pos = (0.1, 0.5); Ang = 0.0; Range = 10.0 };
                       { Pos = (0.5, 0.5); Ang = 0.0; Range = 10.0 }]
            Score = 0
            Lives = Two
        }
        
        let newGame = updateSaucer game
        
        Assert.AreEqual(200, newGame.Score)
        Assert.AreEqual(None, newGame.Saucer)
    
    [<Test>]
    let ``Check game finished`` () =
        let gameFinished = {
            Ship = {Pos = (0., 0.); Vel = (0., 0.); Ang = 0.; Size = 0.1}
            Asteroids = []
            Saucer = None
            Bullets = [{ Pos = (0.1, 0.5); Ang = 0.0; Range = 10.0 };
                       { Pos = (0.5, 0.5); Ang = 0.0; Range = 10.0 }]
            Score = 0
            Lives = Two
        }
        
        let gameUnfinished = {
            Ship = {Pos = (0., 0.); Vel = (0., 0.); Ang = 0.; Size = 0.1}
            Asteroids = [{Pos = (0.1, 0.5); Vel = (0.1, 0.2); Size = Large}]
            Saucer = None
            Bullets = [{ Pos = (0.1, 0.5); Ang = 0.0; Range = 10.0 };
                       { Pos = (0.5, 0.5); Ang = 0.0; Range = 10.0 }]
            Score = 0
            Lives = Two
        }
        
        Assert.IsTrue(checkLevelFinished gameFinished)
        Assert.IsFalse(checkLevelFinished gameUnfinished)

    [<Test>]
    let ``Check gameover`` () =
        let game = {
                Ship = {Pos = (0., 0.); Vel = (0., 0.); Ang = 0.; Size = 0.1}
                Asteroids = []
                Saucer = None
                Bullets = [{ Pos = (0.1, 0.5); Ang = 0.0; Range = 10.0 };
                        { Pos = (0.5, 0.5); Ang = 0.0; Range = 10.0 }]
                Score = 0
                Lives = Two
            }
        let gamestate1 = Playing game
        let gamestate2 = Gameover

        Assert.IsFalse(checkGameOver gamestate1)
        Assert.IsTrue(checkGameOver Gameover)

    