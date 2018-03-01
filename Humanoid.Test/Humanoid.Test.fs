module Humanoid.Test

open System
open Expecto
open Swensen.Unquote
open FsCheck
open Humanoid

[<Tests>]
let tests =
    let memo = new Memo(Humanoid.WordList.RFC1751)
    testList "All" [
        testList "RFC 1751 examples" [
            testCase "Example 1" (fun _ ->
                let key = 0xEB33F77EE73D4053UL
                let words = [| "tide"; "itch"; "slow"; "rein"; "rule"; "mot" |]
                test <@ (memo.FromUInt64 key) = words && memo.ToUInt64 words = key @>
            )
            testCase "Example 2 first half" (fun _ ->
                let key = 0xCCAC2AED591056BEUL
                let words = [| "rash"; "bush"; "milk"; "look"; "bad"; "brim" |]
                test <@ (memo.FromUInt64 key) = words && memo.ToUInt64 words = key @>
            )
            testCase "Example 2 second half" (fun _ ->
                let key = 0x4F90FD441C534766UL
                let words = [| "avid"; "gaff"; "bait"; "rot"; "pod"; "love" |]
                test <@ (memo.FromUInt64 key) = words && memo.ToUInt64 words = key @>
            )
            testCase "Example 3 first half" (fun _ ->
                let key = 0xEFF81F9BFBC65350UL
                let words = [| "trod"; "mute"; "tail"; "warm"; "char"; "kong" |]
                test <@ (memo.FromUInt64 key) = words && memo.ToUInt64 words = key @>
            )
            testCase "Example 3 second half" (fun _ ->
                let key = 0x920CDD7416DE8009UL
                let words = [| "haag"; "city"; "bore"; "o"; "teal"; "awl" |]
                test <@ (memo.FromUInt64 key) = words && memo.ToUInt64 words = key @>
            )
            testCase "Roundtrip single key" (fun _ ->
                Check.QuickThrowOnFailure(fun (DoNotSize i)  -> 
                    test <@ i |> memo.FromUInt64 |> memo.ToUInt64 = i @>)
            )
            testCase "Roundtrip single key is case independent" (fun _ ->
                Check.QuickThrowOnFailure(fun (DoNotSize i)  -> 
                    test <@ i |> memo.FromUInt64 |> Seq.map (fun s -> s.ToLowerInvariant())  |> memo.ToUInt64 = i @>)
            )
            testCase "Roundtrip sequence of keys" (fun _ ->
                Check.QuickThrowOnFailure(fun (i:list<DoNotSize<uint64>>) ->
                    let ints = i |> List.map (fun (DoNotSize i) -> i)
                    test <@ ints |> memo.FromUInt64s |> memo.ToUInt64s |> Seq.toList = ints @>
                )
            )
            testCase "Roundtrip sequence of keys is case independent" (fun _ ->
                Check.QuickThrowOnFailure(fun (i:list<DoNotSize<uint64>>) ->
                    let ints = i |> List.map (fun (DoNotSize i) -> i)
                    test <@ ints |> memo.FromUInt64s |> Seq.map (fun s -> s.ToLowerInvariant()) |> memo.ToUInt64s |> Seq.toList = ints @>
                )
            )
        ]

        testList "Input validation tests" [
            testCase "Unexpected word" (fun _ ->
                raisesWith<ArgumentException> <@ [| "HAAG"; "CITY"; "BORING"; "O"; "TEAL"; "AWL" |] |> memo.ToUInt64 @> (fun e -> <@ e.Message.Contains "BORING" @>)
            )
            testCase "Checksum failed" (fun _ ->
                raisesWith<Exception> <@ [| "HAAG"; "CITY"; "BORE"; "OWL"; "TEAL"; "AWL" |] |> memo.ToUInt64 @> (fun e -> <@ e.Message.Contains "Checksum" @>)
            )
        ]

        testList "Guids" [
            testCase "Roundtrip single Guid" (fun _ ->
                Check.QuickThrowOnFailure(fun guid  -> 
                    test <@ guid |> memo.FromGuid |> memo.ToGuid = guid @>)
            )
        ]
            
        testList "Short UInt64" [
            testCase "Roundtrip short UInt64" (fun _ ->
                Check.QuickThrowOnFailure(fun i  -> 
                    test <@ i |> memo.FromUInt64 |> memo.ToUInt64 = i @>)
            )
            testCase "0UL converts to 'a'" (fun _ ->
                test <@ memo.FromUInt64Short 0UL = [| "a" |] @>
            )
        ]
    ]

[<EntryPoint>]
let main argv =
    Tests.runTestsInAssembly defaultConfig argv
