using SpreadsheetUtilities;
using SpreadSheetCell;
using System.Transactions;
using SpreadSheetMethods;

namespace SS
{
    /// <summary>
    /// this is a child class of AbstractSpreadSheet
    /// see AbstractSpreadSheet for further explanation
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {
        private DependencyGraph graph; //dependency graph of this class
        private HashSet<Cell> cells; //HashSet of the cells

        /// <summary>
        /// Constructor of this class
        /// </summary>
        public Spreadsheet()
        {
            this.cells = new HashSet<Cell>();
            this.graph = new DependencyGraph();
        }

        /// <summary>
        ///   Returns the contents (as opposed to the value) of the named cell.
        /// </summary>
        /// 
        /// <exception cref="InvalidNameException"> 
        ///   Thrown if the name is null or invalid
        /// </exception>
        /// 
        /// <param name="name">The name of the spreadsheet cell to query</param>
        /// 
        /// <returns>
        ///   The return value should be either a string, a double, or a Formula.
        ///   See the AbstractSpreadsheet header
        /// </returns>
        public override object GetCellContents(string name)
        {
            if (!Methods.CheckVarName(name)) throw new InvalidNameException(); //check name

            foreach (var cell in cells)
            {
                if (cell.getName().Equals(name)) return cell.getContent(); //return a cell's content
            }

            return null; //return null if there is no "name" cell
        }

        /// <summary>
        /// Returns an Enumerable that can be used to enumerates 
        /// the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            foreach (var cell in cells)
            {
                if (checkEmptyContent(cell)) yield return cell.getName(); //if content is not empty
            }
        }

        /// <summary>
        ///  Set the contents of the named cell to the given number.  
        /// </summary>
        /// 
        /// <exception cref="InvalidNameException"> 
        ///   If the name is null or invalid, throw an InvalidNameException
        /// </exception>
        /// 
        /// <param name="name"> The name of the cell </param>
        /// <param name="number"> The new contents/value </param>
        /// 
        /// <returns>
        ///   <para>
        ///      The method returns a set consisting of name plus the names of all other cells whose value depends, 
        ///      directly or indirectly, on the named cell.
        ///   </para>
        /// 
        ///   <para>
        ///      For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        ///      set {A1, B1, C1} is returned.
        ///   </para>
        /// </returns>
        public override ISet<string> SetCellContents(string name, double number)
        {
            CheckAndremoveCell(name); //remove existing cell to update

            try
            {
                cells.Add(new Cell(name, number));
            }
            catch
            {
                throw new InvalidNameException();
            }

            ISet<string> set = new HashSet<string>(getAllDependees(name)); //set of dependents of "name" cell
            return set;
        }

        /// <summary>
        /// The contents of the named cell becomes the text.  
        /// </summary>
        /// 
        /// <exception cref="ArgumentNullException"> 
        ///   If text is null, throw an ArgumentNullException.
        /// </exception>
        /// 
        /// <exception cref="InvalidNameException"> 
        ///   If the name is null or invalid, throw an InvalidNameException
        /// </exception>
        /// 
        /// <param name="name"> The name of the cell </param>
        /// <param name="text"> The new content/value of the cell</param>
        /// 
        /// <returns>
        ///   The method returns a set consisting of name plus the names of all 
        ///   other cells whose value depends, directly or indirectly, on the 
        ///   named cell.
        /// 
        ///   <para>
        ///     For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        ///     set {A1, B1, C1} is returned.
        ///   </para>
        /// </returns>
        public override ISet<string> SetCellContents(string name, string text)
        {
            if (text == null) throw new ArgumentNullException("text is null!");
            CheckAndremoveCell(name); //remove existing cell to update

            try
            {
                cells.Add(new Cell(name, text));
            }
            catch
            {
                throw new InvalidNameException();
            }

            ISet<string> set = new HashSet<string>(getAllDependees(name)); //set of dependents of "name" cell
            return set;
        }

