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
        private Camera3D _camera = new Camera3D();
        Player player;
        int x = 0;

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

        private void InitializeCamera()
        {
            _camera.position = new System.Numerics.Vector3(0, 10, 10); // Camera Position
            _camera.target = new System.Numerics.Vector3(0, 0, 0); // Point the camera is focused on
            _camera.up = new System.Numerics.Vector3(0, 1, 0); // Camera up vector (rotation towards target)
            _camera.fovy = 45; // Camera field of view Y
            _camera.projection = CameraProjection.CAMERA_PERSPECTIVE; // Camera mode type
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

            InitializeCamera();

            Scene scene = new Scene();

            player = new Player(0, 0, 40, "Player", Shape.SPHERE);
            player.SetScale(1, 1, 1);
            CircleCollider playerCircleCollider = new CircleCollider(28, player);
            player.SetColor(new Vector4(51, 42, 8, 255));
            player.LookAt(new Vector3(1, 0, 0));

            Enemy enemy = new Enemy(0, 0, 10, player, "Enemy", Shape.CUBE);
            enemy.SetScale(1, 1, 1);
            CircleCollider enemyCircleCollider = new CircleCollider(28, enemy);
            //enemy.SetColor(new Vector4(51, 42, 8, 255));

            //Enemy actor = new Enemy( 80, 80, 50, player, "Actor", "Images/enemy.png");
            //actor.SetScale(50, 50);
            //AABBCollider enemyCollider = new AABBCollider(50, 50, actor);
            //actor.Collider = enemyCollider;
            //actor.Forward = (new Vector2(700, 900));

            player.Collider = playerCircleCollider;

            scene.AddActor(player);
            scene.AddActor(enemy);

            _currentSceneIndex = AddScene(scene);
            _scenes[_currentSceneIndex].Start();
        }

        /// <summary>
        /// Called everytime the game loops
        /// </summary>
        private void Update(float deltaTime)
        {
            _scenes[_currentSceneIndex].Update(deltaTime);

            //_camera.target = new System.Numerics.Vector3(player.WorldPosition.X, player.WorldPosition.Y, player.WorldPosition.Z - 5);
            //_camera.position = new System.Numerics.Vector3(player.WorldPosition.X, player.WorldPosition.Y + 1, player.WorldPosition.Z);
                
            
            while (Console.KeyAvailable)
                Console.ReadKey(true);

        }

        /// <summary>
        /// Called everytime the game loops to update visuals
        /// </summary>
        private void Draw()
        {
            Raylib.BeginDrawing();
            Raylib.BeginMode3D(_camera);

            Raylib.ClearBackground(Color.RAYWHITE);
            Raylib.DrawGrid(50, 1);

            //Adds all actor icons to buffer
            _scenes[_currentSceneIndex].Draw();

            Raylib.EndMode3D();
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
