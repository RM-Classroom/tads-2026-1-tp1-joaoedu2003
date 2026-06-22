public class VeiculoCreateDTO
{
    public string modelo { get; set; } = string.Empty;
    public int anoFabricacao { get; set; }
    public int quilometragemAtual { get; set; }
    public int fabricanteId { get; set; }
    public int categoriaId { get; set; }
}
