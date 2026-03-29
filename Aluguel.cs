public class Aluguel
{
    public int id { get; set; }

    public int clienteId { get; set; }
    public virtual Cliente cliente { get; set; }
    
    public int veiculoId { get; set; }
    public virtual Veiculo veiculo { get; set; }

    public DateTime dataSaida { get; set; }
    public DateTime? dataDevolucao { get; set; }
    
    public int kmInicial { get; set; }
    public int? kmFinal { get; set; }
    
    public decimal valorDiaria { get; set; }
    public decimal? valorTotal { get; set; }
}
