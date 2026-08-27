public class Category
{
    /// <summary>
    /// The unique identifier of the category.
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// User-generated name for this category (e.g., "Weekly Groceries").
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// System category identifier (e.g., "Groceries", "Rent/Mortgage").
    /// </summary>
    public string SystemCategory { get; set; }
}
