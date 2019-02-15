using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicGateIDE
{
    class Result<T>
    {
        public T Value { get; private set; } = default(T);
        public bool IsSuccess { get; private set; } = false;
        public string Message { get; private set; } = "";

        public Result(T value)
        {
            Value = value;
            IsSuccess = true;
        }

        public Result(string errorMessage)
        {
            Message = errorMessage;
        }

        public static Result<T> Success(T value) { return new Result<T>(value); }
        public static Result<T> Failure(string errorMessage) { return new Result<T>(errorMessage); }

        public static implicit operator bool(Result<T> result) { return result.IsSuccess; }
    }
    
}
