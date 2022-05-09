using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutofacToDi
{
    public interface ISomeOtherDependency
    {
        void Foo();
    }
    public class SomeOtherDependency : ISomeOtherDependency
    {
        public void Foo()
        {
            Console.WriteLine("Hello World");
        }
    }
}
