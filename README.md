# Asteroid-PA

¡Bienvenido al repositorio de Asteroid-PA! Este proyecto es una implementación del clásico juego Asteroids en F#, con funciones de control de nave, destrucción de asteroides y platillos voladores enemigos. El código en este repositorio contiene todas las funciones necesarias para la lógica del juego, pero no incluye el bucle principal ni la interfaz gráfica del mismo. Está diseñado para ser integrado en un proyecto mayor que incluya estos componentes.

![alt text](https://silverballmuseum.com/wp-content/uploads/2016/06/asteroids.jpg)

## Características principales

- **Control de nave**: Los jugadores pueden controlar la aceleración, rotación y disparo de la nave espacial.
- **Objetivo**: Destruir todos los asteroides y platillos voladores enemigos.
- **Sistema de puntuación y tipos de asteroide**:
  - Grandes (50 puntos): Se dividen en 2 asteroides medianos.
  - Medianos (100 puntos): Se dividen en 2 asteroides pequeños.
  - Pequeños (200 puntos).
  - Platillo volador (200 puntos).

- **Vidas**: Los jugadores comienzan con 3 vidas. Las colisiones con asteroides, balas o platillos voladores enemigos reducen las vidas.

## Estructura del repositorio

EL repositorio tiene la siguiente estructura:
```bash
.
└── Asteroids/
    ├── src/
    │   ├── library/
    │   └── main/    │           
    └── tests/
    └── Asteroids.sln 
```

Dentro de la carpeta `library` se encuentran todas las funciones organizadas en diferentes módulos según el objetivo de las mismas.

### Test Unitarios

El proyecto contiene NUnits sobre la mayoría de las funciones principales. Dichos test se encuentran en la carpeta `test`. Para ejecutarlos simplemente ejecute la siguiente linea de comando:
```bash
/Asteroids> dotnet test 
```
## Configuración y uso

1. **Clonar el repositorio**:

```bash
git clone https://github.com/rosen14/Asteroid-PA.git
```

2. **Instalar dependencias**:

Asegúrate de tener instalado .NET en tu máquina. Puedes seguir estas [instrucciones](https://learn.microsoft.com/es-mx/dotnet/core/install/) para realizarlo.

3. **Ejecutar**:

Desde la carpeta `main` del repositorio, puedes ejecutar el proyecto. Aunque no contiene el bucle principal del juego ni la interfaz gráfica completa, puedes integrarlo en tu propio proyecto añadiendo estas funcionalidades.

```bash
/Asteroids/src/main> dotnet run 
```
## Contribuciones

Este proyecto está abierto a contribuciones. Si deseas mejorar la lógica del juego, añadir nuevas características o integrar la interfaz gráfica y el bucle principal del juego, te invitamos a enviar un pull request con tus cambios.

## Contacto

Para preguntas o sugerencias sobre el proyecto, puedes contactar al mantenedor del repositorio a través de la página del proyecto en GitHub: [Asteroid-PA](https://github.com/rosen14/Asteroid-PA).