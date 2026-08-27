public class Envelope
{
    /// <summary>
    /// The unique identifier of the envelope.
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// The Id of the user that owns this envelope's budget.
    /// </summary>
    public string UserId { get; set; } 
    
    /// <summary>
    /// Links to ONE account (many-to-one relationship).
    /// </summary>
    public int AccountId { get; set; }
    
    /// <summary>
    /// Reference to the category this envelope belongs to.
    /// </summary>
    public int CategoryId { get; set; }
    
    /// <summary>
    /// User-generated name for this budget (e.g., "Weekly Groceries").
    /// </summary>
    public string Name { get; set; } 
    
    /// <summary>
    /// Total accumulated budget (compound - never resets).
    /// </summary>
    public decimal Budget { get; set; } 
    
    /// <summary>
    /// Cumulative spending on this envelope.
    /// </summary>
    public decimal SpentAmount { get; set; } 
    
    /// <summary>
    /// Date when this budget was added (never resets).
    /// </summary>
    public DateTime DateCreated { get; set; } 
    
    /// <summary>
    /// Whether this envelope's budget is currently active.
    /// </summary>
    public bool IsActive { get; set; } = true;
}
