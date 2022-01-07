﻿namespace Chloe.Oracle
{
    public class DataTypeAttribute : Attribute
    {
        public DataTypeAttribute()
        {
        }
        public DataTypeAttribute(string name)
        {
            this.Name = name;
        }

        public string Name { get; set; }
    }
}
