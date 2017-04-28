using System;

namespace SP.PowerBI.DB.Entities
{
    public class ColumnNameAttribute : Attribute
    {
        public string Name { get; }
        public ColumnNameAttribute(string name)
        {
            this.Name = name;
        }
        
    }
}