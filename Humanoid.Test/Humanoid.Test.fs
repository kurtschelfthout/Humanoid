module Humanoid.Test

open System
open Expecto
open Swensen.Unquote
open FsCheck
open Humanoid

[<Tests>]
let tests =
    testList "All" [

        testList "rfc examples" [
            testCase "Example 1" (fun _ ->
                let key = 0xEB33F77EE73D4053UL
                let words = [| "TIDE"; "ITCH"; "SLOW"; "REIN"; "RULE"; "MOT" |]
                test <@ (Memo.ofKey key) = words && Memo.toKey words = key @>
            )
            testCase "Example 2 first half" (fun _ ->
                let key = 0xCCAC2AED591056BEUL
                let words = [| "RASH"; "BUSH"; "MILK"; "LOOK"; "BAD"; "BRIM" |]
                test <@ (Memo.ofKey key) = words && Memo.toKey words = key @>
            )
            testCase "Example 2 second half" (fun _ ->
                let key = 0x4F90FD441C534766UL
                let words = [| "AVID"; "GAFF"; "BAIT"; "ROT"; "POD"; "LOVE" |]
                test <@ (Memo.ofKey key) = words && Memo.toKey words = key @>
            )
            testCase "Example 3 first half" (fun _ ->
                let key = 0xEFF81F9BFBC65350UL
                let words = [| "TROD"; "MUTE"; "TAIL"; "WARM"; "CHAR"; "KONG" |]
                test <@ (Memo.ofKey key) = words && Memo.toKey words = key @>
            )
            testCase "Example 3 second half" (fun _ ->
                let key = 0x920CDD7416DE8009UL
                let words = [| "HAAG"; "CITY"; "BORE"; "O"; "TEAL"; "AWL" |]
                test <@ (Memo.ofKey key) = words && Memo.toKey words = key @>
            )
            testCase "Roundtrip single uint64" (fun _ ->
                Check.QuickThrowOnFailure(fun (DoNotSize i)  -> 
                    test <@ i |> Memo.ofKey |> Memo.toKey = i @>)
            )
        ]

        testList "Roundtrip sequence of keys" [
            testCase "Sequence of uint64" (fun _ ->
                Check.QuickThrowOnFailure(fun (i:list<DoNotSize<uint64>>) ->
                    let ints = i |> List.map (fun (DoNotSize i) -> i)
                    test <@ ints |> Memo.ofKeys |> Memo.toKeys |> Seq.toList = ints @>
                )
            )
        ]

        testList "Input validation tests" [
            testCase "Unexpected word" (fun _ ->
                raisesWith<ArgumentException> <@ [| "HAAG"; "CITY"; "BORING"; "O"; "TEAL"; "AWL" |] |> Memo.toKey @> (fun e -> <@ e.Message.Contains "BORING" @>)
            )
            testCase "Checksum failed" (fun _ ->
                raisesWith<Exception> <@ [| "HAAG"; "CITY"; "BORE"; "OWL"; "TEAL"; "AWL" |] |> Memo.toKey @> (fun e -> <@ e.Message.Contains "Checksum" @>)
            )
        ]
    ]

[<EntryPoint>]
let main argv =
    Tests.runTestsInAssembly defaultConfig argv
