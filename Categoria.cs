public class Categoria
{
    public int id { get; set; }
    public string descricao { get; set; }
    public decimal valorPadraoDiaria { get; set; }
    public virtual ICollection<Veiculo> veiculos { get; set; }
}