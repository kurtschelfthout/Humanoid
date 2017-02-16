namespace Humanoid

module Convert =
    open System
    let extract key start len =
        let count = if start + len > 64 then 64 - start else len
        let mask = ~~~(0xffffffffffffffffUL <<< count)
        let temp = key >>> (64 - start - count) &&& mask
        temp <<< (len - count)

    let checkSum key =
        let pairSum = 
            [|0..2..62|] 
            |> Array.map (fun start -> extract key start 2)
            |> Array.sum
        pairSum &&& 0x03UL
        
    let keyToWords key =
        let keyCheckSum = checkSum key
        let indices = [|0..11..55|] |> Array.map (fun start -> extract key start 11)
        // 5th element is the last.
        indices.[5] <- indices.[5] ||| keyCheckSum
        indices 
        |> Seq.map (fun idx -> Data.wordList.[int idx])
        |> Seq.toArray

    let wordsToKey ws =
        let findWord w = Array.IndexOf(Data.wordList, w)
        let indices = ws |> Seq.map findWord |> Seq.toArray
        if indices |> Seq.exists (fun i -> i < 0) then failwithf "One of the words cannot be found in the word list."
        let indices = indices |> Array.map uint64
        let buildKey (bits, index) (acc:uint64) = (if bits >=0 then index <<< bits else index >>> abs bits) ||| acc
        let key = 
            indices
            |> Array.zip [| 53..(-11)..(-2)|]
            |> fun l -> Array.foldBack buildKey l 0UL
        let computedSum = checkSum key
        let providedSum = indices.[indices.Length-1] &&& 0x03UL
        if computedSum <> providedSum then failwith "Checksum failed."
        key
        
    let key = 0xEB33F77EE73D4053UL
    let ws = keyToWords key |> Seq.toArray
    let keyRt = ws |> wordsToKey

    let ui = System.BitConverter.ToUInt64(System.BitConverter.GetBytes -0xfffffffffffL,0)
    let i = System.BitConverter.ToInt64(System.BitConverter.GetBytes ui,0)