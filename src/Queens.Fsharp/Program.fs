open System;

/// <summary> Represents a chess field with a single queen placed on each column </summary>
type QueensGame(queensCount: int) =
   let queenPositions = Array.create queensCount 0;
   let rng = new Random();
   do
       for col = 0 to queensCount - 1 do         
           queenPositions.[col] <- rng.Next(0, queensCount);

   
   /// <summary> Sets the queen on the given col to be on the given row (limited to one queen per col) </summary>
   member this.SetQueen(row,col) =  queenPositions.[col] <- row;

   /// <summary> Determines whether there is a queen on the given position </summary>
   member this.IsQueen(row, col) = queenPositions.[col] = row;


   /// <summary> Counts the amount of queens on one of the diagonals intercepting the given position (row, col) </summary>
   member this.SearchDiagonal(row, col, increaseRows, increaseCols) =
        let maxPosition = queensCount - 1;

        let distanceToEdge(position, isIncreasing) =
            match isIncreasing with
            | true -> maxPosition - position 
            | false -> position - 0;

        let range(start, count, isIncreasing) =
            match isIncreasing with
            | true -> seq {start .. start + count} 
            | false -> seq {start .. -1 .. start - count}

        let distance = Math.Min(distanceToEdge(row, increaseRows), distanceToEdge(col, increaseCols)); 

        // Result:
        Seq.zip (range(row, distance, increaseRows)) (range(col, distance, increaseCols))
            |> Seq.filter (fun (r, c) -> this.IsQueen(r, c) && not(r = row && c = col))
            |> Seq.length;  
            

    /// <summary> Get the number of queens conflicting the given position  </summary>
   member this.GetConflicts(row, col) =
            // Counts the queens on the same row (different from the current one)
            let queensOnTheRow = seq [0 .. queensCount-1] 
                                 |> Seq.filter (fun c -> c <> col && this.IsQueen(row, c))
                                 |> Seq.length;
                                
            let queensOnTheDiagonal =  this.SearchDiagonal(row, col, true, true)
                                     + this.SearchDiagonal(row, col, true, false)
                                     + this.SearchDiagonal(row, col, false, true)
                                     + this.SearchDiagonal(row, col, false, false);

            queensOnTheRow + queensOnTheDiagonal;
        

   member this.IsSolved() = seq [0 .. queensCount-1]
                            |> Seq.forall (fun col -> this.GetConflicts(queenPositions.[col], col) = 0)

   member this.PrintBoard() =
        for row = 0 to queensCount-1 do
            for col = 0 to queensCount-1 do         
                 if this.IsQueen(row, col) then printf "*" else printf "_";
                
            printf "%s" Environment.NewLine;


[<EntryPoint>]
let main argv =       
    let size = Int32.Parse(Console.ReadLine());
  
    let mutable game = new QueensGame(size);
    let mutable i = 0;
    let mutable continueLoop = true;

    while (continueLoop) do
       for col = 0 to size-1 do
            let minRow = seq [0 .. size - 1] |> Seq.minBy (fun row -> game.GetConflicts(row, col))
            game.SetQueen(minRow, col)

       if game.IsSolved() then              
           game.PrintBoard();
           continueLoop <-false;
                    
       i <- i + 1;

       if (i % 1000 = 0) then
            game <- new QueensGame(size); // Start over!

    printf "Solution found on %d iterations \r\n" i;
    0 // return an integer exit code
