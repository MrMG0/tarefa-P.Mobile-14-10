namespace VanderClean.App.Services;

public static class SessionManager
{
    public static int EmpresaId { get; set; } = 1;
    public static string EmpresaNome { get; set; } = "VanderClean";
    public static bool IsLoggedIn { get; set; } = false;
}
