//------------------------------------------------------//
// Módulo Actions: acciones ejecutadas por el jugador 
// (disparar, mover nave), y disparo del platillo volador
//------------------------------------------------------//
namespace library

open System
open Types
open Constants
open Utility
open Dynamics

module Actions =

    let accelerateShip (ship: Ship) (input: Input) =
        // función que cambia la velocidad de la nave dependiendo del input
        let velocityDelta (ship: Ship) (thrust: bool) =
            let (velR, velTheta) = cartesianToPolar ship.Vel
            match thrust with
            | true -> (shipAcc * Math.Cos(ship.Ang), shipAcc * Math.Sin(ship.Ang))
            | false -> (-shipDesacc * Math.Cos(velTheta), -shipDesacc * Math.Sin(velTheta))
        
        let velDelta = velocityDelta ship input.Thrust

        let newVelocity =
            let velFinalUnnorm = tupleAdd ship.Vel velDelta
            match input.Thrust with
            | true ->   match cartesianToPolar velFinalUnnorm with
                        | (r, _) when r <= shipMaxVel -> velFinalUnnorm
                        | _ -> velFinalUnnorm |> renormalizeVelocity
            | false ->
                        match cartesianToPolar ship.Vel with
                        | (r, _) when r = 0.0  -> (0.0, 0.0)
                        | _ -> velFinalUnnorm

        newVelocity

    let shootBullet (ship: Ship) (bullets: Bullet list) (input: Input) = 
        // función  que devuelve una nueva lista de balas dependiendo de 
        // si la nave está disparando o no
        let fire (ship: Ship) (bullets: Bullet list) =
            let newBullet = 
                {
                    Pos = (fst ship.Pos + ship.Size * Math.Cos(ship.Ang),
                            snd ship.Pos + ship.Size * Math.Sin(ship.Ang))
                    Ang = ship.Ang
                    Range = 0.0
                }
            bullets @ [newBullet]
        
        match input.Shoot with
        | true -> fire ship bullets
        | false -> bullets

    let saucerShoot (saucer: Saucer Option) (bullets: list<Bullet>)=
        // función que devuelve una lista de balas dependiendo de si
        // el platillo dispara o no
        let saucerFire (saucer: Saucer) (bullets: list<Bullet>) =
            let angRandom = 2.0*Math.PI*rand.NextDouble()
            let newBullet = 
                {
                    Pos = (fst saucer.Pos + saucer.Size*Math.Cos(angRandom),
                        snd saucer.Pos + saucer.Size*Math.Sin(angRandom))
                    Ang = angRandom
                    Range = 0.0
                }
            bullets @ [newBullet]
            
        let fireProbability = 0.3
    
        match saucer with
        | None -> bullets
        | Some s -> 
                match rand.NextDouble() < fireProbability with
                | false -> bullets
                | true -> saucerFire s bullets