using SQLite;
using VanderClean.App.Models;

namespace VanderClean.App.Services;

public class LocalDbService
{
    private SQLiteAsyncConnection? _database;
    private readonly string _dbPath;

    public LocalDbService()
    {
        _dbPath = Path.Combine(FileSystem.AppDataDirectory, "vanderclean_local.db");
    }

    private async Task InitAsync()
    {
        if (_database is not null) return;

        _database = new SQLiteAsyncConnection(_dbPath);
        await _database.CreateTableAsync<EmpresaLocal>();
        await _database.CreateTableAsync<PedidoEstoqueLocal>();
        await _database.CreateTableAsync<ItemPedidoEstoqueLocal>();
    }

    // Empresa
    public async Task<EmpresaLocal?> ObterEmpresaAsync()
    {
        await InitAsync();
        return await _database!.Table<EmpresaLocal>().FirstOrDefaultAsync();
    }

    public async Task SalvarEmpresaAsync(EmpresaLocal empresa)
    {
        await InitAsync();
        var existente = await ObterEmpresaAsync();
        if (existente == null)
        {
            await _database!.InsertAsync(empresa);
        }
        else
        {
            empresa.Id = existente.Id;
            await _database!.UpdateAsync(empresa);
        }
    }

    // Pedidos Estoque
    public async Task<List<PedidoEstoqueLocal>> ObterPedidosAsync()
    {
        await InitAsync();
        return await _database!.Table<PedidoEstoqueLocal>().OrderByDescending(p => p.Id).ToListAsync();
    }

    public async Task<PedidoEstoqueLocal?> ObterPedidoPorIdAsync(int id)
    {
        await InitAsync();
        return await _database!.Table<PedidoEstoqueLocal>().FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<ItemPedidoEstoqueLocal>> ObterItensDoPedidoAsync(int idPedidoEstoque)
    {
        await InitAsync();
        return await _database!.Table<ItemPedidoEstoqueLocal>()
            .Where(i => i.IdPedidoEstoque == idPedidoEstoque)
            .ToListAsync();
    }

    public async Task<int> SalvarPedidoAsync(PedidoEstoqueLocal pedido, List<ItemPedidoEstoqueLocal> itens)
    {
        await InitAsync();
        if (pedido.Id == 0)
        {
            await _database!.InsertAsync(pedido);
        }
        else
        {
            await _database!.UpdateAsync(pedido);
            await _database.ExecuteAsync("DELETE FROM ItemPedidoEstoqueLocal WHERE IdPedidoEstoque = ?", pedido.Id);
        }

        foreach (var item in itens)
        {
            item.IdPedidoEstoque = pedido.Id;
            await _database!.InsertAsync(item);
        }

        return pedido.Id;
    }

    public async Task MarcarPedidoSincronizadoAsync(int localId, int apiId)
    {
        await InitAsync();
        var pedido = await ObterPedidoPorIdAsync(localId);
        if (pedido != null)
        {
            pedido.IdPedidoApi = apiId;
            pedido.SincronizadoApi = 1;
            await _database!.UpdateAsync(pedido);
        }
    }
}
