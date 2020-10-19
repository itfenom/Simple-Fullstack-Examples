using System;

namespace Playground.ConsoleApp.DelegateEx
{
    // ReSharper disable once IdentifierTypo
    public class DelagatedEvent
    {
        // ReSharper disable once IdentifierTypo
        public DelagatedEvent()
        {
            var adder = new Adder();
            adder.OnMultipleOfFiveReached += Adder_OnMultipleOfFiveReached;

            adder.Add(4, 3);
            adder.Add(4, 6);
            adder.Add(5, 5);
            adder.Add(5, 20);
        }

        private void Adder_OnMultipleOfFiveReached(object sender, MultipleOfFiveEventArgs e)
        {
            Console.WriteLine("Multiple of five reached: {0} + {1} = {2}", e.Num1, e.Num2, e.Total);
        }
    }

    public class Adder
    {
        //public event EventHandler OnMultipleOfFiveReached;
        public EventHandler<MultipleOfFiveEventArgs> OnMultipleOfFiveReached;

        public int Add(int a, int b)
        {
            var sum = a + b;

            if (sum % 5 == 0 && OnMultipleOfFiveReached != null)
            {
                //OnMultipleOfFiveReached(this, EventArgs.Empty);
                OnMultipleOfFiveReached(this, new MultipleOfFiveEventArgs(a, b, sum));
            }
            else
            {
                Console.WriteLine("Adding {0} + {1}", a, b);
            }

            return sum;
        }
    }

    public class MultipleOfFiveEventArgs : EventArgs
    {
        public int Total { get; set; }

        public int Num1 { get; set; }

        public int Num2 { get; set; }

        public MultipleOfFiveEventArgs(int num1, int num2, int total)
        {
            Num1 = num1;
            Num2 = num2;
            Total = total;
        }
    }
}
