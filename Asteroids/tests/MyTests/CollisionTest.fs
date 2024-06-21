namespace library.Tests

open NUnit.Framework
open library.Types
open library.Collision

[<TestFixture>]
module CollisionTest =

    let ship_col = { Pos = (1.4, 0.7); Vel = (0.0, 0.0); Ang = 0.0; Size = 0.05 }
    let ship_noncol = { Pos = (0.2, 0.3); Vel = (0.0, 0.0); Ang = 0.0; Size = 0.05 }

    let saucer = Some { Pos = (1.45, 0.72); Dir = Left; Size = 0.1 }
    let bullet = { Pos = (1.44, 0.71); Ang = 0.0; Range = 10.0 }
    let asteroid = { Pos = (1.3, 0.7); Vel = (0.0, 0.0); Size = Size.Large } // radius 0.1

    [<Test>]
    let ``checkCollisionShipAsteroid detects collision with asteroid`` () =
        let result = checkCollisionShipAsteroid ship_col asteroid
        Assert.IsTrue(result)

    [<Test>]
    let ``checkCollisionShipAsteroid does not detect collision with distant ship`` () =
        let result = checkCollisionShipAsteroid ship_noncol asteroid
        Assert.IsFalse(result)

    [<Test>]
    let ``checkCollisionShipSaucer detects collision with saucer`` () =
        let result = checkCollisionShipSaucer ship_col saucer
        Assert.IsTrue(result)

    [<Test>]
    let ``checkCollisionShipSaucer does not detect collision with distant ship`` () =
        let result = checkCollisionShipSaucer ship_noncol saucer
        Assert.IsFalse(result)

    [<Test>]
    let ``checkCollisionShipBullet detects collision with bullet`` () =
        let result = checkCollisionShipBullet ship_col bullet
        Assert.IsTrue(result)

    [<Test>]
    let ``checkCollisionShipBullet does not detect collision with distant ship`` () =
        let result = checkCollisionShipBullet ship_noncol bullet
        Assert.IsFalse(result)

    [<Test>]
    let ``checkCollisionSaucerBullet detect collision`` () =
        let result = checkCollisionSaucerBullet saucer bullet
        Assert.IsTrue(result)