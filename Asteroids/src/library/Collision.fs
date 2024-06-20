namespace library



open Types
open Utility

module Collision =
        
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
        // Devuelve verdadero si hay superposición entre el saucer y una bala
        match saucer with
        | None -> false
        | Some s ->
            let saucerBulletDistance = distance bullet.Pos s.Pos
            saucerBulletDistance < s.Size //la bala se supone puntual

    let isBullDestroyed (bull: Bullet) (asts: Asteroid List) =
        // true cuando bull es destruída por contacto con algún asteroide
        let collisions =
            asts
            |> List.filter (fun x -> checkCollisionBulletAsteroid bull x)
        not (collisions = [])
    
    let isAstDestroyed (bulls: Bullet List) (ast: Asteroid) =
        // true cuando asteroide es destruido por contacto con alguna bala
        let collisions =
            bulls
            |> List.filter (fun x -> checkCollisionBulletAsteroid x ast)
        not (collisions = [])