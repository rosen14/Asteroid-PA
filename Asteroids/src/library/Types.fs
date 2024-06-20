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

    let nextLives (life: Lives) =
        match life with
        | Three -> Two
        | Two -> One
        | One -> One

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

    type Gamestate =
        | Playing of Game
        | Gameover