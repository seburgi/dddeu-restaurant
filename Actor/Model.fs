module Model

open System
open System.Threading


type Item = {
    Name : string
    Quantity : int
    Price : double
}

type Order = {
    TableNumber : int
    SubTotal : double
    Tax : double
    Total : double
    Ingredients: string
    OrderId : Guid
    Items : List<Item>
    IsPaid : bool
    CreatedOn : DateTime

    Id : Guid
    CorrId : Guid
    CauseId : Guid
}

type Handle = Order -> unit

type Topic =
    | OrderPlacedTopic
    | OrderCookedTopic
    | OrderPricedTopic
    | OrderPaidTopic

type Message =
    | OrderPlaced of OrderPlaced
    | OrderCooked of OrderCooked
    | OrderPriced of OrderPriced
    | OrderPaid of OrderPaid
and OrderPlaced =  Order
and OrderCooked =  Order
and OrderPriced =  Order
and OrderPaid =  Order

type MessageHandler = Message -> unit
