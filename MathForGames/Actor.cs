﻿using System;
using System.Collections.Generic;
using System.Text;
using MathLibrary;
using Raylib_cs;

namespace MathForGames
{

    class Actor
    {
        private string _name;
        private bool _started;
        private Vector2 _forward;
        private Collider _collider;
        private Matrix3 _globalTransform = Matrix3.Identity;
        private Matrix3 _localTransform = Matrix3.Identity;
        private Matrix3 _translation = Matrix3.Identity;
        private Matrix3 _rotation = Matrix3.Identity;
        private Matrix3 _scale = Matrix3.Identity;
        private Actor[] _children = new Actor[0];
        private Actor _parent;
        private Sprite _sprite;

        /// <summary>
        /// True if the start function has been called for this actor
        /// </summary>
        public bool Started
        {
            get { return _started; }
        }

        public string Name
        {
            get { return _name; }
        }

        public Vector2 LocalPosition
        {
            get { return new Vector2(_translation.M02, _translation.M12);  }
            set 
            {
                SetTranslation(value.X, value.Y);
            }
        }

        /// <summary>
        /// The position of this actor in the world
        /// </summary>
        public Vector2 WorldPosition
        {
            //Return the global transform's T column
            get { return new Vector2(_globalTransform.M02, _globalTransform.M12); }
            set
            {
                //If the acto has a parent...
                if (Parent != null)
                {
                    //...convert the world coordinates into the local coordinates and translate the actor
                    float xOffset = (value.X - Parent.WorldPosition.X) / new Vector2(_globalTransform.M00, _globalTransform.M10).Magnitude;
                    float yOffset = (value.Y - Parent.WorldPosition.Y) / new Vector2(_globalTransform.M10, _globalTransform.M11).Magnitude;

                    SetTranslation(xOffset, yOffset);
                }
                //If this actor doesn't have a parent...
                else
                    //...set the position to the given value
                    LocalPosition = value;
            }
        }

        public Matrix3 GlobalTransform
        {
            get { return _globalTransform; }
            private set { _globalTransform = value; }
        }

        public Matrix3 LocalTransform
        {
            get { return _localTransform; }
            private set { _localTransform = value; }
        }

        public Actor Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        public Actor[] Children
        {
            get { return _children; }
        }

        public Vector2 Size
        {
            get 
            {
                float xScale = new Vector2(_scale.M00, _scale.M10).Magnitude;
                float yScale = new Vector2(_scale.M01, _scale.M11).Magnitude;

                return new Vector2(xScale, yScale); 
            }
            set { SetScale(value.X, value.Y); }
        }

        public Vector2 Forward
        {
            get { return new Vector2(_rotation.M00, _rotation.M10); }
            set 
            { 
                Vector2 point = value.Normalized + LocalPosition;
                LookAt(point);
            }
        }

        public Sprite Sprite
        {
            get { return _sprite; }
            set { _sprite = value; }
        }

        public Collider Collider
        {
            get { return _collider; }
            set { _collider = value; }
        }

        public Actor(float x, float y, string name = "Actor", string path = "") :
            this(new Vector2 { X = x, Y = y }, name, path) {}


        public Actor(Vector2 position, string name = "Actor", string path = "")
        {
            SetTranslation(position.X, position.Y);
            _name = name;

            if (path != "")
                _sprite = new Sprite(path);
        }

        public void UpdateTransforms()
        {
            _localTransform = _translation * _rotation * _scale;

            if (Parent != null)
                GlobalTransform = Parent.GlobalTransform * LocalTransform;
            else
                GlobalTransform = LocalTransform;
        }

        public void AddChild(Actor child)
        {
            //Create a temp array larger than the original
            Actor[] tempArray = new Actor[_children.Length + 1];

            //Copy all values from the original array into the temp array
            for (int i = 0; i < _children.Length; i++)
            {
                tempArray[i] = _children[i];
            }

            //Add the new child to the end of the new array
            tempArray[_children.Length] = child;

            //Set the old array to be the new array
            _children = tempArray;

            //Set the parent of the child to be this actor
            child.Parent = this;

        }

        public bool RemoveChild(Actor child)
        {
            //Create a variable to store if the removal was successful
            bool childRemoved = false;

            //Create a new array that is smaller than the original
            Actor[] tempArray = new Actor[_children.Length - 1];

            //Copy all values except the actor we don't want into the new array
            int j = 0;
            for (int i = 0; i < tempArray.Length; i++)
            {
                //If the actor that the loop is on is not the one to remove...
                if (_children[i] != child)
                {
                    //...add the actor back into the new array
                    tempArray[j] = _children[i];
                    j++;
                }
                //Otherwise if this actor is the one to remove...
                else
                    //...set actorRemoved to true
                    childRemoved = true;
            }

            //If the actor removal was successful...
            if (childRemoved)
            {
                //...set the old array to be the new array
                _children = tempArray;

                //Set the parent of the child to be nothing
                child.Parent = null;
            }
           
            return childRemoved;
        }

        public virtual void Start()
        {
            _started = true;
        }

        public virtual void Update(float deltaTime)
        {
            UpdateTransforms();

            foreach (Actor child in _children)
            {
                Rotate(deltaTime);
                child.Rotate(3 * deltaTime);
            }

            if (_name == "Star")
                Rotate(deltaTime);
            if (_name == "Sun")
                Rotate(deltaTime / 4);

            if (_name != "Star")
                Console.WriteLine(_name + " X: " + WorldPosition.X + "|| Y: " + WorldPosition.Y);
                
        }

        public virtual void Draw()
        {
            if (_sprite != null)
                _sprite.Draw(_globalTransform);
            if (_name != "Star")
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

        public void SetTranslation(float translationX, float translationY)
        {
            _translation = Matrix3.CreateTranslation(translationX, translationY);
        }

        public void Translate(float translationX, float translationY)
        {
            _translation *= Matrix3.CreateTranslation(translationX, translationY);
        }

        public void SetRotation(float radians)
        {
            _rotation = Matrix3.CreateRotation(radians);
        }

        public void Rotate(float radians)
        {
            _rotation *= Matrix3.CreateRotation(radians);
        }

        public void SetScale(float x, float y)
        {
            _scale = Matrix3.CreateScale(x, y);
        }

        public void Scale(float x, float y)
        {
            _scale *= Matrix3.CreateScale(x, y);
        }

        /// <summary>
        /// Rotates the actor to face the given position
        /// </summary>
        /// <param name="position">The position the actor should be looking at</param>
        public void LookAt(Vector2 position)
        {
            //Find the direction that the actor should look in
            Vector2 direction = (position - LocalPosition).Normalized;

            //Use the dot product to find the angle the actor needs to rotate
            float dotProd = Vector2.DotProduct(direction, Forward);

            if (dotProd > 1)
                dotProd = 1;

            float angle = (float)Math.Acos(dotProd);

            //Find a perpendicular vedctor to the direction
            Vector2 perpDirection = new Vector2(direction.Y, -direction.X);

            //Find the dot product of the perpendicular vector and the current forward
            float perpDot = Vector2.DotProduct(perpDirection, Forward);

            //If the result is not 0, use it to change the sign of the angle to be either positive or negative
            if (perpDot != 0)
                angle *= -perpDot / Math.Abs(perpDot);

            Rotate(angle);
        }
    }
}
