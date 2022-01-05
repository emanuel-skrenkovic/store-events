// namespace Store.Checkout.Application;
//
// public class CheckoutProcess : IProcess<CheckoutState>
// {
//     /*
//         OrderPlaced,
//         PaymentVerified,
//         OrderReceivedByWarehouse,
//         Shipped,
//         Received
//     */
//
//     public bool TryNext(CheckoutState currentState, IEvent receivedEvent, out CheckoutState updatedState, out object command)
//     {
//         updatedState = null;
//         command = null;
//         
//         bool updated = receivedEvent switch
//         {
//             OrderPlacedEvent     @event => When(currentState, @event, out updatedState, out command),
//             PaymentVerifiedEvent @event => When(currentState, @event, out updatedState, out command),
//             _ => false
//         };
//
//         return updated;
//     }
//     
//     private bool When(CheckoutState currentState, OrderPlacedEvent _, out CheckoutState updatedState, out object command)
//     {
//         updatedState = currentState with { Status = CheckoutStatus.OrderPlaced };
//         command = null;
//         
//         return true;
//     }
//     
//     private bool When(CheckoutState currentState, PaymentVerifiedEvent @event, out CheckoutState updatedState, out object command)
//     {
//         updatedState = currentState with { Status = CheckoutStatus.PaymentVerified };
//         command = new WarehouseShipOrderCommand(@event.CorrelationId, @event.CausationId);
//
//         return true;
//     }
// }