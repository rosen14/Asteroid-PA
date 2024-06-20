namespace library


module Constants =

    let fps = 24
    let aspect_ratio = 1.5
    let maxAsteroidVel = 20.0/(float fps)
    let bulletsVelocity = 25/(float fps)
    let maxBullRange = 20
    let shipAcc = 1.0 / (float fps)
    let shipDesacc = 0.5 / (float fps)
    let shipMaxVel = 20.0 / (float fps)