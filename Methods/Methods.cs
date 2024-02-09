namespace SpreadSheetMethods
{
    public class Methods
    {
        /// <summary>
        /// Check whether variable name is valid or invalid
        /// </summary>
        /// <param name="name">variable name to check</param>
        /// <returns>true if valid false otherwise</returns>
        public static bool CheckVarName(string name)
        {
            if (name == null)
            {
                return false;
            }
            char[] chars = name.ToCharArray();
            if (!Char.IsLetter(chars[0]) || !chars[0].Equals('_')) return false;

            return true;
        }

    }
}
