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
        Assert.AreEqual((0.5,0.7), shiftGeneral pos vel)
    
    [<Test>]
    let ``moveBullet updates position and range correctly`` () =
        let updatedBullet = moveBullet bullet
        Assert.AreEqual()

