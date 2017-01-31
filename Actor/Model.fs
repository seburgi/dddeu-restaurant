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
}

type Handle = Order -> unit

type Topic =
    | OrderPlacedTopic
    | OrderCookedTopic
    | OrderPricedTopic
    | OrderPaidTopic

//type OrderPlaced = OrderPlaced of Order
//type OrderCooked = OrderCooked of Order
//type OrderPriced = OrderPriced of Order
//type OrderPaid = OrderPaid of Order


type Message =
    | OrderPlacedMsg of Order
    | OrderCookedMsg of Order
    | OrderPricedMsg of Order
    | OrderPaidMsg of Order



type MessageHandler = Message -> unit
