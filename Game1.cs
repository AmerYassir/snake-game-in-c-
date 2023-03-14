using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace sanke
{
    class food
    {
        public Vector2D pos;
        public Texture2D foodBlock;

        public food(Vector2D pos,Texture2D block)
        {
            this.pos = pos;
            this.foodBlock = block;
        }
    }
    struct Vector2D
    {
        public int x;
        public int y;

        public Vector2D(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public bool is_equal(Vector2D other)
        {
            if (this.x ==other.x)
            {
                if (this.y==other.y)
                {
                    return true;
                }
            }
            return false;
        }
    }
    enum Direction
    {
        Up = 0,
        Down = 1,
        Left = 2,
        Right = 3,
        pause=4
    }

    static class Directions
    {
        public static Vector2D[] directions = {
        new Vector2D(0, -1), // Up
        new Vector2D(0, 1), // Down
        new Vector2D(-1, 0), // Left
        new Vector2D(1, 0) // Right
    };
    }
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        readonly int window_width = 800;
        readonly int window_height = 600;
        readonly int snake_block_hieght=10;
        readonly int snake_block_width =10;
        readonly int food_block_hieght = 20;
        readonly int food_block_width = 20;
        readonly int initial_lenght =10;

        private Timer timer;
        private int timerValue = 0;
        private int points_counter = 0;
        SpriteFont font;

        Vector2D snake_speed;
        Direction snake_diraction;

        food snake_food;
        Texture2D snakeBlock;
        Texture2D foodBlock;
        List<(Texture2D,Vector2D)> snakeBlocks;
        List<Rectangle> snake_body
        {
            get
            {
                List < Rectangle > rects=new List<Rectangle> ();
                for (int i = 0; i < snakeBlocks.Count-1; i++)
                {
                    rects.Add(new Rectangle(snakeBlocks[i].Item2.x, snakeBlocks[i].Item2.y, snake_block_width, snake_block_hieght));

                }
                return rects;
            }
        }
        Rectangle snake_head
        {    
            get
            {
                var last=snakeBlocks.Last();
                return new Rectangle(last.Item2.x, last.Item2.y, snake_block_width, snake_block_hieght);
            }
        }
        Rectangle food_rect
        {
            get
            {
                return new Rectangle(snake_food.pos.x, snake_food.pos.y, food_block_width, food_block_hieght);
            }
        }
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            timer = new Timer(1000);

            TargetElapsedTime = TimeSpan.FromSeconds(1.0 / 30.0);

            // Set the game to run at a fixed time step
            IsFixedTimeStep = true;
        }

        private Direction check_snake_OnBorder()
        {
            if (snake_head.X>window_width )
            {
                return Direction.Left;
            }
            if (snake_head.X < 0)
            {
                return Direction.Right;
            }
            if (snake_head.Y > window_height)
            {
                return Direction.Up;
            }
            if (snake_head.Y < 0)
            {
                return Direction.Down;
            }
            return Direction.pause;
        }
        private bool check_snake_bite()
        {
            foreach (var block in snake_body)
            {
                if (block.Intersects(snake_head))
                {

                    return true;
                }
            }
            return false;
        }
        private Vector2D generate_food()
        {
            
            Random rand = new Random();
            int x_pos = rand.Next(0, window_width-food_block_width);
            int y_pos = rand.Next(0, window_height-food_block_hieght );
            var tmp= new Rectangle(x_pos, y_pos, food_block_width, food_block_hieght);
            foreach (var block in snake_body)
            {
                if (block.Intersects(tmp))
                {
                    return generate_food();
                }
            }
            return new Vector2D(x_pos, y_pos);
        }
        private void moveNewDir(int graw_val)
        {

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                if (snake_diraction != Direction.Down)
                    if (Keyboard.GetState().IsKeyUp(Keys.Right) && Keyboard.GetState().IsKeyUp(Keys.Left))
                    {
                        snake_diraction = Direction.Up;

                    }


            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                if (snake_diraction != Direction.Up)
                    if (Keyboard.GetState().IsKeyUp(Keys.Right) && Keyboard.GetState().IsKeyUp(Keys.Left))
                    {
                        snake_diraction = Direction.Down;

                    }

            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                if (snake_diraction != Direction.Left)
                    if (Keyboard.GetState().IsKeyUp(Keys.Up) && Keyboard.GetState().IsKeyUp(Keys.Down))
                    {
                        snake_diraction = Direction.Right;

                    }

            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                if (snake_diraction != Direction.Right)
                    if (Keyboard.GetState().IsKeyUp(Keys.Up) && Keyboard.GetState().IsKeyUp(Keys.Down))
                    {
                        snake_diraction = Direction.Left;

                    }
            }
            
            Vector2D change_diraction = Directions.directions[(int)snake_diraction];
            for (int i = 0; i < graw_val; i++)
            {
                var lastPos = snakeBlocks.Last().Item2;
                snakeBlocks.Add((snakeBlock, new Vector2D(lastPos.x + change_diraction.x * snake_speed.x, lastPos.y + change_diraction.y * snake_speed.y)));
            }
            
            snakeBlocks.RemoveAt(0);
        }
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            snake_diraction = Direction.Right;
            _graphics.PreferredBackBufferWidth = window_width;
            _graphics.PreferredBackBufferHeight = window_height;
            _graphics.ApplyChanges();
            snake_food = new food(new Vector2D(0, 0), new Texture2D(GraphicsDevice, 1, 1));
            snake_speed = new Vector2D(snake_block_width, snake_block_hieght);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("File");
            // TODO: use this.Content to load your game content here
            snakeBlock = new Texture2D(GraphicsDevice, 1, 1);
            snakeBlock.SetData(new[] { Color.White });

            foodBlock = new Texture2D(GraphicsDevice, 1, 1);
            foodBlock.SetData(new[] { Color.Red });

            

            snakeBlocks = new List<(Texture2D,Vector2D)>();
            for (int i = 0; i < initial_lenght; i++)
            {
                snakeBlocks.Add((snakeBlock, new Vector2D(i*snake_block_width, 0)));
            }
            
            snake_food.pos = generate_food();
            snake_food.foodBlock = foodBlock;

            timer.Elapsed += (sender, e) =>
            {
                timerValue++;
            };
            timer.Start();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            moveNewDir(1);
            
            if (check_snake_bite())//if snake bite it self
            {
                Exit();
            }
            var snake_dir_after_map = check_snake_OnBorder();//check how to chnage diraction of snake out of board
            if (snake_dir_after_map==Direction.Down)
            {
                var pos = snakeBlocks.Last().Item2;
                snakeBlocks.Add((snakeBlock, new Vector2D(pos.x , window_height)));
                snakeBlocks.RemoveAt(0);
            }
            if (snake_dir_after_map == Direction.Up)
            {
                var pos = snakeBlocks.Last().Item2;
                snakeBlocks.Add((snakeBlock, new Vector2D(pos.x, 0)));
                snakeBlocks.RemoveAt(0);
            }
            if (snake_dir_after_map == Direction.Right)
            {
                var pos = snakeBlocks.Last().Item2;
                snakeBlocks.Add((snakeBlock, new Vector2D(window_width, pos.y)));
                snakeBlocks.RemoveAt(0);
            }
            if (snake_dir_after_map == Direction.Left)
            {
                var pos = snakeBlocks.Last().Item2;
                snakeBlocks.Add((snakeBlock, new Vector2D(0, pos.y)));
                snakeBlocks.RemoveAt(0);
            }
            if (food_rect.Intersects(snake_head))
            {
                snake_food.pos = generate_food();

                moveNewDir(3);// graw snake by 3 each food eaten
                points_counter+=200;
            }
            points_counter++;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();
            // snake rects
            foreach (var block in snakeBlocks)
            {
                _spriteBatch.Draw(block.Item1, new Rectangle((int)block.Item2.x, (int)block.Item2.y, snake_block_width, snake_block_hieght),
                            Color.Beige);
            }
            _spriteBatch.Draw(snake_food.foodBlock, new Rectangle(snake_food.pos.x, snake_food.pos.y, food_block_width, food_block_hieght),
                Color.Red);
            _spriteBatch.DrawString(font, "Timer: " + timerValue, new Vector2(10, 10), Color.White);
            _spriteBatch.DrawString(font, "points: " + points_counter, new Vector2(100, 10), Color.White);
            base.Draw(gameTime);
            _spriteBatch.End();
        }
    }
}