namespace Playground.Core.Utilities
{
    public class BooleanOperations
    {
        public static bool IsNumeric(string inputVal)
        {
            // ReSharper disable once NotAccessedVariable
            double num;
            bool isNum = double.TryParse(inputVal, out num);

            if (isNum)
            {
                return true;
            }

            return false;
        }

        public static string ConvertToTrueFalse(byte bytValue)
        {
            if (bytValue == 0)
            {
                return "False";
            }
            return "True";
        }
    }
}
