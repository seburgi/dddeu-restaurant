module Program

open System
open System.Threading
open Model
open System.Collections.Generic
open System.Threading.Tasks
open ThreadedHandler


let orderPrinter = fun (o : Order) ->
//    printf "%A\r\n" o
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

let cookFood = fun (nextHandle : Handle) (name : string) (timeToCook : int) (order : Order) ->
    
    let cookedOrder = { order with
        Ingredients = "Spaghetti"
    
    }

//    printf "%s is cooking...\r\n" name
    Thread.Sleep(timeToCook)

    nextHandle(cookedOrder)

// Assistant Manager functions

let priceOrder = fun (nextHandle : Handle) (order : Order) ->
    
    let pricedOrder = { order with
        SubTotal = 12.2
        Total = 12.2 * 0.2
        Tax = 0.2
    }

//    printf "Pricing...\r\n"
    Thread.Sleep(500)

    nextHandle(pricedOrder)
    
// Cashier functions

let payOrder = fun (nextHandle : Handle) (order : Order) ->
    
    let paidOrder = { order with
        IsPaid = true
    }

//    printf "Paying...\r\n"

    Thread.Sleep(100)

    nextHandle(paidOrder)

// Repeater
let repeater = fun (nextHandles : List<Handle>) (order: Order) ->
    for handle in nextHandles do
        handle order

// Round Robin
let roundRobin = fun (nextHandles : Queue<Handle>) (order: Order) ->

    let cook = nextHandles.Dequeue()

    cook order

    nextHandles.Enqueue cook

// Threaded Handler
//let threadedHandler = fun    

let threadedHandler = fun (orderQueue: Queue<Order>) (order: Order) ->
    orderQueue.Enqueue order

let startThread = fun (handler: Handle) (orderQueue : Queue<Order>) ->
    let task = Task.Factory.StartNew((fun () ->
        match (orderQueue.Count > 0) with
        | true -> handler(orderQueue.Dequeue())
        | false -> Thread.Sleep(1)
    ), TaskCreationOptions.LongRunning)

    ()

type Agent<'T> = MailboxProcessor<'T>
 
let agent (nextHandler : Handle) =
   Agent.Start(fun inbox ->
     async { while true do
               let! order = inbox.Receive()
               nextHandler order
            }
            )

let agentHandler = fun (agent : Agent<Order>) (order : Order) ->
    agent.Post order


let rec monitor = fun (agents : (string * Agent<'T>) list) ->
    for (name, agent) in agents do
        printf "%s has %d jobs\r\n" name agent.CurrentQueueLength

    Thread.Sleep(1000)
    monitor agents
    

[<EntryPoint>]
let main argv =
    let orderQueue = new Queue<Order>()
    let threadedHandlerWithQueue = threadedHandler orderQueue

    let cashier = payOrder orderPrinter
    let cashierAgent = agent cashier
    let cashierHandler = agentHandler cashierAgent

    let assistantManager = priceOrder cashierHandler
    let assistantManagerAgent = agent assistantManager
    let assistantManagerHandler = agentHandler assistantManagerAgent
    
    let hank = cookFood assistantManagerHandler "Hank" 500
    let hankAgent = agent hank
    let hankHandler = agentHandler hankAgent

    let tom = cookFood assistantManagerHandler "Tom" 5000
    let tomAgent = agent tom
    let tomHandler = agentHandler tomAgent

    let suzy = cookFood assistantManagerHandler "Suzy" 2000
    let suzyAgent = agent suzy
    let suzyHandler = agentHandler suzyAgent

    
    let toMonitor = [
        ("Suzy", suzyAgent)
        ("Tom", tomAgent)
        ("Hank", hankAgent)
        ("Assistant Manager", assistantManagerAgent)
        ("Cashier", cashierAgent)
        ]

    Task.Factory.StartNew((fun () -> monitor toMonitor), TaskCreationOptions.LongRunning)



    let cooks = roundRobin (new Queue<Handle>([hankHandler; tomHandler; suzyHandler]))
    let waiter = placeOrder cooks
    
    for i in 0 .. 100 do 
        waiter i
//
//    suzyAgent.

    Console.ReadLine() |> ignore


    0 // return an integer exit code

    

    
