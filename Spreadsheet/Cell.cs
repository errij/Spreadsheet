using SpreadSheetMethods;
using SpreadsheetUtilities;

namespace SpreadSheetCell
{
    /// <summary>
    /// This class represents a cell of Spreadsheet
    /// A cell conatins its name, value, and content.
    /// 
    /// <para>
    /// Content can be either string, double, or Formula.
    /// </para>
    /// 
    /// </summary>
    public class Cell
    {
        private string name;
        private object value;
        private object content;

        /// <summary>
        /// Setup a cell with content
        /// </summary>
        /// <param name="name">cell name</param>
        /// <param name="content">content to cell. Can be either double, string, formula, or error</param>
        /// <exception cref="ArgumentException">if name or content is invalid</exception>
        public Cell(string name, object content)
        {
            if (!Extension.CheckVarName(name)) throw new ArgumentException($"Invalid name: {name}");
            checkContent(content);
            this.name = name;
            this.content = content;
        }

        /// <summary>
        /// Get name of a cell
        /// </summary>
        /// <returns>name</returns>
        public string getName()
        {
            return name;
        }

        /// <summary>
        /// Get value of a cell
        /// </summary>
        /// <returns>value</returns>
        public object getValue()
        {
            return value;
        }

        /// <summary>
        /// Get content of a cell
        /// </summary>
        /// <returns>content</returns>
        public object getContent()
        {
            return content;
        }

        /// <summary>
        /// Set value of a cell
        /// </summary>
        /// <param name="value">Value to set</param>
        public void setValue(object value)
        {
            this.value = value;
        }

        /// <summary>
        /// retrieve a hashcode of the cell using content and name
        /// </summary>
        /// <returns>Hashcode</returns>
        public override int GetHashCode()
        {
            string st = name + content.ToString();

            return st.GetHashCode();  
        }

        /// <summary>
        /// determine equality of cell using hashcode
        /// </summary>
        /// <param name="obj">object to compare</param>
        /// <returns>true if equal false otherwise</returns>
        public override bool Equals(object? obj)
        {
            return this.GetHashCode() == obj.GetHashCode();
        }

        /// <summary>
        /// Check content is valid.
        /// See class header
        /// </summary>
        /// <param name="content">Content to check</param>
        /// <exception cref="ArgumentException">If content is invalid</exception>
        private void checkContent(object content)
        {
            if (!(content is string || content is double || content is Formula)) throw new ArgumentException($"content is invalid! : {content}");
        }
    }
}
