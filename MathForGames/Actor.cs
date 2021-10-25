using System;
using System.Collections.Generic;
using System.Text;
using MathLibrary;
using Raylib_cs;

namespace MathForGames
{
    struct Icon
    {
        public char Symbol;
        public Color color;
    }

    class Actor
    {
        private Icon _icon;
        private string _name;
        private Vector2 _position;
        private bool _started;
        private Vector2 _forward;
        private Collider _collider;

        /// <summary>
        /// True if the start function has been called for this actor
        /// </summary>
        public bool Started
        {
            get { return _started; }
        }

        public Vector2 Position
        {
            get { return _position;  }
            set { _position = value; }
        }

        public Icon Icon
        {
            get { return _icon; }
        }

        public Vector2 Forward
        {
            get { return _forward; }
            set { _forward = value; }
        }

        public Collider Collider
        {
            get { return _collider; }
            set { _collider = value; }
        }

        public Actor(char icon, float x, float y, Color color, string name = "Actor") :
            this(icon, new Vector2 { X = x, Y = y }, color, name) {}


        public Actor(char icon, Vector2 position, Color color, string name = "Actor")
        {
            _icon = new Icon { Symbol = icon, color = color };
            _position = position;
            _name = name;
        }

        public virtual void Start()
        {
            _started = true;
        }

        public virtual void Update(float deltaTime)
        {
            Console.WriteLine(_name + ": " + Position.X + ", " + Position.Y);
        }

        public virtual void Draw()
        {
            Raylib.DrawText(Icon.Symbol.ToString(), (int)Position.X - 18, (int)Position.Y - 28, 50, Icon.color);
            Collider.Draw();
        }

        public void End()
        {

        }

        public virtual void OnCollision(Actor other)
        {

        }

        public virtual bool CheckForCollision(Actor other)
        {
            //Return false if either actor doesn't have a collider attached 
            if (Collider == null || other.Collider == null)
                return false;

            return Collider.CheckCollision(other);
        }
    }
}
