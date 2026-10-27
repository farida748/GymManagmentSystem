using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.BLL.Common
{
    public sealed record Result(bool Success, string? error = null, ResultKind kind = ResultKind.Ok)
    {
        public static Result Ok() => new Result(Success: true);

        public static Result NotFound(string error = "Not found") => new Result( false, error, ResultKind.NotFound);

        public static Result Fail(string error ,ResultKind resultKind= ResultKind.Conflict) => new Result(false, error,resultKind);

        public static Result ValidationError(string error) => new Result(false, error, ResultKind.ValidationError);
    }

    public sealed record Result<T>(bool Success, T? Data = default, string? error = null, ResultKind kind = ResultKind.Ok) 
    {
        public static Result<T> Ok(T data) => new Result<T>(true, data);
        public static Result<T> NotFound(string error = "Not found") => new Result<T>(false, default, error, ResultKind.NotFound);
        public static Result<T> Fail(string error, ResultKind resultKind = ResultKind.Conflict) => new Result<T>(false, default, error, resultKind);
        //public static Result<T> ValidationError(string error) => new Result<T>(false, default, error, ResultKind.ValidationError);
    }
}
