namespace library.Tests

open NUnit.Framework
open library.Types
open library.Actions
open library.Constants
open System

[<TestFixture>]
module ActionsTest =
    [<SetUp>]
    let Setup () =
        ()

    [<Test>]
    let ``accelerate Ship from zero velocity`` () =
        let ship = {
            Pos = (aspect_ratio/2., 0.5)
            Vel = (0., 0.)
            Ang = 0.
            Size = 0.1
            }

        let input = {
            Thrust = true
            Shoot = false
            Rot = Zero
        }
        let newVelocty = accelerateShip ship input
        let expectedVelocity = (shipAcc, 0.)
        Assert.AreEqual(expectedVelocity, newVelocty)

    [<Test>]
    let ``limit speed reached`` () =
        let ship = {
            Pos = (aspect_ratio/2., 0.5)
            Vel = (shipMaxVel, 0.)
            Ang = 0.
            Size = 0.1
            }

        let input = {
            Thrust = true
            Shoot = false
            Rot = Zero
        }
        let newVelocty = accelerateShip ship input
        let expectedVelocity = (shipMaxVel, 0.)
        Assert.AreEqual(expectedVelocity, newVelocty)
    (*
    [<Test>]
    let ``zero Velocity `` () =
        let ship = {
            Pos = (aspect_ratio/2., 0.5)
            Vel = (0., 0.)
            Ang = 0.
            Size = 0.1
            }

        let input = {
            Thrust = false
            Shoot = false
            Rot = Zero
        }
        let newVelocty = accelerateShip ship input
        let expectedVelocity = (0., 0.)
        Assert.AreEqual(expectedVelocity, newVelocty)
    *)
    [<Test>]
    let ``ship shoot bullet`` () =
        let ship = {
            Pos = (aspect_ratio/2., 0.5)
            Vel = (0., 0.)
            Ang = 0.3
            Size = 0.1
            }

        let input = {
            Thrust = false
            Shoot = true
            Rot = Zero
        }
        let bullet1 = { Pos = (1.44, 0.71); Ang = 0.0; Range = 10.0 }
        let originalBulletList = [bullet1]

        let newBulletList = shootBullet ship originalBulletList input
        let newBulletPos = (fst ship.Pos + ship.Size * Math.Cos(ship.Ang),
                            snd ship.Pos + ship.Size * Math.Sin(ship.Ang))
        let expectedBulletList = originalBulletList @ [{Pos = newBulletPos; Ang = ship.Ang; Range = 0}]
        Assert.AreEqual(expectedBulletList, newBulletList)

        