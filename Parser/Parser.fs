#light

//    Ook. Ook?   Increment the pointer 
//    Ook? Ook.   Decrement the pointer 
//    Ook. Ook.   Increment the value at the pointer 
//    Ook! Ook!   Decrement the value at the pointer 
//    Ook! Ook.   Write the value at the pointer to standard output 
//    Ook. Ook!   Read a value from standard input and store it at the pointer 
//    Ook! Ook?   If the value at the pointer is zero, jump past the matching Ook? Ook! 
//    Ook? Ook!   Jump to the matching Ook! Ook?

module Parser

    open System.Text.RegularExpressions
    open System

    let clean (line : string) =
        String.trim [' '; '\n'] line

    let split (line : string) =
        Array.to_list (Regex.Split(line, @"\s+"))
        
    let rec tokenise (split : List<string>) = 
        if List.nonempty split then
            let h = split.Head
            let h2 = split.Tail.Head
            (h + " " + h2) :: (tokenise split.Tail.Tail)
        else
            []
           
       //match text with
       //    | "Ook. Ook?" -> "increment"
       //    | _ -> "undef"   

    exception IncorrectToken of string

    let rec lex (tokens : List<string>) = 
        if List.nonempty tokens then
            let head = tokens.Head
            let l = 
                match head with
                    | "Ook. Ook?" -> "increment-pointer"
                    | "Ook? Ook." -> "decrement-pointer"
                    | "Ook. Ook." -> "increment-value"
                    | "Ook! Ook!" -> "decrement-value"
                    | "Ook! Ook." -> "stdout"
                    | "Ook. Ook!" -> "stdin"
                    | "Ook! Ook?" -> "jump-ifzero"
                    | "Ook? Ook!" -> "jump-ifnotzero"
                    | head -> raise (IncorrectToken head)
            l :: (lex tokens.Tail)
        else
           []
           
    let parse text =
        lex(tokenise(split(clean(text))))

