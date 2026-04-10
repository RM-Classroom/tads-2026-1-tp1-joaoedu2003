public class AluguelCreateDTO
{
    public int clienteId { get; set; }
    public int veiculoId { get; set; }
    public decimal valorDiaria { get; set; }
    // A data de saída geralmente é o momento do cadastro (DateTime.Now)
}
