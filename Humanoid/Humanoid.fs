namespace Humanoid

module Memo =
    open System
    let private extract key start len =
        let count = if start + len > 64 then 64 - start else len
        let mask = ~~~(0xffffffffffffffffUL <<< count)
        let temp = key >>> (64 - start - count) &&& mask
        temp <<< (len - count)

    let private checkSum key =
        let pairSum = 
            [|0..2..62|] 
            |> Array.sumBy (fun start -> extract key start 2)
        pairSum &&& 0x03UL
        
    let ofKey key =
        let keyCheckSum = checkSum key
        let indices = [|0..11..55|] |> Array.map (fun start -> extract key start 11)
        // 5th element is the last.
        indices.[5] <- indices.[5] ||| keyCheckSum
        indices 
        |> Seq.map (fun idx -> Data.wordList.[int idx])
        |> Seq.toArray

    let toKey words =
        let findWord w = Data.wordList |> Array.tryFindIndex (fun elem -> String.Equals(elem, w, StringComparison.InvariantCultureIgnoreCase))
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

    let ofKeys keys =
        keys
        |> Seq.collect ofKey
        |> Seq.toArray
    let toKeys words =
        words
        |> Seq.chunkBySize 6
        |> Seq.map toKey
        |> Seq.toArray
