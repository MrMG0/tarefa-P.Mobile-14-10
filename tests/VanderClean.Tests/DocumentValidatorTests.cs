using VanderClean.Core.Utils;
using Xunit;

namespace VanderClean.Tests;

public class DocumentValidatorTests
{
    [Theory]
    [InlineData("12.345.678/0001-90", false)] // CNPJ fictício não passa nos dígitos
    [InlineData("11.222.333/0001-81", true)]  // CNPJ matematicamente válido
    [InlineData("11222333000181", true)]      // CNPJ sem máscara válido
    [InlineData("00000000000000", false)]     // Dígitos repetidos inválidos
    [InlineData("123", false)]
    public void ValidarCnpj_DeveValidarCorretamente(string cnpj, bool esperado)
    {
        var resultado = DocumentValidator.ValidarCnpj(cnpj);
        Assert.Equal(esperado, resultado);
    }

    [Theory]
    [InlineData("111.444.777-35", true)]  // CPF matematicamente válido
    [InlineData("11144477735", true)]    // CPF sem pontuação válido
    [InlineData("111.111.111-11", false)] // CPFs repetidos
    [InlineData("123.456.789-00", false)] // Dígitos verificadores incorretos
    public void ValidarCpf_DeveValidarCorretamente(string cpf, bool esperado)
    {
        var resultado = DocumentValidator.ValidarCpf(cpf);
        Assert.Equal(esperado, resultado);
    }
}
