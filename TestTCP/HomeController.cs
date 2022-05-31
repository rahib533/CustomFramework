using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTCP
{
    public class HomeController
    {
        public string Index()
        {
            return "It works";
        }

        public int Index2()
        {
            return 5;
        }

        public Person GetName()
        {
            return new Person { Name = "Rahib", Surname = "Jafarov" };
        }
    }

    public class Person
    {
        public string Name { get; set; }
        public string Surname { get; set; }
    }
}
