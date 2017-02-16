module Humanoid.Test

open Expecto
open Swensen.Unquote
open FsCheck
open Humanoid

[<Tests>]
let tests =
    testList "rfc examples" [
        testCase "Example 1" (fun _ ->
            let key = 0xEB33F77EE73D4053UL
            let words = [| "TIDE"; "ITCH"; "SLOW"; "REIN"; "RULE"; "MOT" |]
            test <@ (Convert.keyToWords key) = words && Convert.wordsToKey words = key @>
        )
        testCase "Example 2 first half" (fun _ ->
            let key = 0xCCAC2AED591056BEUL
            let words = [| "RASH"; "BUSH"; "MILK"; "LOOK"; "BAD"; "BRIM" |]
            test <@ (Convert.keyToWords key) = words && Convert.wordsToKey words = key @>
        )
        testCase "Example 2 second half" (fun _ ->
            let key = 0x4F90FD441C534766UL
            let words = [| "AVID"; "GAFF"; "BAIT"; "ROT"; "POD"; "LOVE" |]
            test <@ (Convert.keyToWords key) = words && Convert.wordsToKey words = key @>
        )
        testCase "Example 3 first half" (fun _ ->
            let key = 0xEFF81F9BFBC65350UL
            let words = [| "TROD"; "MUTE"; "TAIL"; "WARM"; "CHAR"; "KONG" |]
            test <@ (Convert.keyToWords key) = words && Convert.wordsToKey words = key @>
        )
        testCase "Example 3 second half" (fun _ ->
            let key = 0x920CDD7416DE8009UL
            let words = [| "HAAG"; "CITY"; "BORE"; "O"; "TEAL"; "AWL" |]
            test <@ (Convert.keyToWords key) = words && Convert.wordsToKey words = key @>
        )
        testCase "Roundtrip" (fun _ ->
            Check.QuickThrowOnFailure(fun i -> test <@ i |> Convert.keyToWords |> Convert.wordsToKey = i @>)
        )
        testCase "Roundtrip bigger" (fun _ ->
            let bigUInt64 = gen {
                let! shift = Gen.choose(0,64) 
                let! res = Arb.Default.UInt64().Generator
                return res <<< shift }
            Check.QuickThrowOnFailure(
                Prop.forAll (Arb.fromGenShrink(bigUInt64,Arb.Default.UInt64().Shrinker)) (fun i  -> 
                test <@ i |> Convert.keyToWords |> Convert.wordsToKey = i @>))
        )
    ]

[<EntryPoint>]
let main argv =
    Tests.runTestsInAssembly defaultConfig argv
