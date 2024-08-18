using OneOf;

namespace IF.Domain
{
    public class Result<TSuccess, TError>
    {
        private readonly OneOf<TSuccess, TError> _value;

        private Result(OneOf<TSuccess, TError> value)
        {
            _value = value;
        }

        public static Result<TSuccess, TError> Success(TSuccess value) => new Result<TSuccess, TError>(value);
        public static Result<TSuccess, TError> Failure(TError error) => new Result<TSuccess, TError>(error);

        public void Switch(Action<TSuccess> successAction, Action<TError> errorAction) => _value.Switch(successAction, errorAction);
        public TResult Match<TResult>(Func<TSuccess, TResult> successFunc, Func<TError, TResult> errorFunc) => _value.Match(successFunc, errorFunc);
    }
}
