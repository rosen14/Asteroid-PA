//-------------------------------------------------//
// Módulo dynamics: funciones que mueven los objetos 
// y describen la dinámica
//-------------------------------------------------//

namespace library

open System
open Types
open Constants
open Utility
open Collision

module Dynamics =

    let astSplit (ast: Asteroid) = 
        // función que toma como argumento un asteroide y devuelve
        // dos asteroides en el mismo lugar, con velocidades random, de tamaño menor
        // destruye el asteroid si el tamañ es small.
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
        // función que actualiza la posición de una bala por inercia
        let Vx = bulletsVelocity * Math.Cos(bullet.Ang)
        let Vy = bulletsVelocity * Math.Sin(bullet.Ang)

        let newPosX = trueModulo ((fst bullet.Pos) + Vx ) aspect_ratio
        let newPosY = trueModulo ((snd bullet.Pos) + Vy ) 1.0
        let newPosition = (newPosX, newPosY)
        
        let newRange = bullet.Range + 1./(float fps)

        let newBullet = 
            { bullet with
                Pos = newPosition
                Range = newRange
            }
        newBullet

    let moveAndClearBullets (bullets: Bullet list) = 
        // función que mueve las balas y destruye aquellas
        // cuyo rango supere el máximo
        bullets
        |> List.map moveBullet
        |> List.filter (fun b -> b.Range <= maxBullRange)

    let shiftGeneral (pos: float*float) (vel: float*float) =
        // función general que actualiza la posición de "pos" dada "vel", tuplas float*float
        let newPosX = trueModulo (fst pos + fst vel) aspect_ratio
        let newPosY = trueModulo (snd pos + snd vel) 1.0
        let newPosition = (newPosX, newPosY)
        newPosition
    
    let newPosAsteroid (asteroid: Asteroid) =
        // Función que actualiza la posición de los asteroides
        shiftGeneral asteroid.Pos asteroid.Vel
    
    let newPosShip (ship: Ship) =
        // Función que actualiza la posición de las naves
        shiftGeneral ship.Pos ship.Vel

    let moveSaucer (saucer: Saucer) = 
        // Función que actualiza la posición de un platillo
        let vel = 
            match saucer.Dir with
            | Right -> (saucerVel, 0.)
            | Left -> (-saucerVel, 0.)
        let newPos = shiftGeneral saucer.Pos vel
        newPos

    let deltaAngle (input: Input) = 
        // Dado el input (si hay rotación o no) devuelve en qué valor se rota la nave.
        let angQuantum = Math.PI/8.0
        let newAngle (rotation: Rotation) =
            match rotation with
            | Positive -> angQuantum
            | Negative -> -angQuantum
            | Zero -> 0.0
        newAngle input.Rot