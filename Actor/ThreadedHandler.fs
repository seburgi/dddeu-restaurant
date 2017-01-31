module ThreadedHandler

open Model
open System.Collections.Generic
open System

type Agent<'T> = MailboxProcessor<'T>


type ThreadedHandler (nextHandler : Handle) =
    let queue = new Queue<Order>()

    let agent =
        Agent.Start(fun inbox ->
            async {
                while true do
                    let! order = inbox.Receive()
                    nextHandler order
                }
            )
    
    member this.Handle (order : Order) = agent.Post order
    
    member this.Length = agent.CurrentQueueLength

//type WaiterMailbox () =
//
//    static let createOrder tableNumber = 
//        let order = 
//            {
//                TableNumber = tableNumber;
//                SubTotal = 0.0;
//                Tax = 0.0;
//                Total = 0.0;
//                Ingredients = "";
//                OrderId = Guid.NewGuid();
//                Items = List.Empty;
//                IsPaid = false;
//            };
//
//        order
//    
//    static let agent = MailboxProcessor.Start(fun inbox -> 
//
//        // the message processing function
//        let rec messageLoop oldState = async{
//
//            // read a message
//            let! msg = inbox.Receive()
//
//            // do the core logic
//            let newOrder = createOrder msg
//
//            // loop to top
//            return! messageLoop newOrder 
//        }
//
//        // start the loop 
//        messageLoop 0
//        )
//
//    // public interface to hide the implementation
//    static member Add i = agent.Post i