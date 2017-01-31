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
}

type Handle = Order -> unit
