namespace library


open Types
open Constants
open System

module Utility =

    let rand = System.Random()   // inicializo instancia de Random()

    let sizeMap (size: Size) = 
        match size with
        | Large -> 0.04
        | Medium -> 0.02
        | Small -> 0.01

    let min_abs x y = 
        min (abs x) (abs y)

    let distance (pos1: float * float) (pos2: float * float) = 
        let dx1 = fst pos1 - fst pos2
        let dy1 = snd pos1 - snd pos2

        let dx2 = min_abs dx1 (aspect_ratio - dx1)
        let dy2 = min_abs dy1 (1.0 - dy1)
        Math.Sqrt(dx2 ** 2.0 + dy2 ** 2.0)

    let cartesianToPolar (vec: float * float) =
        let (x, y) = vec                               
        let r = Math.Sqrt(x ** 2.0 + y ** 2.0)
        let theta = Math.Atan2(y, x)
        r, theta

    let polarToCartesian (r: float) (theta: float) =
        let x = r * Math.Cos(theta)
        let y = r * Math.Sin(theta)
        (x, y)

    let trueModulo (x: float) (modulo: float) =
        let y = x % modulo
        match y with
        | _ when y < 0.0 -> y + modulo
        | _ -> y
    
    let tupleAdd (tup1: float*float) (tup2: float*float) =
        (fst tup1 + fst tup2, snd tup1 + snd tup2)

    let renormalizeVelocity (vel: float*float) = 
        // Esta función auxiliar sirve para acelerar y cambiar la dirección conservando la velocidad
        let (r, theta) = cartesianToPolar vel
        let norm = Math.Sqrt ((fst vel)**2 + (snd vel)**2)
        let (vx, vy) = vel
        (vx * r / norm, vy * r / norm)