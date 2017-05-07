namespace ExcelHelper.Common.Models
{
    public class ListItemModel
    {

        public ListItemModel(string name , object value)
        {
            Name = name;
            Value = value;
        }


        public string Name { get; set; }

        public object Value { get; set; }


        public override string ToString()
        {
            return Name;
        }
    }
}