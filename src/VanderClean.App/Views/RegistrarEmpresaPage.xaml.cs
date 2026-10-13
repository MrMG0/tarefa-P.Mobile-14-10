using VanderClean.App.Models;
using VanderClean.App.Services;
using VanderClean.Core.DTOs;
using VanderClean.Core.Utils;

namespace VanderClean.App.Views;

public partial class RegistrarEmpresaPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly LocalDbService _localDbService;

    public RegistrarEmpresaPage()
    {
        InitializeComponent();
        _apiService = new ApiService();
        _localDbService = new LocalDbService();
    }

    private async void OnSalvarClicked(object sender, EventArgs e)
    {
        var razao = RazaoSocialEntry.Text?.Trim();
        var cnpj = CnpjEntry.Text?.Trim();
        var email = EmailEntry.Text?.Trim();
        var senha = SenhaEntry.Text?.Trim();

        if (string.IsNullOrWhiteSpace(razao) || string.IsNullOrWhiteSpace(cnpj) ||
            string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
        {
            await DisplayAlert("Atenção", "Preencha os campos obrigatórios (*).", "OK");
            return;
        }

        if (!DocumentValidator.ValidarCnpj(cnpj))
        {
            await DisplayAlert("CNPJ Inválido", "Por favor informe um CNPJ válido.", "OK");
            return;
        }

        BtnSalvar.IsEnabled = false;

        var dto = new EmpresaRegistroDto
        {
            RazaoSocial = razao,
            NomeFantasia = NomeFantasiaEntry.Text?.Trim(),
            Cnpj = cnpj,
            Email = email,
            Telefone = TelefoneEntry.Text?.Trim(),
            Cep = CepEntry.Text?.Trim(),
            Cidade = CidadeEntry.Text?.Trim(),
            Uf = UfEntry.Text?.Trim(),
            Senha = senha
        };

        // Salva localmente no SQLite
        var localEmpresa = new EmpresaLocal
        {
            RazaoSocial = dto.RazaoSocial,
            NomeFantasia = dto.NomeFantasia,
            Cnpj = dto.Cnpj,
            Email = dto.Email,
            Telefone = dto.Telefone,
            Cep = dto.Cep,
            Cidade = dto.Cidade,
            Uf = dto.Uf,
            Senha = dto.Senha,
            SincronizadoApi = 0
        };

        await _localDbService.SalvarEmpresaAsync(localEmpresa);

        // Tenta enviar para a API
        var resultado = await _apiService.RegistrarEmpresaAsync(dto);

        if (resultado.Sucesso && resultado.Empresa != null)
        {
            localEmpresa.SincronizadoApi = 1;
            await _localDbService.SalvarEmpresaAsync(localEmpresa);

            SessionManager.EmpresaId = resultado.Empresa.Id;
            SessionManager.IsLoggedIn = true;

            await DisplayAlert("Sucesso", "Empresa cadastrada e sincronizada!", "OK");
            await Shell.Current.GoToAsync("//PedidosPage");
        }
        else
        {
            await DisplayAlert("Aviso", "Salvo localmente. Não foi possível conectar à API imediatamente.", "Continuar");
            SessionManager.EmpresaId = localEmpresa.Id;
            SessionManager.IsLoggedIn = true;
            await Shell.Current.GoToAsync("//PedidosPage");
        }

        BtnSalvar.IsEnabled = true;
    }
}
