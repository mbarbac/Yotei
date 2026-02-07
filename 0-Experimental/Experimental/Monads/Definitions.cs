/*
MAP: Transform the value...
-------------------------------------------------

public static Result<TOut> Map<TIn, TOut>(
    this Result<TIn> result, Func<TIn, TOut> mapper)
{
    return result.IsSuccess
        ? Result<TOut>.Success(mapper(result.Value))
        : Result<TOut>.Failure(result.Error);
}
 
BIND: Chain operation...
-------------------------------------------------

public static Result<TOut> Bind<TIn, TOut>(
    this Result<TIn> result, Func<TIn, Result<TOut>> binder)
{
    return result.IsSuccess
        ? binder(result.Value)
        : Result<TOut>.Failure(result.Error);
}

MATCH: Handle both cases...
-------------------------------------------------

public static TOut Match<TIn, TOut>(
    this Result<TIn> result, Func<TIn, TOut> onSuccess, Func<Error, TOut> onFailure)
{
    return result.IsSuccess
        ? onSuccess(result.Value)
        : onFailure(result.Error);
}

USAGE...
-------------------------------------------------
return ValidateOrder(request)
    .Bind(order => CheckInventory(order))
    .Bind(order => ProcessPayment(order))
    .Bind(order => CreateShipment(order))
    .Map(shipment => new OrderConfirmation(shipment.TrackingNumber));
 */