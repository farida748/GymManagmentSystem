using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.BLL.Common
{
    public sealed record Result(bool Success, string? Error = null, ResultKind kind = ResultKind.Ok)
    {
        public static Result Ok() => new Result(Success: true);

        public static Result NotFound(string Error = "Not found") => new Result( false, Error, ResultKind.NotFound);

        public static Result Fail(string Error ,ResultKind resultKind= ResultKind.Conflict) => new Result(false, Error,resultKind);

        public static Result ValidationError(string Error) => new Result(false, Error, ResultKind.ValidationError);
    }

    public sealed record Result<T>(bool Success, T? Data = default, string? Error = null, ResultKind kind = ResultKind.Ok) 
    {
        public static Result<T> Ok(T data) => new Result<T>(true, data);
        public static Result<T> NotFound(string Error = "Not found") => new Result<T>(false, default, Error, ResultKind.NotFound);
        public static Result<T> Fail(string Error, ResultKind resultKind = ResultKind.Conflict) => new Result<T>(false, default, Error, resultKind);
        //public static Result<T> ValidationError(string Error) => new Result<T>(false, default, Error, ResultKind.ValidationError);
    }
}
