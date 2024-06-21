namespace library.Tests

open NUnit.Framework
open library.Types

[<TestFixture>]
module TypesTests =

    [<Test>]
    let ``nextLives should return Two when input is Three`` () =
        let result = nextLives Three
        Assert.AreEqual(Two, result)

    [<Test>]
    let ``nextLives should return One when input is Two`` () =
        let result = nextLives Two
        Assert.AreEqual(One, result)