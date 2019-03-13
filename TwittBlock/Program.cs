using LinqToTwitter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TwittBlock.WebTwitter;

namespace TwittBlock
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] links = new string[] { "topicza.com", "socialhot24.com", "darachilli.com", "starnews2day.com", "you-health.net", "lekded369.com", "startclip.com" };
            var blocker = new Blocker(links);
            Console.Write("Enter user name: ");
            string userName = Console.ReadLine();
            Console.Write("Enter password: ");
            string password = Console.ReadLine();
            blocker.StartScan(userName, password).Wait();
        }
    }
}
