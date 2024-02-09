using SpreadSheetMethods;

namespace SpreadSheetCell
{
    public class Cell
    {
        private string name;
        private object value;
        private object content;

        /// <summary>
        /// Setup a cell with content
        /// </summary>
        /// <param name="content">either formula, string, or error</param>
        public Cell(string name, object content)
        {
            if(@Methods.CheckVarName(name)) throw new ArgumentException($"Invalid name: {name}");
            this.name = name;
            this.content = content;
        }

        public string getName()
        {
            return name;
        }

        public object getValue()
        {
            return value;
        }

        public object getContent()
        {
            return content;
        }

        public void setValue(object value)
        {
            this.value = value;
        }
    }
}
