using SpreadSheetCell;
using SpreadSheetMethods;
using SpreadsheetUtilities;
using SS;
using System.Collections.Generic;
using System.Linq;

namespace SpreadsheetTests
{
    [TestClass]
    public class SpreadsheetTests
    {
        /// <summary>
        /// This is for private method getAllDependees.
        /// Make sure uncomment the test and the method before testing.
        /// </summary>
/*        [TestMethod]
        [TestCategory("Temporary private method test")]
        public void getAllDependeesTest()
        {
            DependencyGraph graph = new DependencyGraph();
            graph.AddDependency("a", "b");
            graph.AddDependency("b", "c");
            graph.AddDependency("c", "d");
            graph.AddDependency("d", "e"); // a -> b -> c -> d-> e

            IEnumerator<string> e = getAllDependees("e", graph).GetEnumerator();
            e.MoveNext();
            Assert.AreEqual("e", e.Current);
            e.MoveNext();
            Assert.AreEqual("d", e.Current);
            e.MoveNext();
            Assert.AreEqual("c", e.Current);
            e.MoveNext();
            Assert.AreEqual("b", e.Current);
            e.MoveNext();
            Assert.AreEqual("a", e.Current);

            DependencyGraph g2 = new DependencyGraph();
            graph.AddDependency("a", "b"); //a -> b
            IEnumerator<string> e2 = getAllDependees("a", g2).GetEnumerator();
            e2.MoveNext();
            Assert.AreEqual("a", e2.Current);
            Assert.IsFalse(e2.MoveNext());
        }
        private IEnumerable<string> getAllDependees(string name, DependencyGraph graph)
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
                    yield return current;  // Yield the current node

                    foreach (string dependee in graph.GetDependees(current))
                    {
                        stack.Push(dependee);  // Push dependents onto the stack for later exploration
                    }
                }
            }
        }*/

        [TestMethod]
        [TestCategory("Method test")]
        public void nameValidityTest()
        {
            Assert.IsTrue(Extension.CheckVarName("A1"));
            Assert.IsFalse(Extension.CheckVarName("1A"));
            Assert.IsFalse(Extension.CheckVarName("11"));
            Assert.IsTrue(Extension.CheckVarName("___!"));
            Assert.IsFalse(Extension.CheckVarName(""));
            Assert.IsFalse(Extension.CheckVarName(null));
            Assert.IsFalse(Extension.CheckVarName("     "));
            Assert.IsTrue(Extension.CheckVarName("a1"));
            Assert.IsTrue(Extension.CheckVarName("aaaaaaaaaaaaaa2352343256@##!!__1"));
            Assert.IsFalse(Extension.CheckVarName("&11"));
        }

        [TestMethod]
        [TestCategory("Cell test")]
        public void CellTest()
        {
            Cell cell = new Cell("A1", new Formula("x + 2"));
            Assert.AreEqual(new Formula("x+2"), cell.getContent());
            Assert.AreEqual("A1", cell.getName());
            Cell cell2 = new Cell("B1", 1.0);
            Assert.AreEqual(1.0, cell2.getContent());
            Assert.AreEqual("B1", cell2.getName());
            Cell cell3 = new Cell("C1", "FIVE GUYS BEST");
            Assert.AreEqual("FIVE GUYS BEST", cell3.getContent());
            Assert.AreEqual("C1", cell3.getName());
            Cell cell4 = new Cell("a", "");
            Cell cell5 = new Cell("A", "");
            Assert.AreNotEqual(cell4.getName(), cell5.getName());
        }

        [TestMethod]
        [TestCategory("Cell test")]
        [ExpectedException(typeof(ArgumentException))]
        public void invalidCellName()
        {
            Cell cell = new Cell("1a", new Formula("x + 2"));
        }

        [TestMethod]
        [TestCategory("Cell test")]
        public void valueTest()
        {
            Cell cell = new Cell("A1", new Formula("x + 2"));
            Formula f = (Formula) cell.getContent();
            cell.setValue(f.Evaluate(s => 1));
            Assert.AreEqual(3.0, cell.getValue());  
        }

        [TestMethod]
        [TestCategory("Cell test")]
        [ExpectedException(typeof(ArgumentException))]
        public void invalidCellContent()
        {
            Cell cell = new Cell("a", new HashSet<Formula> { new Formula("x + 2") });
        }
        [TestMethod]
        [TestCategory("Cell test")]
        public void cellHashCodeTest()
        {
            HashSet<Cell> set = new HashSet<Cell>{ new Cell("A1", 1.0), new Cell("B1", 2.0)};
            set.Remove(new Cell("A1", 1.0));
            List<string> check = new List<string>();    
            foreach(Cell cell in set)
            {
                check.Add(cell.getName());
            }
            Assert.IsFalse(check.Contains("A1"));
        }
        [TestMethod]
        [TestCategory("Spreadsheet Test")]
        public void spreadsheetSetTest()
        {
            AbstractSpreadsheet sp = new Spreadsheet();
            sp.SetCellContents("A1", 12);
            sp.SetCellContents("B1", "test");
            sp.SetCellContents("C1", new Formula("3 + 4"));
        }
        [TestMethod]
        [TestCategory("Spreadsheet Test")]
        [ExpectedException(typeof(InvalidNameException))]
        public void spreadsheetInvalidnameSetTest1()
        {
            AbstractSpreadsheet sp = new Spreadsheet();
            sp.SetCellContents("1", 12);
        }

