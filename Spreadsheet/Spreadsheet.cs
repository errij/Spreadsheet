using SpreadsheetUtilities;
using SpreadSheetCell;
using System.Transactions;
using SpreadSheetMethods;

namespace SS
{
    public class Spreadsheet : AbstractSpreadsheet
    {
        private DependencyGraph graph;
        private HashSet<Cell> cells;

        public Spreadsheet()
        {
            this.cells = new HashSet<Cell>();
            this.graph = new DependencyGraph();
        }

        public override object GetCellContents(string name)
        {
            if (!Methods.CheckVarName(name)) throw new InvalidNameException();

            foreach (var cell in cells)
            {
                if (cell.getName().Equals(name)) return cell.getContent();
            }

            return null;
        }

        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            foreach (var cell in cells)
            {
                if (checkEmptyContent(cell)) yield return cell.getName();
            }
        }

        public override ISet<string> SetCellContents(string name, double number)
        {
            CheckAndremoveCell(name);

            try
            {
                cells.Add(new Cell(name, number));
            }
            catch
            {
                throw new InvalidNameException();
            }

            ISet<string> set = new HashSet<string>(getAllDependees(name));
            return set;
        }

        public override ISet<string> SetCellContents(string name, string text)
        {
            if (text == null) throw new ArgumentNullException("text is null!");
            CheckAndremoveCell(name);
            try
            {
                cells.Add(new Cell(name, text));
            }
            catch
            {
                throw new InvalidNameException();
            }

            ISet<string> set = new HashSet<string>(getAllDependees(name));
            return set;
        }

        public override ISet<string> SetCellContents(string name, Formula formula)
        {
            object temp = GetCellContents(name);
            try
            {
                Cell cell = new Cell(name, formula);

                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        CheckAndremoveCell(name);
                        cells.Add(cell);
                        IEnumerator<string> e = formula.GetVariables().GetEnumerator();
                        while (e.MoveNext()) graph.AddDependency(name, e.Current);

                        GetCellsToRecalculate(name);

                        scope.Complete();
                    }
                    catch (CircularException ex)
                    {
                        scope.Dispose();

                        cells.Remove(cell);

                        if (temp != null) cells.Add(new Cell(name, temp));

                        IEnumerator<string> e = formula.GetVariables().GetEnumerator();
                        while (e.MoveNext()) graph.RemoveDependency(name, e.Current);

                        throw ex;
                    }
                }

                ISet<string> set = new HashSet<string>(getAllDependees(name));

                return set;
            }
            catch (ArgumentException ex)
            {
                throw new InvalidNameException();
            }
        }

        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            return graph.GetDependees(name);
        }

        private IEnumerable<string> getAllDependees(string name)
        {
            HashSet<string> seen = new HashSet<string>();
            Stack<string> stack = new Stack<string>();
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

        private bool checkEmptyContent(Cell cell)
        {
            if (cell.getContent() == null) return false;
            else if (cell.getContent() is string) return !string.IsNullOrWhiteSpace((string)cell.getContent());

            return true;
        }

        private void CheckAndremoveCell(string name)
        {
            foreach (var cell in cells)
            {
                if (cell.getName().Equals(name)) cells.Remove(cell);

            }
        }
    }
}
