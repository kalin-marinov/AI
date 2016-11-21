type Frog = LEFT | RIGHT | EMPTY

/// <summary> Prepares the initial state </summary>
let initArray = fun frogs -> 
 Array.init (2 * frogs + 1) 
    (fun pos -> match pos with
                 | _ when pos < frogs -> RIGHT
                 | _ when pos > frogs -> LEFT
                 | frogs -> EMPTY);

type GameField(field : Frog[], previousState : GameField option) =

   let len = field.Length;
   let mutable emptySpot = field |> Array.findIndex(fun pos -> pos = EMPTY);

   // other ctor:
   new(length:int) = GameField(initArray(length), None);

   // Public members:
   member this.PreviousState with get () = previousState;

   member this.Print() =
      field |> Array.iter (fun x -> match x with
                                           | RIGHT -> printf ">"
                                           | LEFT -> printf "<"
                                           | EMPTY -> printf "_");
      printfn ""; // Finish the line                                       
                                                  
   member this.MoveEmpty(newPos: int) =
      field.[emptySpot] <- field.[newPos];
      field.[newPos] <- EMPTY;
      emptySpot <- newPos;


   member this.GenerateAndMove(newPos:int) =
      let newState = new GameField(Array.copy(field), Some(this));
      newState.MoveEmpty(newPos);
      newState;


   member this.GetPossibleMoves() =
     seq{
            // >_
            if (emptySpot > 0 && field.[emptySpot - 1] = RIGHT)
                then yield this.GenerateAndMove(emptySpot - 1);

            // ><_
            if emptySpot > 1 && (field.[emptySpot-2], field.[emptySpot-1]) = (RIGHT, LEFT)
                then yield this.GenerateAndMove(emptySpot-2);

            // _>
            if (emptySpot < len - 1 && field.[emptySpot + 1] = LEFT)
                then yield this.GenerateAndMove(emptySpot + 1);

            // _><
            if emptySpot < len - 2 && (field.[emptySpot+1], field.[emptySpot+2]) = (RIGHT, LEFT)
                then yield this.GenerateAndMove(emptySpot+2);   
      }     

   member this.IsDestination() = 
          field.[emptySpot] = EMPTY && 
          field.[0 .. emptySpot-1] |> Array.forall(fun spot -> spot = LEFT) &&
          field.[emptySpot+1 .. len-1] |> Array.forall(fun spot -> spot = RIGHT);
     

let rec DfsSearch (field : GameField) : GameField option =
  if field.IsDestination() then Some(field);

  else match field.GetPossibleMoves()
           |> Seq.map DfsSearch
           |> Seq.tryFind (fun dfsResult -> dfsResult <> None) 
            with
              | Some(result) -> result
              | None -> None
    

let rec PrintPath(field :GameField) =
  if(field.PreviousState <> None) 
    then PrintPath(field.PreviousState.Value); 

  field.Print();


open System;

[<EntryPoint>]
let main argv = 
    let size = Int32.Parse(Console.ReadLine());

    let field = new GameField(size);
    let result = DfsSearch(field);

    PrintPath(result.Value);

    0 // return SUCCESS


