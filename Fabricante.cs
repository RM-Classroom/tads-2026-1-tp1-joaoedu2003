public class Fabricante
{
    public int id { get; set; }
    public string nome { get; set; }
    public virtual ICollection<Veiculo> veiculos { get; set; }
}
