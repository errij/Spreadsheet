using SpreadsheetUtilities;

namespace FormulaTests
{
    [TestClass]
    public class FormulaTests
    {
        private static double look(string value)
        {
            if(value == "X")
            {
                return 4;
            }
            else if(value == "Y") 
            {
                return 5;
            }
            else if (value == "x") 
            {
                return -4;
            }
            else if (value == "y")
            {
                return -5;
            }
            else
            {
                throw new ArgumentException("nope");
            }
        }
        private static string normal(string value) 
        {
            if(value.Equals("TEST")) 
            {
                throw new ArgumentException("For test");
            }
            return value.ToUpper();
        }

        private static bool valid(string value)
        {
            if (value.Equals("TEST"))
            {
                return false;
            }
            return true;
        }

        [TestMethod]
        public void simpleConstructor()
        {
            Formula simple = new Formula("1+2");
            Assert.AreEqual(3.0, simple.Evaluate(look) );
            Formula simple2 = new Formula("x+2");
            Assert.AreEqual(-2.0, simple2.Evaluate(look));
            Formula simple3 = new Formula("X+2");
            Assert.AreEqual(6.0, simple3.Evaluate(look));
            Assert.AreNotEqual(simple3.Evaluate(look), simple2.Evaluate(look));
            Formula simple4 = new Formula("Y + y");
            Assert.AreEqual(0.0, simple4.Evaluate(look));  
        }

        [TestMethod]
        public void NormalizedConstructor() 
        {
            Formula f = new Formula("x+2", normal, s => true);
            Formula f2 = new Formula("X+2", normal, s => true);
            Assert.IsTrue(f.Equals(f2));
            Assert.AreEqual(6.0, f.Evaluate(look));
            Formula f3 = new Formula("Y + y", normal, s => true);
            Assert.AreEqual(10.0, f3.Evaluate(look));
        }

        [TestMethod] public void NormalizedConstructor2()
        {
            Formula f = new Formula("x", normal, s => true);
            Formula f2 = new Formula("    ", normal, s => true);
            Assert.AreEqual("X", f.ToString());
            Assert.AreEqual("", f2.ToString());
        }

