using SpreadsheetUtilities;
using SS;
using SpreadSheetCell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Spreadsheet
{
    internal class Spreadsheet : AbstractSpreadsheet
    {
        private DependencyGraph graph;
        private HashSet<Cell> cells;

        public override object GetCellContents(string name)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            throw new NotImplementedException();
        }

        public override ISet<string> SetCellContents(string name, double number)
        {
            try
            {
                Cell newCell = new Cell(name, number);
                cells.Add(newCell);
            }
            catch
            {
                throw new InvalidNameException();
            }
            if(graph.HasDependees(name))
            {

            }
        }

        public override ISet<string> SetCellContents(string name, string text)
        {
            throw new NotImplementedException();
        }

        public override ISet<string> SetCellContents(string name, Formula formula)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<string> getAllDependees(string name)
        {

        }
    }
}
