module Program

open System
open System.Threading

// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

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

//let handle = fun (o : Order) -> ()

let orderPrinter = fun (o : Order) ->
    printf "%A" o
    ()

// Waiter functions

let placeOrder = fun (handle : Handle) (orderInfo : string) ->
    let order = 
        {
            TableNumber = 3;
            SubTotal = 0.0;
            Tax = 0.0;
            Total = 0.0;
            Ingredients = "";
            OrderId = Guid.NewGuid();
            Items = List.Empty;
            IsPaid = false;
        };
    handle(order)

// Cook functions

let cookFood = fun (handle : Handle) (order : Order) ->
    
    let cookedOrder = { order with
        Ingredients = "Spaghetti"
    
    }

    printf "Cooking..."
    Thread.Sleep(2000)

    handle(cookedOrder)

// Assistant Manager functions


let priceOrder = fun (handle : Handle) (order : Order) ->
    
    let pricedOrder = { order with
        SubTotal = 12.2
        Total = 12.2 * 0.2
        Tax = 0.2
    }

    printf "Pricing..."
    Thread.Sleep(2000)

    handle(pricedOrder)
    
// Cashier functions

let payOrder = fun (handle : Handle) (order : Order) ->
    
    let paidOrder = { order with
        IsPaid = true
    }

    printf "Paying..."

    Thread.Sleep(2000)

    handle(paidOrder)

[<EntryPoint>]
let main argv =
    let cashier = payOrder orderPrinter
    let assistantManager = priceOrder cashier
    let cook = cookFood assistantManager
    let waiter = placeOrder cook




    waiter "foo"

    printfn "%A" argv
    0 // return an integer exit code

    

    
