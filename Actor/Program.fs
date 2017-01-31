module Program

open System
open System.Threading
open Model
open System.Collections.Generic
open System.Threading.Tasks
open ThreadedHandler


let orderPrinter = fun (o : Order) ->
//    printf "%A\r\n" o
    printf "Served %d\r\n" o.TableNumber
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
            CreatedOn = DateTime.Now
        };
    nextHandle(order)


let isExpired = fun (order: Order) (ttl : int) ->
    (order.CreatedOn.AddMilliseconds (float ttl)) < DateTime.Now

// Cook functions

let cookFood = fun (name : string) (timeToCook : int) (Message order) ->
    let newOrder =
             { order with
                Ingredients = "Spaghetti"
             }
//    printf "%s is cooking...\r\n" name
    Thread.Sleep(timeToCook)

    publish (OrderCookedMsg (OrderCooked newOrder))

// Assistant Manager functions

let priceOrder = fun ((OrderCooked order) : OrderCooked) ->
    
    let priced = OrderPriced { order with
        SubTotal = 12.2
        Total = 12.2 * 0.2
        Tax = 0.2
    }

//    printf "Pricing...\r\n"
    Thread.Sleep(500)

    publish (OrderPricedMsg priced)
    
// Cashier functions

let payOrder = fun ((OrderPriced order) : OrderPriced) ->
    
    let paid = OrderPaid { order with
        IsPaid = true
    }

//    printf "Paying...\r\n"

    Thread.Sleep(100)

    publish (OrderPaidMsg paid)

// Repeater
let repeater = fun (nextHandles : List<Handle>) (order: Order) ->
    for handle in nextHandles do
        handle order

// Round Robin
let roundRobin = fun (nextHandles : Queue<Handle>) (order: Order) ->

    let cook = nextHandles.Dequeue()

    cook order

    nextHandles.Enqueue cook  

let threadedHandler = fun (orderQueue: Queue<Order>) (order: Order) ->
    orderQueue.Enqueue order

let startThread = fun (handler: Handle) (orderQueue : Queue<Order>) ->
    let task = Task.Factory.StartNew((fun () ->
        match (orderQueue.Count > 0) with
        | true -> handler(orderQueue.Dequeue())
        | false -> Thread.Sleep(1)
    ), TaskCreationOptions.LongRunning)

    ()

//type Agent<'T> = MailboxProcessor<'T>
// 
//let agent (nextHandler : Handle) =
//   Agent.Start(fun inbox ->
//     async { while true do
//               let! order = inbox.Receive()
//               nextHandler order
//            }
//            )

//let agentHandler = fun (agent : Agent<Order>) (order : Order) ->
//    agent.Post order

let rec moreFairHandler = fun (handlers : Queue<ThreadedHandler>) (msg: Message) ->
    let handler = handlers.Dequeue()
    handlers.Enqueue handler

    match handler.Count < 5 with
    | true ->
        handler.Handle msg
    | false ->
        Thread.Sleep(1)
        moreFairHandler handlers msg

let ttlHandler = fun (ttl: int) (msgHandler: MessageHandler) (msg) ->
//    let order = match msg with
//        | OrderPlacedMsg (OrderPlaced order) -> order
//        | OrderCookedMsg (OrderCooked order) -> order
//        | OrderPricedMsg (OrderPriced order) -> order
//        | OrderPaidMsg (OrderPaid order) -> order

//    match isExpired order ttl with
//    | true ->
//        printf "Dropped %d\r\n" order.TableNumber
//        ()
//    | false -> msgHandler msg
    msgHandler msg


let rec monitor = fun (handlers : ThreadedHandler list) ->
    for handler in handlers do
        printf "%s has %d jobs\r\n" handler.Name handler.Count

    Thread.Sleep(1000)
    monitor handlers



[<EntryPoint>]
let main argv =
    let orderQueue = new Queue<Order>()
    let threadedHandlerWithQueue = threadedHandler orderQueue

    let oneSecondTtl = ttlHandler 12000

    let cashier = payOrder |> oneSecondTtl
    let cashierT = new ThreadedHandler(cashier, "Cashier")

    let x = handle |> oneSecondTtl

    let assistantManager = priceOrder  |> oneSecondTtl
    let assistantManagerT = new ThreadedHandler(assistantManager, "Assistant Manager")

    let r = new Random(13)
    
    let hank = cookFood "Hank" (r.Next(0, 4000))  |> oneSecondTtl
    let hankT = new ThreadedHandler(hank, "Hank")

    let tom = cookFood "Tom" (r.Next(0, 4000))  |> oneSecondTtl
    let tomT = new ThreadedHandler(tom, "Tom")

    let suzy = cookFood "Suzy" (r.Next(0, 4000))  |> oneSecondTtl
    let suzyT = new ThreadedHandler(suzy, "Suzy")
    
    let toMonitor = [
        suzyT
        tomT
        hankT
        cashierT
        assistantManagerT
        ]

    Task.Factory.StartNew((fun () -> monitor toMonitor), TaskCreationOptions.LongRunning)
        |> ignore

    let cooksQueue = new Queue<ThreadedHandler>([hankT; tomT; suzyT])
    let cooks = moreFairHandler cooksQueue
    

    let waiter = placeOrder cooks


    subscribe Topic.PaymentReceived cashierT.Handle
    subscribe Topic.OrderPlaced cooks
    subscribe Topic.CookingFinished assistantManagerT.Handle

    
    for i in 0 .. 100 do 
        waiter i

    Console.ReadLine() |> ignore


    0 // return an integer exit code

    

    
