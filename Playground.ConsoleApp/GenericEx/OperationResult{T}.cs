namespace Playground.ConsoleApp.GenericEx
{
    public class OperationResult<T>
    {
        public T Result { get; set; }
        public string Message { get; set; }

        public OperationResult()
        {
        }

        public OperationResult(T result, string message) : this()
        {
            Result = result;
            Message = message;
        }

        public T RetrieveValue(string sql, T defaultVal)
        {
            T value = defaultVal;

            return value;
        }
    }
}

/***
 * Usage
 *  var operationResult = new OperationResult<bool>(success, msgText);
 *  var operationResult = new OperationResult<decimal>(value, msgText);
 *
 *  var valueInt = OperationResult.RetrieveValue<int>("", 42);
 *  var valueString = OperationResult.RetrieveValue<string>("", "something");
 * **/
