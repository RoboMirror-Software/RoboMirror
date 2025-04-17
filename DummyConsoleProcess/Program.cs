using System;
using System.Collections.Generic;
using System.Text;

namespace DummyConsoleProcess
{
	class Program
	{
		static void Main(string[] args)
		{
			Environment.ExitCode = -1;

			// for 100 seconds:
			for (int i = 1; i <= 100 * 10; ++i)
			{
				Console.WriteLine(i);
				System.Threading.Thread.Sleep(100);
			}
		}
	}
}
