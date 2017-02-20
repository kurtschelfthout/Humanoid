#load "Data.fs"
#load "Humanoid.fs"
open Humanoid
open System

let g = Guid.NewGuid()
let i = [| BitConverter.ToUInt64(g.ToByteArray(),0);BitConverter.ToUInt64(g.ToByteArray(),8)|]


Memo.ofKeys i

Memo.ofKey (uint64 Int32.MaxValue)