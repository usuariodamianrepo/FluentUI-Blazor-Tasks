namespace BackEnd.API.Data;

public partial class Txsk
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public int? ContactId { get; set; }
    public DateTime DueDate { get; set; }
    public int TaskTypeId { get; set; } // suggestion: TaskTypeId property name is more clear than TxskTypeId and does not affect any keyword like Task (which Txsk is understandable )
    public int TxskStatusId { get; set; } // same above

    public virtual Contact? Contact { get; set; }
    public virtual TxskType TaskTxskType { get; set; } = null!; // same above
    public virtual TxskStatus TxskStatus { get; set; } = null!;
}
