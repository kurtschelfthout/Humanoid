#load "Data.fs"
#load "Humanoid.fs"
open Humanoid
open System


Memo.fromUInt64 02035567889UL

Memo.toUInt64 [|"a"; "a"; "a"; "use"; "wed"; "gale"|]

Memo.fromGuid <| Guid.Parse("a4133885-613b-40c9-aa4a-94e1c143918f")

Memo.toGuid [|"tin"; "ally"; "atom"; "acid"; "phi"; "tun"; "grab"; "gave"; "tuba";"nell"; "web"; "been"|]


Memo.fromUInt64Short 6587UL

Memo.toUInt64Short [ "alp"; "skim" ]