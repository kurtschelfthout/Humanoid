# Humanoid - Human friendly Ids

Humanoid provides a small and simple F# API to convert identifiers (like integers, guids, strings, ...) into a representation that is more easy to pronounce, shout, remember and generally communicate to humans: a bunch of words.

Humanoid is at its core an implementation of [RFC 1751](https://tools.ietf.org/html/rfc1751), with a few small extensions.

## Quickstart

First, we need a simple instance of the ```Memo``` type so it knows which word list to use:

```fsharp
> let memo = Memo(WordList.RFC1751)
```

Here's a phone number:

```fsharp
> memo.FromUInt64 02035567889UL
val it : string [] = [|"a"; "a"; "a"; "uses"; "wed"; "gale"|]
```

That list of six words from the word list uniquely corresponds to the given integer. So given the six words, we can recover it exactly:

```fsharp
> memo.ToUInt64 [|"a"; "a"; "a"; "uses"; "wed"; "gale"|]
val it : uint64 = 2035567889UL
```

Really any 64 bits corresponds uniquely to a list of six words. Humanoid just consists of a few simple functions to convert between the two representations.

The six words have a few more bits of information for error checking, so say we mishear:

```fsharp
memo.ToUInt64 [|"a"; "a"; "a"; "use"; "wed"; "gale"|]
System.Exception: Checksum failed.
   at FSI_0003.Humanoid.Memo.ToUInt64(IEnumerable`1 words)
   at <StartupCode$FSI_0009>.$FSI_0009.main@()
Stopped due to error
```

## How does it work?

Humanoid, as per RFC 1751, contains a list of 2048, or 2^11, words. In other words, each word encodes 11 bits of information. So to represent 64 bits, we need at least 6 words. Since 6*11 is 66 bits, Humanoid uses the two leftover bits as a checksum to catch errors.

You can think of the words as a base 2048 number system. (This is only approximately correct because of the checksum, but the intuition is correct) As you can tell from the above, the word 'a' is zero.

## More stuff

RFC 1751 also describes how to encode 128bits as a list of 12 words - just by converting the two 64-bit parts separately. A guid is a good example:

```fsharp
> memo.FromGuid <| Guid.Parse("a4133885-613b-40c9-aa4a-94e1c143918f")
val it : string [] = [|"tin"; "ally"; "atom"; "acid"; "phi"; "tun"; "grab"; "gave"; "tuba"; "nell"; "web"; "been"|]

> memo.ToGuid [|"tin"; "ally"; "atom"; "acid"; "phi"; "tun"; "grab"; "gave"; "tuba";"nell"; "web"; "been"|]
val it : Guid = a4133885-613b-40c9-aa4a-94e1c143918f
```

Humanoid generalizes that further and can convert a sequence of 64 bits to a list of words and back, using ```Memo.FromUInt64s``` and ```Memo.ToUInt64s```.

If you have a small number, relative to the range of a UInt64, the leading words will be 'a' because that's the 0th word in the wordlist. Humanoid can omit leading 'a's, similar to omitting leading zeroes:

```fsharp
> memo.FromUInt64Short 6587UL
val it : string [] = [|"alp"; "skim"|]

> memo.ToUInt64Short [ "alp"; "skim" ]
val it : uint64 = 6587UL
```

You may find 'alp skim' or the like easier to remember than your pin code.

Finally, there is also a ```WordList.Frequent``` with the 2048 most frequently used words from a British English corpus. (Obviously depending on the corpus, the most frequently used words will be different.)

```fsharp
> let memo = Memo(WordList.Frequent)
> memo.FromGuid <| Guid.Parse("a4133885-613b-40c9-aa4a-94e1c143918f")
val it : string [] =
  [|"grow"; "kill"; "manage"; "top"; "team"; "union"; "distance"; "target";
    "yellow"; "combination"; "herself"; "principle"|]
```

## Why don't you tell me what you really think?

This is a cute idea, and there is some potential here. Obviously to communicate an identifier, say in a situation where you're troubleshooting a production bug that manifests itself when someone tries to edit an entity with a particular id, it's much easier for both you and the other person to communicate by copy-pasting the id in a chat or email.

But I've been in situations where - esp. where only email is available - this breaks the flow of communication because sending and receiving an email can take some time. Probably longer than for one person to say six short words, and for you to type them in. Also some normal people (I mean non-programmers...) like to send screenshots when they encounter a problem, and in that case copy-pasting is not an option. I'd rather type 12 short words than a guid.

Using words leverages dictionary-based completion, say you're typing an identifier on a mobile device.

Some identifiers (guids, SHAs) are so atrocious that people have taken to talk about them by saying the first few characters. I've done this sometimes when talking to someone about a few commits. I'd rather talk about commits 'alp' and 'bean' than 'c4gfe' and 'ffb4e'.

How many times have you had to say your credit card number, account number and so on over the phone? Wouldn't you rather have said a few short words instead?

That said, I don't find RFC 1751 entirely satsifactory. In particular, the choice of word list is not great - they are all 1-4 letter words and thus short, but shortness in this case is not necessarily a virtue. For example, 'bean', 'been' and 'beam' are all in there, and those are impossible or hard to distinguish by ear. Also it makes them harder to recall if you don't have a strong visual memory. Hence the alternative list of 2048 frequently used words (no matter the length) is probably better.

Even more memorable would be to form simple sentences given lists of nouns, verbs and adjectives: 'The quick fox jumps over the lazy dog' is also 6 words (2 adjectives, 1 verb, 1 preposition, 2 nouns) and is a lot more memorable than 6 somewhat random words, even frequently used ones.

Finally, to quote Nelson Mandela:

> If you talk to a man in a language he understands, that goes to his head. If you talk to him in his language, that goes to his heart.

So to be human friendly, the words should be in whatever language the user is most comfortable in. Luckily, you can plug in whatever word list you need - there just need to be exactly 2048 distinct words.

## Ok, but still: Why?

I thought about the problem of human-readable identifiers for a bit, and found out by accident that an RFC exists for exactly that. I found it interesting enough to tinker with. I haven't actually used this for anything real. I have seen something like this used though, e.g. Keybase generates a list of words to be used as a paper public key instead of the usual Base64 encoded gibberish.
