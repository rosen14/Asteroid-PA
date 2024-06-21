//--------------------------------------------------//
// Módulo constants: definición de constantes varias
//--------------------------------------------------//
namespace library

module Constants =

    let fps = 30
    let aspect_ratio = 1.5                // tamaño de la pantalla en eje x con respecto a eje y
    let maxAsteroidVel = 5./(float fps)   // velocidad máxima de asteroides
    let bulletsVelocity = 6./(float fps)  // velocidad de las balas
    let maxBullRange = 20.                // rango máximo en la trayectoria de las balas
    let shipAcc = 0.1 / (float fps)       // velocidad acelerada a cada paso por la nave
    let shipDesacc = 0.05 / (float fps)   // velocidad por frame perdida por no acelerar la nave por
    let shipMaxVel = 5.0 / (float fps)    // velocidad máxima de la nave
    let saucerVel = 1.0/(float fps)       // velocidad horizontal del platillo