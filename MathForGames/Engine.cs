using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;
using MathLibrary;
using Raylib_cs;

namespace MathForGames
{
    class Engine
    {
        private static bool _applicationShouldClose;
        private static int _currentSceneIndex;
        private Scene[] _scenes = new Scene[0];
        private Stopwatch _stopwatch = new Stopwatch();

        /// <summary>
        /// Called to begin the application
        /// </summary>
        public void Run()
        {
            //Call start for the entire application
            Start();

            float currentTime = 0;
            float lastTime = 0;
            float deltaTime = 0;

            //Loop until the application is told to close
            while (!_applicationShouldClose && !Raylib.WindowShouldClose())
            {
                //Get how much time has passed since the application started
                currentTime = _stopwatch.ElapsedMilliseconds / 1000.0f;

                //Set delta time to be the difference in time from the last time recorded to the current time
                deltaTime = currentTime - lastTime;

                //Update the application
                Update(deltaTime);
                //Draw all items
                Draw();

                //Set the last time recorded to be the current time
                lastTime = currentTime;
            }

            //Call end for the entire application
            End();

        }

        /// <summary>
        /// Calledwhen the application starts
        /// </summary>
        private void Start()
        {
            _stopwatch.Start();
            //Create a window using raylib
            Raylib.InitWindow(800, 450, "Math for Games");
            Raylib.SetTargetFPS(60);

            Scene scene = new Scene();

            Actor planet = new Actor(300, 200, "Planet", "Images/earth.png");

            Actor planet2 = new Actor(2, 2, "Planet2", "Images/planet.png");

            Actor planet3 = new Actor(.5f, .5f, "Planet3", "Images/planet2.png");

            Actor sun = new Actor(800, 1, "Sun", "Images/sun.png");

            for (int i = 0; i < 150; i++)
            {
                Random rnd = new Random();
                int numx = rnd.Next(1, 800);
                int numy = rnd.Next(1, 500);
                Actor star = new Actor(numx, numy, "Star", "Images/star.png");
                star.SetScale(10, 10);
                scene.AddActor(star);
            }

            sun.SetScale(600, 600);
            planet.SetScale(50, 50);
            planet2.SetScale(2, 2);
            planet3.SetScale(.2f, .2f);

            Player player = new Player(10, 10, 200, "Player", "Images/player.png");
            player.SetScale(50, 50);
            planet.AddChild(planet2);
            planet2.AddChild(planet3);
            CircleCollider playerCircleCollider = new CircleCollider(28, player);
            CircleCollider planetCircleCollider = new CircleCollider(24, planet);
            CircleCollider planet2CircleCollider = new CircleCollider(29, planet2);
            CircleCollider planet3CircleCollider = new CircleCollider(10, planet3);
            CircleCollider sunCircleCollider = new CircleCollider(180, sun);

            //Enemy actor = new Enemy( 80, 80, 50, player, "Actor", "Images/enemy.png");
            //actor.SetScale(50, 50);
            //AABBCollider enemyCollider = new AABBCollider(50, 50, actor);
            //actor.Collider = enemyCollider;
            //actor.Forward = (new Vector2(700, 900));

            player.Collider = playerCircleCollider;
            planet.Collider = planetCircleCollider;
            planet2.Collider = planet2CircleCollider;
            planet3.Collider = planet3CircleCollider;
            sun.Collider = sunCircleCollider;

            
            
            scene.AddActor(planet2);
            scene.AddActor(planet);
            scene.AddActor(planet3);
            scene.AddActor(player);
            scene.AddActor(sun);
            _currentSceneIndex = AddScene(scene);
            _scenes[_currentSceneIndex].Start();
        }

        /// <summary>
        /// Called everytime the game loops
        /// </summary>
        private void Update(float deltaTime)
        {
            _scenes[_currentSceneIndex].Update(deltaTime);

            while (Console.KeyAvailable)
                Console.ReadKey(true);

        }

        /// <summary>
        /// Called everytime the game loops to update visuals
        /// </summary>
        private void Draw()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.BLACK);

            //Adds all actor icons to buffer
            _scenes[_currentSceneIndex].Draw();

            Raylib.EndDrawing();
        }

        /// <summary>
        /// Called when the application exits
        /// </summary>
        private void End()
        {
            _scenes[_currentSceneIndex].End();
            Raylib.CloseWindow();
        }

        /// <summary>
        /// Adds a scene to the engine's scene array
        /// </summary>
        /// <param name="scene">The scene that will be added to the scene array</param>
        /// <returns>The index where the new scene is located</returns>
        public int AddScene(Scene scene)
        {
            //Create a new temporary array
            Scene[] tempArray = new Scene[_scenes.Length + 1];

            //Copy all values from old array into the new array
            for (int i = 0; i < _scenes.Length; i++)
            {
                tempArray[i] = _scenes[i];
            }

            //Set the last indec to be the new scene
            tempArray[_scenes.Length] = scene;

            //Set the old array to be the new array
            _scenes = tempArray;

            //Return the last index
            return _scenes.Length - 1;
        }

        /// <summary>
        /// Gets the next key in the input stream
        /// </summary>
        /// <returns>The key that was pressed</returns>
        public static ConsoleKey GetNextKey()
        {
            //If there is no key being pressed...
            if (!Console.KeyAvailable)
                //...return
                return 0;

            //Return the current key being pressed
            return Console.ReadKey(true).Key;
        }

        /// <summary>
        /// Ends the application
        /// </summary>
        public static void CloseApplication()
        {
            _applicationShouldClose = true;
        }
    }
}
