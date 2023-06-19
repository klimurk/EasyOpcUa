using Microsoft.Extensions.DependencyInjection;
using OpcUa.Client.Applications;
using OpcUa.Persistance;
using OpcUa.Persistance;

namespace OpcUa.ConsoleApp;

internal class Program
{
	private static IServiceProvider ServiceProvider;
	static void Main(string[] args)
	{
		//setup our DI
		ServiceProvider = BuildServiceProvider();
		var controller = ServiceProvider.GetRequiredService<OpcControllerService>();
		controller.CheckList();

		Console.WriteLine("Hello, World!");
		Console.ReadLine();
	}

	private static IServiceProvider BuildServiceProvider() => new ServiceCollection()
		.AddClientApplication()
		.AddPersistence()
		.BuildServiceProvider();
}