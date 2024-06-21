namespace library.Tests

open NUnit.Framework
open library.Types
open library.Utility
open library.Dynamics
open System
open library.Constants

[<TestFixture>]
module DynamicsTests =

    let bullet = { Pos = (1.0, 0.5); Ang = 0.0; Range = 10.0 }
    let asteroids = [
        { Pos = (0.2, 0.7); Vel = (0.1, 0.2); Size = Size.Large };
        { Pos = (1.3, 0.4); Vel = (-0.2, -0.3); Size = Size.Medium }
    ]
    let ship = { Pos = (1.4, 0.3); Vel = (0.2, -0.1); Ang = 0.0; Size = 0.1 }
    let saucer = { Pos = (0.9, 0.8); Dir = Left; Size = 0.15 }
    

    [<Test>]
    let ``shiftGeneral shifts correctly`` () =
        let pos = (0.5, 0.5)
        let vel = (0.1, 0.2)
        Assert.AreEqual((0.6,0.7), shiftGeneral pos vel)
    
    [<Test>]
    let ``moveBullet updates position and range correctly`` () =
        let bullet = { Pos = (aspect_ratio, 0.5); Ang = 0.0; Range = 10.0 } // 
        let updatedBullet = moveBullet bullet
        let newPos = (trueModulo (fst bullet.Pos + bulletsVelocity) aspect_ratio, 0.5)
        let newRange = bullet.Range + bulletsVelocity
        let expectedBullet = { Pos = newPos; Ang = 0.0; Range = newRange }
        Assert.AreEqual(expectedBullet, updatedBullet)

    [<Test>]
    let ``remove bullet by exceeding maxRange`` () =
        let bullet2 = { Pos = (1.0, 0.5); Ang = 0.0; Range = maxBullRange}
        let originalBulletList = [bullet; bullet2]
        let updatedBulletList = moveAndClearBullets (originalBulletList)
        let excpectedBulletList = [moveBullet bullet]
        Assert.AreEqual(excpectedBulletList, updatedBulletList)

    [<Test>]
    let ``large asteroid divides into two medium ones with equal position `` () =
        
        let largeAsteroid = {Pos = (0.2, 0.7); Vel = (0.5, 0.5); Size = Large}
        let generatedAsteroids = astSplit (largeAsteroid)
        Assert.AreEqual(Medium, generatedAsteroids[0].Size)
        Assert.AreEqual(Medium, generatedAsteroids[1].Size)
        Assert.AreEqual(largeAsteroid.Pos, generatedAsteroids[0].Pos)
        Assert.AreEqual(largeAsteroid.Pos, generatedAsteroids[1].Pos)

    [<Test>]
    let ``destroying a small asteroid results in an empty list `` () =
        
        let smallAsteroid = {Pos = (0.2, 0.7); Vel = (0.5, 0.5); Size = Small}
        let generatedAsteroids = astSplit (smallAsteroid)
        let expectedList : List<Asteroid> = []
        Assert.AreEqual(expectedList, generatedAsteroids)

    [<Test>]
    let ``turn in all directions`` () =
        let input1 = {Thrust = true; Shoot = false; Rot = Positive}
        let deltaAngle1 = deltaAngle (input1)
        let expectedAngle1 = Math.PI/8.0
        Assert.AreEqual(expectedAngle1, deltaAngle1)

        let input2 = {Thrust = true; Shoot = false; Rot = Negative}
        let deltaAngle2 = deltaAngle (input2)
        let expectedAngle2 = -Math.PI/8.0
        Assert.AreEqual(expectedAngle2, deltaAngle2)

        let input3 = {Thrust = true; Shoot = false; Rot = Zero}
        let deltaAngle3 = deltaAngle (input3)
        let expectedAngle3 = 0.
        Assert.AreEqual(expectedAngle3, deltaAngle3)

    [<Test>]
    let ``moveSaucer in both directions `` () =
        let saucer = {Pos = (0.9, 0.1); Dir = Left; Size = 0.1}
        let newSaucerPos = moveSaucer (saucer)
        let expectedPos = (0.9 - saucerVel, 0.1)
        Assert.AreEqual(expectedPos, newSaucerPos)

        let saucer2 = {Pos = (0.9, 0.1); Dir = Right; Size = 0.1}
        let newSaucerPos2 = moveSaucer (saucer)
        let expectedPos2 = (trueModulo (0.9 - saucerVel) aspect_ratio, 0.1)
        Assert.AreEqual(expectedPos2, newSaucerPos2)

    