        /// <summary>
        /// Set the contents of the named cell to the formula.  
        /// </summary>
        /// 
        /// <exception cref="ArgumentNullException"> 
        ///   If formula parameter is null, throw an ArgumentNullException.
        /// </exception>
        /// 
        /// <exception cref="InvalidNameException"> 
        ///   If the name is null or invalid, throw an InvalidNameException
        /// </exception>
        /// 
        /// <exception cref="CircularException"> 
        ///   If changing the contents of the named cell to be the formula would 
        ///   cause a circular dependency, throw a CircularException.  
        ///   (NOTE: No change is made to the spreadsheet.)
        /// </exception>
        /// 
        /// <param name="name"> The cell name</param>
        /// <param name="formula"> The content of the cell</param>
        /// 
        /// <returns>
        ///   <para>
        ///     The method returns a Set consisting of name plus the names of all other 
        ///     cells whose value depends, directly or indirectly, on the named cell.
        ///   </para>
        ///   <para> 
        ///     For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        ///     set {A1, B1, C1} is returned.
        ///   </para>
        /// 
        /// </returns>
        public override ISet<string> SetCellContents(string name, Formula formula)
        {
            object temp = GetCellContents(name); //temporary data to retrieve the value

            using (TransactionScope scope = new TransactionScope()) //using scope to make sure
            {
                try
                {
                    CheckAndremoveCell(name); //remove existing cell to update
                    cells.Add(new Cell(name, formula));
                    IEnumerator<string> e = formula.GetVariables().GetEnumerator(); //get variables formula contains
                    while (e.MoveNext()) graph.AddDependency(name, e.Current); //iterate and add dependency to the graph

                    GetCellsToRecalculate(name); //check for circular dependency

                    scope.Complete(); //complete the scope
                }
                catch (CircularException ex) //if a circular dependency detected
                { 
                    cells.Remove(new Cell(name, formula)); //remove added cell

                    if (temp != null) cells.Add(new Cell(name, temp)); //if there was an old value, retrieve it

                    IEnumerator<string> e = formula.GetVariables().GetEnumerator(); //get variables formula contains
                    while (e.MoveNext()) graph.RemoveDependency(name, e.Current); //remove the variables

                    throw ex; //throw CircularException ex
                }
            }

            ISet<string> set = new HashSet<string>(getAllDependees(name)); //set of dependents of "name" cell

            return set;
        }

        /// <summary>
        /// Returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell. 
        /// </summary>
        /// 
        /// <exception cref="InvalidNameException"> 
        ///   If the name is null or invalid, throw an InvalidNameException
        /// </exception>
        /// 
        /// <param name="name"></param>
        /// <returns>
        ///   Returns an enumeration, without duplicates, of the names of all cells that contain
        ///   formulas containing name.
        /// 
        ///   <para>For example, suppose that: </para>
        ///   <list type="bullet">
        ///      <item>A1 contains 3</item>
        ///      <item>B1 contains the formula A1 * A1</item>
        ///      <item>C1 contains the formula B1 + A1</item>
        ///      <item>D1 contains the formula B1 - C1</item>
        ///   </list>
        /// 
        ///   <para>The direct dependents of A1 are B1 and C1</para>
        /// 
        /// </returns>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            return graph.GetDependees(name);
        }

        /// <summary>
        /// Get all dependents that have name as a dependee.
        /// All direct and indirect dependents will be included
        /// </summary>
        /// <param name="name">name to get</param>
        /// <returns>set of direct and indirect dependets</returns>
        private IEnumerable<string> getAllDependees(string name)
        {
            HashSet<string> seen = new HashSet<string>(); //visited nodes
            Stack<string> stack = new Stack<string>(); //stack to store dependents
            stack.Push(name);

            while (stack.Count > 0)
            {
                string current = stack.Pop();

                if (!seen.Contains(current))
                {
                    seen.Add(current);

                    foreach (string dependee in graph.GetDependees(current))
                    {
                        stack.Push(dependee);  // Push dependents onto the stack for later exploration
                    }

                    yield return current;  // Yield the current node
                }
            }
        }

        /// <summary>
        /// check content is whether empty or not
        /// </summary>
        /// <param name="cell">cell to check</param>
        /// <returns>false if a cell is empty, true otherwise</returns>
        private bool checkEmptyContent(Cell cell)
        {
            if (cell.getContent() == null) return false;
            else if (cell.getContent() is string) return !string.IsNullOrWhiteSpace((string)cell.getContent()); //if content is string, detect whitespaces

            return true;
        }

        /// <summary>
        /// check a cell named "name" is present in cells HashSet
        /// and if exsists, remove a cell.
        /// 
        /// This method is for update cell.
        /// </summary>
        /// <param name="name">cell's name to find</param>
        private void CheckAndremoveCell(string name)
        {
            foreach (var cell in cells) if (cell.getName().Equals(name)) cells.Remove(cell);
        }
    }
}
