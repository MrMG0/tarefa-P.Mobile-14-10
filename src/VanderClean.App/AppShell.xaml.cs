using VanderClean.App.Views;

namespace VanderClean.App;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(RegistrarEmpresaPage), typeof(RegistrarEmpresaPage));
		Routing.RegisterRoute(nameof(NovoPedidoPage), typeof(NovoPedidoPage));
	}
}
