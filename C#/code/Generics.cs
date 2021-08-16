using System;
using System.Collections;
using System.Collections.Generic;
using static System.Console;

namespace code
{
    public class Generics
    {
        public static void Test()
        {
            "测试泛型类的静态属性".WriteTemplate();
            GenericClass<int>.Name = "wjc";
            GenericClass<string>.Name = "wy";
            Console.WriteLine($"int class : {GenericClass<int>.Name}, string class : {GenericClass<string>.Name}");

            "双向泛型链表".WriteTemplate();
            var list3 = new LinkedList<string>();
            list3.AddLast("2");
            list3.AddLast("four");
            list3.AddLast("foo");

            foreach (string s in list3)
            {
                Console.WriteLine(s);
            }

            "泛型方法 - 重载".WriteTemplate();
            var test = new MethodOverloads();
            test.Foo(33); //编译时，找类型明确的
            test.Foo("abc");
            test.Foo("abc", 42);
            test.Foo(33, "abc");
            test.Bar(44); //运行时，找泛型参数的
            test.Foo(new List<LinkedListNode<int>>());
            test.Foo<string>(new List<string>());
            test.Foo(new List<string>());

            "泛型接口 - 协变和逆变".WriteTemplate();
            IOut<Rectangle> rectangles = RectangleCollection.GetRectangles();
            //Cannot implicitly convert type 'code.IOut<code.Rectangle>' to 'code.IOut<code.Shape>'
            IOut<Shape> shapes = rectangles;
            for (int i = 0; i < shapes.Count; i++)
            {
                Console.WriteLine(shapes[i]);
            }

            IIn<Shape> shapeShow = new ShapeShow();
            //cannot implicitly convert type 'code.IIn<code.Shape>' to 'code.IIn<code.Rectangle>'
            IIn<Rectangle> rectangleShow = shapeShow;
            rectangleShow.Show(rectangles[0]);

            "泛型委托 - 协变和逆变".WriteTemplate();
            // 很明显actionShape 是让动物叫，因为Dog是Animal，那么既然Animal 都能叫，Dog肯定也能叫
            Action<Shape> actionShape = new Action<Shape>(a => WriteLine($"shape width:{a.Width}, height:{a.Height}"));
            Action<Rectangle> actionRec = actionShape;
            actionShape(rectangles[0]);
        }


    }

    /* 泛型类 - 静态 */
    class GenericClass<T>
    {
        public static string Name { get; set; }
    }

    /* 泛型类 - 链表元素 */
    class LinkedListNode<T>
    {
        public T Value { get; set; }
        public LinkedListNode<T> Prev { get; set; }
        public LinkedListNode<T> Next { get; set; }

        public LinkedListNode(T value)
        {
            Value = value;
        }
    }

    /* 泛型类 - 双向链表 */
    class LinkedList<T> : IEnumerable<T>
    {
        public LinkedListNode<T> FirstNode { get; set; }
        public LinkedListNode<T> LastNode { get; set; }

        public LinkedListNode<T> AddLast(T value)
        {
            var newNode = new LinkedListNode<T>(value);
            if (FirstNode != null)
            {
                LastNode.Next = newNode;
                newNode.Prev = LastNode;
                LastNode = newNode;
            }
            else
            {
                FirstNode = newNode;
                LastNode = FirstNode;
            }
            return newNode;
        }

        public IEnumerator<T> GetEnumerator()
        {
            LinkedListNode<T> current = FirstNode;
            while (current != null)
            {
                yield return current.Value;
                current = current.Next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    /* 泛型方法 - 重载 */
    public class MethodOverloads
    {
        public void Foo<T>(T obj)
        {
            WriteLine($"Foo<T>(T obj), obj type: {obj.GetType().Name}");
        }

        public void Foo(int x)
        {
            WriteLine("Foo(int x)");
        }

        public void Foo<T1, T2>(T1 obj1, T2 obj2)
        {
            WriteLine($"Foo<T1, T2>(T1 obj1, T2 obj2); {obj1.GetType().Name} {obj2.GetType().Name}");
        }

        public void Foo<T>(int obj1, T obj2)
        {
            WriteLine($"Foo<T>(int obj1, T obj2); {obj2.GetType().Name}");
        }

        public void Foo(List<String> list)
        {
            WriteLine($"Foo(List<String> obj), obj type: {list.GetType().Name}");
        }

        public void Foo<T>(List<T> list)
        {
            WriteLine($"Foo<T>(List<T> obj), obj type: {list.GetType().Name}");
        }

        public void Bar<T>(T obj)
        {
            Foo(obj);
        }
    }

    /* 泛型委托 */
    public delegate void Action<in T>(T obj);

    // covariant
    public interface IOut<out T>
    {
        T this[int index] { get; }
        int Count { get; }
    }

    public class RectangleCollection : IOut<Rectangle>
    {
        private Rectangle[] data = new Rectangle[2]
        {
      new Rectangle { Height=2, Width=5 },
      new Rectangle { Height=3, Width=7}
        };

        private static RectangleCollection coll;
        public static RectangleCollection GetRectangles() => coll ?? (coll = new RectangleCollection());

        public Rectangle this[int index]
        {
            get
            {
                if (index < 0 || index > data.Length)
                    throw new ArgumentOutOfRangeException("index");
                return data[index];
            }
        }
        public int Count => data.Length;
    }

    // contra-variant
    public interface IIn<in T>
    {
        void Show(T item);
    }

    public class ShapeShow : IIn<Shape>
    {
        public void Show(Shape s) => WriteLine($"{s.GetType().Name} Width: {s.Width}, Height: {s.Height}");
    }
}