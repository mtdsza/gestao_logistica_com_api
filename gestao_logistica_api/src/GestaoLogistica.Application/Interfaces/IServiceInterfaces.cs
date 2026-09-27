using GestaoLogistica.Application.Common.Models;
using GestaoLogistica.Application.DTOs.Auth;
using GestaoLogistica.Application.DTOs.ItemInventario;
using GestaoLogistica.Application.DTOs.Motorista;
using GestaoLogistica.Application.DTOs.Transportadora;
using GestaoLogistica.Application.DTOs.Veiculo;
using GestaoLogistica.Application.DTOs.Viagem;

namespace GestaoLogistica.Application.Interfaces;

// contrato de caso de uso para autenticação, cadastro e permissões de usuários
public interface IAuthService
{
    // autentica credenciais, valida status ativo e devolve token jwt assinado
    Task<ResultWrapper<LoginResponseDto>> LoginAsync(LoginRequestDto request);

    // cadastra novo usuário com senha criptografada via bcrypt
    Task<ResultWrapper<UsuarioDto>> RegisterAsync(RegisterRequestDto request);

    // lista todos os usuários cadastrados no sistema
    Task<ResultWrapper<List<UsuarioDto>>> GetAllUsuariosAsync();

    // ativa ou inativa o acesso de determinado usuário
    Task<ResultWrapper<bool>> UpdateAtivoAsync(int id, bool ativo);
}

// contrato de caso de uso para gerenciamento de empresas transportadoras
public interface ITransportadoraService
{
    Task<ResultWrapper<List<TransportadoraDto>>> GetAllAsync();
    Task<ResultWrapper<TransportadoraDto>> GetByIdAsync(int id);
    Task<ResultWrapper<TransportadoraDto>> CreateAsync(CreateTransportadoraDto request);
    Task<ResultWrapper<TransportadoraDto>> UpdateAsync(int id, UpdateTransportadoraDto request);
    Task<ResultWrapper<bool>> DeleteAsync(int id);
}

// contrato de caso de uso para motoristas
public interface IMotoristaService
{
    Task<ResultWrapper<List<MotoristaDto>>> GetAllAsync();
    Task<ResultWrapper<List<MotoristaDto>>> GetAvailableAsync();
    Task<ResultWrapper<MotoristaDto>> GetByIdAsync(int id);
    Task<ResultWrapper<MotoristaDto>> CreateAsync(CreateMotoristaDto request);
    Task<ResultWrapper<MotoristaDto>> UpdateAsync(int id, UpdateMotoristaDto request);
    Task<ResultWrapper<bool>> DeleteAsync(int id);
}

// contrato de caso de uso para veículos
public interface IVeiculoService
{
    Task<ResultWrapper<List<VeiculoDto>>> GetAllAsync();
    Task<ResultWrapper<List<VeiculoDto>>> GetAvailableAsync();
    Task<ResultWrapper<VeiculoDto>> GetByIdAsync(int id);
    Task<ResultWrapper<VeiculoDto>> CreateAsync(CreateVeiculoDto request);
    Task<ResultWrapper<VeiculoDto>> UpdateAsync(int id, UpdateVeiculoDto request);
    Task<ResultWrapper<bool>> DeleteAsync(int id);
}

// contrato de caso de uso para estoque com suporte a cache
public interface IItemInventarioService
{
    // busca catálogo de itens com mecanismo de cache (cache hit / cache miss)
    Task<ResultWrapper<List<ItemInventarioDto>>> GetAllAsync();
    Task<ResultWrapper<ItemInventarioDto>> GetByIdAsync(int id);

    // mutações que causam invalidação reativa da memória cache
    Task<ResultWrapper<ItemInventarioDto>> CreateAsync(CreateItemInventarioDto request);
    Task<ResultWrapper<ItemInventarioDto>> UpdateAsync(int id, UpdateItemInventarioDto request);
    Task<ResultWrapper<bool>> DeleteAsync(int id);
    Task<ResultWrapper<bool>> UpdateStockAsync(int id, int novaQuantidade);
}

// contrato de caso de uso para viagens, fretes e métricas operacionais
public interface IViagemService
{
    Task<ResultWrapper<List<ViagemDto>>> GetAllAsync();
    Task<ResultWrapper<ViagemDto>> GetByIdAsync(int id);
    Task<ResultWrapper<List<ViagemDto>>> GetByMotoristaAsync(int idMotorista);
    Task<ResultWrapper<List<ViagemDto>>> GetHistoryByMotoristaAsync(int idMotorista);
    Task<ResultWrapper<List<ViagemDto>>> GetHistoryByVeiculoAsync(int idVeiculo);
    Task<ResultWrapper<List<ViagemDto>>> GetHistoryByTransportadoraAsync(int idTransportadora);

    // consolida faturamento, viagens em trânsito e eficiência no prazo para a gerência
    Task<ResultWrapper<DashboardMetricasDto>> GetDashboardMetricasAsync();

    // calcula a distância geodésica e sugere o valor de frete baseado no peso e na rota
    Task<ResultWrapper<CalcularFreteResponseDto>> CalcularFreteAsync(CalcularFreteRequestDto request);

    // cria a viagem validando capacidade do veículo, disponibilidade e debitando estoque
    Task<ResultWrapper<ViagemDto>> CreateAsync(CreateViagemDto request);

    // transiciona o status (ao concluir, libera o motorista e o veículo)
    Task<ResultWrapper<bool>> UpdateStatusAsync(int id, string novoStatus);

    // marca ou desmarca a viagem como faturada
    Task<ResultWrapper<bool>> UpdateFaturadoAsync(int id, bool faturado);
}