        [TestMethod]
        public void notationTest()
        {
            Formula f = new Formula("5e2 + 200");
            Assert.AreEqual(700.0, f.Evaluate(look));
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void InvalidVar() 
        {
            Formula f = new Formula("TEST+2", s => s, valid);
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void InvalidVar2()
        {
            Formula f = new Formula("1A + 2");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void NormalErrorTest()
        {
            Formula f = new Formula("TEST+2", normal, s => true);
        }

        [TestMethod]
        public void weirdValues()
        {
            Formula f = new Formula("xxxxxx_nnnnaaaa+2", normal, s => true);
            Assert.AreEqual(3.0, f.Evaluate(s => 1));
        }

        [TestMethod]
        public void lookupFail()
        {
            Formula f = new Formula("xxxxxx_nnnnaaaa+2", normal, s => true);
            Assert.IsTrue(f.Evaluate(look) is FormulaError);
        }

        [TestMethod]
        public void complicatedMath()
        {
            Formula f = new Formula("y1*3-8/2+4*(8-9*2)/14*x7"); //8 - (20/7) =/= 5.14285714286 --by google calc
            Assert.AreEqual(5.142857142857142, f.Evaluate(s => (s == "x7") ? 1 : 4));
            Formula f2 = new Formula("x1+(x2+(x3+(x4+(x5+x6))))");
            Assert.AreEqual(6.0, f2.Evaluate(s => 1));
            Formula f3 = new Formula("3.424435 + 44.342534 * 23.324211 + (6.234532*23423.24)"); //147070.618378 -- by google calc
            Assert.AreEqual(147070.61837797068, f3.Evaluate(s => 1));
        }

        [TestMethod]
        public void dividedZero()
        {
            Formula f = new Formula("x/0", normal, s => true);
            Assert.IsTrue(f.Evaluate(s => 1) is FormulaError);
        }

        [TestMethod]
        public void invalidFunction()
        {
            Formula f = new Formula("x++", normal, s => true);
            Assert.IsTrue(f.Evaluate(s => 1) is FormulaError);
            Formula f2 = new Formula("2 3 4", normal, s => true);
            Assert.IsTrue(f2.Evaluate(s => 1) is FormulaError);
            Formula f3 = new Formula("(()))", normal, s => true);
            Assert.IsTrue(f3.Evaluate(s => 1) is FormulaError);
            Formula f4 = new Formula("(1/0)", normal, s => true);
            Assert.IsTrue(f4.Evaluate(s => 1) is FormulaError);
            Formula f5 = new Formula("4*(1/0)", normal, s => true);
            Assert.IsTrue(f5.Evaluate(s => 1) is FormulaError);
            Formula f6 = new Formula("4/(0)", normal, s => true);
            Assert.IsTrue(f6.Evaluate(s => 1) is FormulaError);
            Formula f7 = new Formula("3+4+11+345435+4.01232+12*", normal, s => true);
            Assert.IsTrue(f7.Evaluate(s => 1) is FormulaError);
            Formula f8 = new Formula(" ", normal, s => true);
            Assert.IsTrue(f8.Evaluate(s => 1) is FormulaError);
            Formula f9 = new Formula("      ", normal, s => true);
            Assert.IsTrue(f9.Evaluate(s => 1) is FormulaError);
        }

        [TestMethod]
        public void getVarTest()
        {
            Formula f = new Formula("x++"); //should return x
            IEnumerator<string> e = f.GetVariables().GetEnumerator();
            e.MoveNext();
            Assert.AreEqual("x", e.Current);
            Assert.IsFalse(e.MoveNext());
            Formula f1 = new Formula("x++", normal, s => true); //should return X
            IEnumerator<string> e1 = f1.GetVariables().GetEnumerator();
            e1.MoveNext();
            Assert.AreEqual("X", e1.Current);
            Formula f2 = new Formula("x+y+z+d");
            IEnumerator<string> e2 = f2.GetVariables().GetEnumerator();
            e2.MoveNext();
            Assert.AreEqual("x", e2.Current);
            e2.MoveNext();
            Assert.AreEqual("y", e2.Current);
            e2.MoveNext();
            Assert.AreEqual("z", e2.Current);
            e2.MoveNext();
            Assert.AreEqual("d", e2.Current);
            Assert.IsFalse(e2.MoveNext());
            Formula f2a = new Formula("x+y+z+d", normal, s => true);
            IEnumerator<string> e2a = f2a.GetVariables().GetEnumerator();
            e2a.MoveNext();
            Assert.AreEqual("X", e2a.Current);
            e2a.MoveNext();
            Assert.AreEqual("Y", e2a.Current);
            e2a.MoveNext();
            Assert.AreEqual("Z", e2a.Current);
            e2a.MoveNext();
            Assert.AreEqual("D", e2a.Current);
            Assert.IsFalse(e2a.MoveNext());
            Formula f3 = new Formula("x+x+x+x");
            IEnumerator<string> e3 = f3.GetVariables().GetEnumerator();
            e3.MoveNext();
            Assert.AreEqual("x", e3.Current);
            e3.MoveNext();
            Assert.AreEqual("x", e3.Current);
            e3.MoveNext();
            Assert.AreEqual("x", e3.Current);
            e3.MoveNext();
            Assert.AreEqual("x", e3.Current);
            Assert.IsFalse(e3.MoveNext());
            Formula f4 = new Formula("4e1 + x + y");
            IEnumerator<string> e4 = f4.GetVariables().GetEnumerator();
            e4.MoveNext();
            Assert.AreNotEqual("e", e4.Current);
            Assert.AreEqual("x", e4.Current);
            e4.MoveNext();
            Assert.AreEqual("y", e4.Current);
        }

        [TestMethod]
        public void equalTest()
        {
            Formula f = new Formula("x + 2");
            Formula f2 = new Formula("x + 2", normal, s => true);
            Assert.IsFalse (f2.Equals(f));
            Formula f3 = new Formula("x + 2", normal, s => true);
            Assert.IsTrue(f3.Equals(f2));
            Assert.IsTrue(f2 == f3);
            Assert.IsTrue(f2.GetHashCode() == f3.GetHashCode());
            Assert.IsFalse(f == f2);
            Assert.IsFalse(f.GetHashCode() == f2.GetHashCode());
            Assert.IsFalse(f2 != f3);
            Assert.IsTrue(f != f2);
            Formula f4 = new Formula("x1+y2", normal, s=> true);
            Formula f5 = new Formula("X1 + Y2");
            Assert.IsTrue (f4.Equals(f5));
            Formula f6 = new Formula("5e2 + 1e1", normal, s => true);
            Formula f7 = new Formula("500 + 10");
            Assert.IsTrue(f6.Equals(f7));
            Formula f8 = new Formula("5.0000");
            Formula f9 = new Formula("5");
            Assert.IsTrue (f8.Equals(f9));
        }

        [TestMethod]
        public void otherInvalid()
        {
            Formula f = new Formula("2 / 0");
            Assert.IsTrue(f.Evaluate(look) is FormulaError);
            Formula f2 = new Formula("! + 3");
            Assert.IsTrue(f2.Evaluate(look) is FormulaError);
        }

        [TestMethod]
        public void toStringTest()
        {
            Formula f = new Formula("x + Y");
            Formula f2 = new Formula("x + Y", normal, s => true);
            Assert.IsFalse(f.ToString().Equals(f2.ToString()));
            Assert.AreEqual("x+Y", f.ToString());   
            Assert.AreEqual("X+Y", f2.ToString());
        }
    }
}