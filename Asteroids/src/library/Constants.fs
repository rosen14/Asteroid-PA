namespace library


module Constants =

    let fps = 30
    let aspect_ratio = 1.5
    let maxAsteroidVel = 5./(float fps)
    let bulletsVelocity = 6./(float fps)
    let maxBullRange = 20.
    let shipAcc = 0.1 / (float fps)
    let shipDesacc = 0.05 / (float fps)
    let shipMaxVel = 5.0 / (float fps)