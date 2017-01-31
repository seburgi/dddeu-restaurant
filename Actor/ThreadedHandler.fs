module ThreadedHandler

open Model
open System.Collections.Generic
open System

let subscriptions = new Dictionary<Topic, MessageHandler list>();

let getSubscriptionTopic = fun (msg: Message) ->
    match msg with
    | OrderPlaced _ -> OrderPlacedTopic
    | OrderCooked _ -> OrderCookedTopic
    | OrderPriced _ -> OrderPricedTopic
    | OrderPaid _ -> OrderPaidTopic
    
let publish = fun (msg: Message) ->
    match subscriptions.TryGetValue(getSubscriptionTopic msg) with
        | (true, s) ->
            for handler in s do
                handler msg
        | (false, _) -> ()


type Agent<'T> = MailboxProcessor<'T>

type ThreadedHandler (handler: MessageHandler, name : string) =
    let queue = new Queue<Order>()

    let agent =
        Agent.Start(fun inbox ->
            async {
                while true do
                    let! msg = inbox.Receive()
                    handler msg
                }
            )
    
    member this.Handle (msg : Message) = agent.Post msg
    
    member this.Count = agent.CurrentQueueLength
    member this.Name = name



let subscribe = fun (topic : Topic) (handler : MessageHandler) ->
    subscriptions.[topic] <-
        match subscriptions.TryGetValue(topic) with
        | (true, s) -> s @ [ handler ]
        | (false, _) -> [ handler ]


