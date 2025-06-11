namespace BackEnd.API.Data;

public partial class Txsk
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public int? ContactId { get; set; }
    public DateTime DueDate { get; set; }
    public int TxskTypeId { get; set; }
    public int TxskStatusId { get; set; }

    public virtual Contact? Contact { get; set; }
    public virtual TxskType TxskType { get; set; } = null!;
    public virtual TxskStatus TxskStatus { get; set; } = null!;
}
