using static System.Console;

namespace code
{
    public class Pointer
    {
        /* 
         * error: Unsafe code may only appear if compiling with /unsafe
         * 在code.csproj中,填加项 AllowUnSafeBlocks.
         */
        unsafe public static void Test()
        {
            "不安全的代码-指针".WriteTemplate();

            int x = 10;
            double z = 1.5;
            int* pX = &x;
            double* pZ = &z;

            WriteLine($"Address of x is 0x{(ulong)&x:X}, size is {sizeof(int)}, value is {x}");
            WriteLine($"Address of pX=&x is 0x{(ulong)&pX:X}, size is {sizeof(int*)}, value is 0x{(ulong)pX:X}");
            WriteLine($"Address of z is 0x{(ulong)&z:X}, size is {sizeof(double)}, value is {z}");
            WriteLine($"Address of pZ=&z is 0x{(ulong)&pZ:X}, size is {sizeof(double*)}, value is 0x{(ulong)pZ:X}");

            // 更改指针指向的地址所对应的值
            *pX = 20;
            WriteLine($"After setting *pX, x = {x}");
            WriteLine($"*pX = {*pX}");

            pZ = (double*)pX;
            WriteLine($"x treated as a double = {*pZ}");

            WriteLine("\nNow with struct");
            WriteLine($"Size of CurrencyStruct struct is {sizeof(CurrencyStruct)}");
            CurrencyStruct amount1, amount2;
            CurrencyStruct* pAmount = &amount1;
            long* pDollars = &(pAmount->Dollars);
            byte* pCents = &(pAmount->Cents);

            WriteLine($"Address of amount1 is 0x{(ulong)&amount1:X}");
            WriteLine($"Address of amount2 is 0x{(ulong)&amount2:X}");
            WriteLine($"Address of pAmount is 0x{(ulong)&pAmount:X}");
            WriteLine($"Address of pDollars is 0x{(ulong)&pDollars:X}");
            WriteLine($"Address of pCents is 0x{(ulong)&pCents:X}");
            pAmount->Dollars = 20;
            *pCents = 50;
            WriteLine($"amount1 contains {amount1}");

            --pAmount;   // this should get it to point to amount2
            WriteLine($"amount2 has address 0x{(ulong)pAmount:X} " +
                $"and contains {*pAmount}");

            // do some clever casting to get pCents to point to cents
            // inside amount2
            CurrencyStruct* pTempCurrency = (CurrencyStruct*)pCents;
            pCents = (byte*)(--pTempCurrency);
            WriteLine("Address of pCents is now 0x{0:X}", (ulong)&pCents);

            WriteLine("\nNow with classes");
            // now try it out with classes
            var amount3 = new CurrencyClass();

            // fixed 语句设置指向托管变量的指针，并在执行该语句期间"固定"此变量。这样就可以防止变量的重定位。
            // You can only take the address of an unfixed expression inside of a fixed statement initializer
            fixed (long* pDollars2 = &(amount3.Dollars))
            fixed (byte* pCents2 = &(amount3.Cents))
            {
                WriteLine($"amount3.Dollars has address 0x{(ulong)pDollars2:X}");
                WriteLine($"amount3.Cents has address 0x{(ulong)pCents2:X}");
                *pDollars2 = -100;
                WriteLine($"amount3 contains {amount3}");
            }
        }

        unsafe public static void QuickArray()
        {
            "用指针在栈中创建高性能,低系统开销的数组".WriteTemplate();
            // Write("How big an array do you want? \n> ");
            // string userInput = ReadLine();
            // int size = int.Parse(userInput);

            int size = 20;
            long* pArray = stackalloc long[size];
            for (int i = 0; i < size; i++)
            {
                pArray[i] = i * i;
            }

            for (int i = 0; i < size; i++)
            {
                WriteLine($"Element {i} = {*(pArray + i)}");
            }
        }
    }

    internal struct CurrencyStruct
    {
        public long Dollars;
        public byte Cents;

        public override string ToString() => $"{Dollars}.{Cents}";
    }

    internal class CurrencyClass
    {
        public long Dollars = 0;
        public byte Cents = 0;

        public override string ToString() => $"{Dollars}.{Cents}";
    }
}