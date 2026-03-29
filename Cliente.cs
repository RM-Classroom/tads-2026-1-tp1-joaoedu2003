public class Cliente
{
    public int id { get; set; }
    public string nome { get; set; }
    public string cpf { get; set; }
    public string email { get; set; }
    public virtual ICollection<Aluguel> alugueis { get; set; }
}