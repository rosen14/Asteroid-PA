//---------------------------------------------//
// Módulo Utility: funciones utilitarias varias
//---------------------------------------------//
namespace library

open Types
open Constants
open System

module Utility =

    let rand = System.Random()   // inicializo instancia de Random()

    let sizeMap (size: Size) = 
        // función que asocia tamaños de asteroide a radios.
        match size with
        | Large -> 0.1
        | Medium -> 0.08
        | Small -> 0.04

    let min3_abs (x: float) (y: float) (z: float) = 
        // toma el mínimo de los absolutos de tres números.
        let min_temp = min (abs x) (abs y)
        min min_temp (abs z)

    let distance (pos1: float * float) (pos2: float * float) = 
        // calcula la distancia entre dos posiciones teniendo en cuenta el hyperspace. 
        // toma el mínimo entre la distancia euclídea, y la correspondiente a posiciones shifteadas una pantalla de distancia
        let dx1 = fst pos1 - fst pos2
        let dy1 = snd pos1 - snd pos2
    
        let dx2 = min3_abs dx1 (aspect_ratio - dx1) (aspect_ratio + dx1)
        let dy2 = min3_abs dy1 (1.0 - dy1) (1.0 + dy1)
        Math.Sqrt(dx2 ** 2.0 + dy2 ** 2.0)

    let cartesianToPolar (vec: float * float) =
        // convierte un vector cartesiano en su norma y ángulo (con el cero abajo y la izquierda).
        let (x, y) = vec                               
        let r = Math.Sqrt(x ** 2.0 + y ** 2.0)
        let theta = Math.Atan2(y, x)
        r, theta

    let polarToCartesian (r: float) (theta: float) =
        // inversa de cartesianToPolar, a partir de norma y ángulo devuelve una tupla cartesiana
        let x = r * Math.Cos(theta)
        let y = r * Math.Sin(theta)
        (x, y)

    let trueModulo (x: float) (modulo: float) =
        // devuelve el módulo como es definido en python por ejemplo
        // ej: trueModulo -0.3 1.0 = 0.7 y no -0.3.
        let y = x % modulo
        match y with
        | _ when y < 0.0 -> y + modulo
        | _ -> y
    
    let tupleAdd (tup1: float*float) (tup2: float*float) =
        // adiciona dos tuplas float*float elemento por elemento
        (fst tup1 + fst tup2, snd tup1 + snd tup2)

    let renormalizeVelocity (vel: float*float) = 
        // Función que renormaliza vectores velocidad que se han pasado de la velocidad máxima
        let r = shipMaxVel
        let norm = Math.Sqrt ((fst vel)**2 + (snd vel)**2)
        let (vx, vy) = vel
        (vx * r / norm, vy * r / norm)