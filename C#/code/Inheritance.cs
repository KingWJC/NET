using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace code
{
    public class Inheritance
    {
        public static void Test()
        {
            "interface, abstract class, Constructor, virtual method, abstract method, extension method".WriteTemplate();
            Shape rec = new Rectangle(10, 10, 0, 0);
            rec.Name = rec.GetType().Name;
            rec.Draw();
            IAction action = rec;
            action[0].Move(new Point(10, 10));
            //扩展方法可继承,像静态方法
            rec.ComputeArea();
        }
    }

    /* 接口 */
    public interface IAction
    {
        //方法
        void Move(Point point);
        //属性
        decimal Area { get; }
        //索引器
        Shape this[int index] { get; }
    }

    /* 抽象父类 */
    public abstract class Shape : IAction
    {
        public string Name { get; set; }
        public Point Position { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        // 实现接口,只读,私有赋值.
        public decimal Area { get; private set;}
        public Shape this[int index] => this;
        public Shape(int width, int height, int x, int y)
        {
            this.Position = new Point(x, y);
            this.Width = width;
            this.Height = height;
        }

        public abstract void Draw();

        public virtual void Move(Point point)
        {
            this.Position = point;
            Console.WriteLine($"move to new position X:{Position.X},Y:{Position.Y}");
        }

        //抽象类不适合包含公共的非静态方法，因为不会被实例化，永远都不会调用。
        public override string ToString()
        {
            return "父类" + Name;
        }
    }

    /* 实现子类 */
    public class Rectangle : Shape
    {
        public Rectangle()
        : base(width: 0, height: 0, x: 0, y: 0)
        {

        }

        public Rectangle(int width, int height, int x, int y)
        : base(width, height, x, y)
        {

        }

        public override void Draw() => Console.WriteLine($"Drawing rectangle with X:{Position.X},Y:{Position.Y} and width:{Width},height:{Height}");

        public override void Move(Point p)
        {
            Console.WriteLine($"{Name} begin to move");
            base.Move(p);
        }

        public override string ToString()
        {
            return "子类" + Name;
        }
    }

    /* 抽象父类的扩展方法 */
    static class ExtendHelper
    {
        public static void ComputeArea(this Shape shape)
        {
            Console.WriteLine($"the area of shape is {shape.Width * shape.Height}");
        }
    }
}

