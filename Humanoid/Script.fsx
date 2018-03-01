open System.IO
#load "WordList.fs"
#load "Humanoid.fs"
open Humanoid
open System

let memo = Memo(WordList.Frequent)
memo.FromUInt64 2035567889UL
//Memo.ToUInt64 it
//toUInt64 [|"a"; "a"; "a"; "use"; "wed"; "gale"|]

memo.FromGuid <| Guid.Parse("a4133885-613b-40c9-aa4a-94e1c143918f")

// do  filename
//     |> File.ReadAllLines
//     |> Seq.map (fun s -> "\"" + s.Split(' ').[2] + "\"")
//     |> Seq.distinct
//     |> Seq.take 2048
//     |> Seq.chunkBySize 6
//     |> Seq.map (fun ch -> String.Join("; ", ch))
//     |> Seq.toArray
//     |> fun s -> File.WriteAllLines("Output.fs",s)

// Memo.fromGuid <| Guid.Parse("a4133885-613b-40c9-aa4a-94e1c143918f")

// Memo.toGuid [|"tin"; "ally"; "atom"; "acid"; "phi"; "tun"; "grab"; "gave"; "tuba";"nell"; "web"; "been"|]

// Memo.fromUInt64Short 6587UL

// Memo.toUInt64Short [ "alp"; "skim" ]

