[<AutoOpen>]

type Warenkorb = Warenkorb of int

type Command =
    | Lege_Warenkorb_an of Lege_Warenkorb_an
    | Bestelle_Warenkorb of Bestelle_Warenkorb 
    | Lege_Produkt_In_Warenkorb of Lege_Produkt_In_Warenkorb

and Lege_Warenkorb_an = {
    Warenkorb: Warenkorb
    Kunde: string }

and Bestelle_Warenkorb = {
    Warenkorb: Warenkorb }

and Lege_Produkt_In_Warenkorb = {
    Warenkorb: Warenkorb 
    Produkt: string}
