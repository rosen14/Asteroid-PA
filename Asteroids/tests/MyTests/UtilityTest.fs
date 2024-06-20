namespace library.Tests

open NUnit.Framework
open library.Utility
open library.Types
open library.Constants
open System
[<TestFixture>]
module UtilityTests =

    [<Test>]
    let ``min3_abs returns minimum of absolute values`` () =
        let result = min3_abs -3.0 2.0 0.0
        Assert.AreEqual(0.0, result)

    [<Test>]
    let ``distance calculates correct distance through hyperspace`` () =
        let result = distance (0.1, 0.5) (aspect_ratio - 0.1, 0.5)
        Assert.AreEqual(0.2, result, 0.0001)

    [<Test>]
    let ``distance calculates correct distance through normalspace`` () =
        let result = distance (1.0, 0.0) (1.0, 0.5)
        Assert.AreEqual(0.5, result, 0.0001)

    [<Test>]
    let ``cartesianToPolar converts correctly`` () =
        let result = cartesianToPolar (1.0, 1.0)
        Assert.AreEqual(Math.Sqrt(2.0), fst result, 0.0001)
        Assert.AreEqual(Math.PI / 4.0, snd result, 0.0001)

    [<Test>]
    let ``polarToCartesian converts correctly`` () =
        let result = polarToCartesian (Math.Sqrt(2.0)) (Math.PI / 4.0)
        Assert.AreEqual(1.0, fst result, 0.0001)
        Assert.AreEqual(1.0, snd result, 0.0001)

    [<Test>]
    let ``trueModulo returns correct positive result`` () =
        let result = trueModulo -0.3 1.0
        Assert.AreEqual(0.7, result, 0.0001)

    [<Test>]
    let ``tupleAdd adds tuples correctly`` () =
        let result = tupleAdd (1.0, 2.0) (3.0, 4.0)
        Assert.AreEqual((4.0, 6.0), result)

    [<Test>]
    let ``renormalizeVelocity normalizes velocity correctly`` () =
        let result = renormalizeVelocity (20., 20.)
        let norm = Math.Sqrt((fst result) ** 2.0 + (snd result) ** 2.0)
        Assert.AreEqual(shipMaxVel, norm, 0.0001)