        [TestMethod]
        [TestCategory("Spreadsheet Test")]
        [ExpectedException(typeof(InvalidNameException))]
        public void spreadsheetInvalidnameSetTest2()
        {
            AbstractSpreadsheet sp = new Spreadsheet();
            sp.SetCellContents("", "test");
        }

        [TestMethod]
        [TestCategory("Spreadsheet Test")]
        [ExpectedException(typeof(InvalidNameException))]
        public void spreadsheetInvalidnameSetTest3()
        {
            AbstractSpreadsheet sp = new Spreadsheet();
            sp.SetCellContents("1111abs", new Formula("3 + 4"));
        }

        [TestMethod]
        [TestCategory("Spreadsheet Test")]
        public void getContentsTest()
        {
            AbstractSpreadsheet sp = new Spreadsheet();
            sp.SetCellContents("A1", 12);
            sp.SetCellContents("B1", "test");
            sp.SetCellContents("C1", new Formula("3 + 4"));
            Assert.AreEqual(12.0, sp.GetCellContents("A1"));
            Assert.AreNotEqual(12.0, sp.GetCellContents("a1"));
            Assert.AreEqual("test", sp.GetCellContents("B1"));
            Assert.AreEqual(new Formula("3 + 4"), sp.GetCellContents("C1"));
            sp.SetCellContents("D1", " ");
            sp.SetCellContents("D2", "");
            IEnumerator<object> e = sp.GetNamesOfAllNonemptyCells().GetEnumerator();
            e.MoveNext();
            Assert.AreEqual("A1", e.Current);
            e.MoveNext();
            Assert.AreEqual("B1", e.Current);
            e.MoveNext();
            Assert.AreEqual("C1", e.Current);
            Assert.IsFalse(e.MoveNext());
        }
        [TestMethod]
        [TestCategory("Spreadsheet Test")]
        public void emptynameGetTest()
        {
            Spreadsheet sp = new Spreadsheet();
            sp.GetCellContents("A1");
            sp.GetCellContents("____&&&@ogoisehgrhsehg");
        }

        [TestMethod]
        [TestCategory("Spreadsheet Test")]
        [ExpectedException(typeof(InvalidNameException))]
        public void invalidnameGetTest()
        {
            Spreadsheet sp = new Spreadsheet();
            sp.GetCellContents("1A");
        }

        [TestMethod]
        [TestCategory("Spreadsheet Test")]
        [ExpectedException(typeof(CircularException))]
        public void circularGetTest()
        {
            Spreadsheet sp = new Spreadsheet();
            sp.SetCellContents("A1", new Formula("B1 + 1"));
            sp.SetCellContents("B1", new Formula("A1 + 1"));
        }

        [TestMethod]
        [TestCategory("Spreadsheet Test")]
        public void circularRollbackTest()
        {
            Spreadsheet sp = new Spreadsheet();

            try
            {
                sp.SetCellContents("A1", new Formula("B1 + 1"));
                sp.SetCellContents("B1", new Formula("A1 + 1"));
            }
            catch (CircularException)
            {
            }
            Assert.IsNotNull(sp.GetCellContents("A1"));
            Assert.IsNull(sp.GetCellContents("B1"));
            string st = string.Join("", sp.SetCellContents("A1", new Formula("B1 + 1")));
            Assert.AreEqual("A1", st);
        }

        [TestMethod]
        [TestCategory("SpreadsheetTest")]
        [ExpectedException(typeof(CircularException))]
        public void FormulaCircularReferenceTest()
        {
            Spreadsheet sp = new Spreadsheet();
            sp.SetCellContents("A1", new Formula("A1 + 1"));
            sp.SetCellContents("A1", new Formula("A1 + 1"));
        }

        [TestMethod]
        [TestCategory("SpreadsheetTest")]
        public void dependencyTest()
        {
            Spreadsheet sp = new Spreadsheet();
            sp.SetCellContents("B1", new Formula("A1 + 1"));
            sp.SetCellContents("C1", new Formula("B1 + 1"));
            sp.SetCellContents("D1", new Formula("C1 + 1"));
            sp.SetCellContents("E1", new Formula("D1 + 1"));
            sp.SetCellContents("F1", new Formula("E1 + 1"));
            string st = string.Join("", sp.SetCellContents("A1", 1.0));
            Assert.AreEqual("A1B1C1D1E1F1", st);
            sp.SetCellContents("B1", new Formula("A1 + 1"));
            sp.SetCellContents("C1", new Formula("A1 + 1"));
            sp.SetCellContents("D1", new Formula("A1 + 1"));
            sp.SetCellContents("E1", new Formula("A1 + 1"));
            sp.SetCellContents("F1", new Formula("A1 + 1"));
            Assert.AreEqual(new Formula("A1 + 1"), sp.GetCellContents("E1"));
            ISet<string> set = sp.SetCellContents("A1", 1.0);
            Assert.IsTrue(set.Contains("A1"));
            Assert.IsTrue(set.Contains("B1"));
            Assert.IsTrue(set.Contains("C1"));
            Assert.IsTrue(set.Contains("D1"));
            Assert.IsTrue(set.Contains("F1"));
            Assert.IsTrue(set.Contains("E1"));
            try
            {
                sp.SetCellContents("E1", new Formula("E1"));
            }
            catch { }

            Assert.AreEqual(new Formula("A1 + 1"), sp.GetCellContents("E1"));
        }
    }

}