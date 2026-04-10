public class Cliente
{
    public int id { get; set; }
    public string nome { get; set; } = string.Empty;
    public string cpf { get; set; } = string.Empty;
    public string email { get; set; } = string.Empty;

    public virtual ICollection<Aluguel> alugueis { get; set; } = new List<Aluguel>();
}
