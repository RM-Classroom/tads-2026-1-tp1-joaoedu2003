public class AluguelReadDTO
{
    public int id { get; set; }
    public string nomeCliente { get; set; } = string.Empty;
    public string cpfCliente { get; set; } = string.Empty;
    public string modeloVeiculo { get; set; } = string.Empty;
    public string fabricanteVeiculo { get; set; } = string.Empty;
    public DateTime dataSaida { get; set; }
    public DateTime? dataDevolucao { get; set; }
    public decimal? valorTotal { get; set; }
}
