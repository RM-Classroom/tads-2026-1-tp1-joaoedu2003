public class Veiculo
{
    public int id { get; set; }
    public string modelo { get; set; }
    public int anoFabricacao { get; set; }
    public int quilometragemAtual { get; set; }

    public int fabricanteId { get; set; }
    public virtual Fabricante fabricante { get; set; }

    public int categoriaId { get; set; }
    public virtual Categoria categoria { get; set; }
}