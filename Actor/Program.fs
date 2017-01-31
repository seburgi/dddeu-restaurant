module Program

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

let orderPrinter = fun (o : Order) ->
    printf "%A" o
    ()

// Waiter functions

let placeOrder = fun (nextHandle : Handle) (tableNumber : int) ->
    let order = 
        {
            TableNumber = tableNumber;
            SubTotal = 0.0;
            Tax = 0.0;
            Total = 0.0;
            Ingredients = "";
            OrderId = Guid.NewGuid();
            Items = List.Empty;
            IsPaid = false;
        };
    nextHandle(order)

// Cook functions

let cookFood = fun (nextHandle : Handle) (order : Order) ->
    
    let cookedOrder = { order with
        Ingredients = "Spaghetti"
    
    }

    printf "Cooking...\r\n"
    Thread.Sleep(2000)

    nextHandle(cookedOrder)

// Assistant Manager functions

let priceOrder = fun (nextHandle : Handle) (order : Order) ->
    
    let pricedOrder = { order with
        SubTotal = 12.2
        Total = 12.2 * 0.2
        Tax = 0.2
    }

    printf "Pricing...\r\n"
    Thread.Sleep(2000)

    nextHandle(pricedOrder)
    
// Cashier functions

let payOrder = fun (nextHandle : Handle) (order : Order) ->
    
    let paidOrder = { order with
        IsPaid = true
    }

    printf "Paying...\r\n"

    Thread.Sleep(2000)

    nextHandle(paidOrder)

[<EntryPoint>]
let main argv =
    let cashier = payOrder orderPrinter
    let assistantManager = priceOrder cashier
    let cook = cookFood assistantManager
    let waiter = placeOrder cook
    
    for i in 0 .. 10 do 
        waiter i

    0 // return an integer exit code

    

    
