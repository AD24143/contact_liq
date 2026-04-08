using System.Collections.Generic;

namespace contact_liq;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public virtual List<Contact> Contacts { get; set; } = new();
}
