namespace Humanoid

open System

type Memo(wordList:string[]) =

    let extract key start len =
        let count = if start + len > 64 then 64 - start else len
        let mask = ~~~(0xffffffffffffffffUL <<< count)
        let temp = key >>> (64 - start - count) &&& mask
        temp <<< (len - count)

    let checkSum key =
        let pairSum = 
            [|0..2..62|] 
            |> Array.sumBy (fun start -> extract key start 2)
        pairSum &&& 0x03UL

    let  assert2048 wordList =
        let l = 
            wordList
            |> Seq.distinct
            |> Seq.length
        if l <> 2048 then failwithf "Word list should consist of exactly 2048 distinct words."        

    do assert2048 wordList

    /// Converts an UInt64 into exactly 6 words from the word list
    /// that uniquely correspond to it.
    member __.FromUInt64 (key:uint64) : string[] =
        let keyCheckSum = checkSum key
        let indices = [|0..11..55|] |> Array.map (fun start -> extract key start 11)
        // 5th element is the last.
        indices.[5] <- indices.[5] ||| keyCheckSum
        indices 
        |> Seq.map (fun idx -> wordList.[int idx])
        |> Seq.toArray

    /// Takes 6 words, checksums them, and if the checksum succeeds, returns the unique
    /// UInt64 that corresponds to the words.
    member __.ToUInt64 (words:seq<string>) : uint64 =
        let findWord w = wordList |> Array.tryFindIndex (fun elem -> String.Equals(elem, w, StringComparison.OrdinalIgnoreCase))
        let indices = words |> Seq.map findWord |> Seq.toArray
        let notFound = indices |> Seq.exists (fun i -> i.IsNone) 
        if notFound then
            let unknownWords = indices |> Seq.zip words |> Seq.where (fun (_,i) -> i.IsNone) |> Seq.map fst
            invalidArg "words" (sprintf "Could not find %s in the word list." (String.Join(", ", unknownWords)))
        let indices = indices |> Array.map (Option.get >> uint64)
        let buildKey (bits, index) (acc:uint64) = (if bits >=0 then index <<< bits else index >>> abs bits) ||| acc
        let key = 
            indices
            |> Array.zip [| 53..(-11)..(-2)|]
            |> fun l -> Array.foldBack buildKey l 0UL
        let computedSum = checkSum key
        let providedSum = indices.[indices.Length-1] &&& 0x03UL
        if computedSum <> providedSum then failwith "Checksum failed."
        key

    /// Takes any number of UInt64 and converts them to a list of words that
    /// uniquely correspond to them. The list of words is a multiple of 6.
    member t.FromUInt64s keys =
        keys
        |> Seq.collect t.FromUInt64
        |> Seq.toArray

    /// Takes a list of words of any length, with length a multiple of 6,
    /// and converts them to a sequnce of UInt64s that uniquely correspond to them.
    member t.ToUInt64s (words:seq<string>) =
        words
        |> Seq.chunkBySize 6
        |> Seq.map t.ToUInt64
        |> Seq.toArray
    
    /// Converts a Guid into a list of 12 words that uniquely correspond to it.
    member t.FromGuid (guid:Guid) =
        let bytes = guid.ToByteArray()
        let is = [| BitConverter.ToUInt64(bytes,0);BitConverter.ToUInt64(bytes,8) |]
        t.FromUInt64s is

    /// Converts a sequence of 12 words into the Guid that uniquely corresponds to them.
    member t.ToGuid words =
        let is = t.ToUInt64s words
        if is.Length <> 2 then invalidArg "words" "Cannot create Guid from the given words."
        let bytes = Array.append (BitConverter.GetBytes(is.[0])) (BitConverter.GetBytes(is.[1]))
        Guid(bytes)

    /// Convert a UInt64 into a list of 1-6 words. The smaller the key, the shorter
    /// the list of words.
    member t.FromUInt64Short key =
        let words = t.FromUInt64 key
        let result = 
            words
            |> Array.skipWhile (fun word -> word.Equals(wordList.[0]))
        //don't omit the last "zero"
        if result.Length = 0 then [| wordList.[0] |] else result

    /// Convert a list of 1-6 words into a UInt64.
    member t.ToUInt64Short words =
        let length = Seq.length words
        if length < 1 || length > 6 then invalidArg "words" "Number of words must be between 1 and 6."
        let nbOfLeadingZeroes = 6 - length
        let padToSix = 
            [| for _ in 1..nbOfLeadingZeroes -> wordList.[0]
               for w in words -> w |]
        t.ToUInt64 padToSix
