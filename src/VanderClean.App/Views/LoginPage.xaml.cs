using VanderClean.App.Services;

namespace VanderClean.App.Views;

public partial class LoginPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly LocalDbService _localDbService;

    public LoginPage()
    {
        InitializeComponent();
        _apiService = new ApiService();
        _localDbService = new LocalDbService();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        var email = EmailEntry.Text?.Trim();
        var senha = SenhaEntry.Text?.Trim();

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
        {
            await DisplayAlert("Atenção", "Preencha o e-mail e a senha.", "OK");
            return;
        }

        LoadingIndicator.IsVisible = true;
        LoadingIndicator.IsRunning = true;
        BtnEntrar.IsEnabled = false;

        try
        {
            var resultado = await _apiService.LoginAsync(email, senha);

            if (resultado.Sucesso && resultado.EmpresaId.HasValue)
            {
                SessionManager.EmpresaId = resultado.EmpresaId.Value;
                SessionManager.IsLoggedIn = true;

                // Navegação segura via Shell
                await Shell.Current.GoToAsync("//PedidosPage");
            }
            else
            {
                // Fallback offline: verifica se existe empresa gravada no SQLite
                var empresaLocal = await _localDbService.ObterEmpresaAsync();
                if (empresaLocal != null && empresaLocal.Email.Equals(email, StringComparison.OrdinalIgnoreCase))
                {
                    SessionManager.EmpresaId = empresaLocal.Id;
                    SessionManager.IsLoggedIn = true;
                    await DisplayAlert("Modo Offline", "Conectado localmente via SQLite.", "OK");
                    await Shell.Current.GoToAsync("//PedidosPage");
                }
                else
                {
                    await DisplayAlert("Falha no Login", resultado.Mensagem, "OK");
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", ex.Message, "OK");
        }
        finally
        {
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
            BtnEntrar.IsEnabled = true;
        }
    }

    private async void OnRegistrarClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(RegistrarEmpresaPage));
    }
}
